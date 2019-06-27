#include <alsa/asoundlib.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include "TCP_Client.hpp"
#include "AFX.hpp"
#include "pcm.hpp"
#include "gpio.hpp"
int capture_size;  //获取捕获大小
int play_size;      //播放大小

int audio_start_cap =0;
int audio_start_play =0;
int audio_deal_play=0;
int audio_deal_cap =0;
int audio_end_play=0;
int audio_end_cap=0;

char *capture_buff;  //捕获缓冲指针
char *play_buff;
struct WAV_HEADER wav_header;

struct SND_PARA{
	snd_pcm_t *handle;
	snd_pcm_hw_params_t *params;
};

void *open_capture(void *arg)   //语音捕获线程
{
	snd_pcm_t *handle;          //snd pcm句柄
    snd_pcm_hw_params_t *params; //硬件参数
    snd_pcm_uframes_t frames;    //帧数
    unsigned int val;
    int rc,dir;
    struct sockaddr_in s_addr;
    int sockfd;
    if((sockfd=socket(AF_INET,SOCK_DGRAM,0))==-1)   //开UDP用来发送
    {
        perror("socket");
        exit(errno);
    }
    else
        printf("creat sockfd success!.\n\r");
    s_addr.sin_family=AF_INET;
    s_addr.sin_port=htons(7000);
    s_addr.sin_addr.s_addr=inet_addr(remote_ip);

    rc = snd_pcm_open(&handle, "default", SND_PCM_STREAM_CAPTURE, 0);   //开语音设备
    if (rc < 0)
    {
        fprintf(stderr, "unable to open pcm device: %s\n", snd_strerror(rc));
    }
     snd_pcm_hw_params_alloca(&params);                                        //配置
     snd_pcm_hw_params_any(handle, params);
     snd_pcm_hw_params_set_access(handle, params, SND_PCM_ACCESS_RW_INTERLEAVED);
     snd_pcm_hw_params_set_format(handle, params, SND_PCM_FORMAT_S16);
     snd_pcm_hw_params_set_channels(handle, params, 2);
     val = 8000;
     snd_pcm_hw_params_set_rate_near(handle, params, &val, &dir);
     frames = 32;
     snd_pcm_hw_params_set_period_size_near(handle, params, &frames, &dir);
     rc = snd_pcm_hw_params(handle, params);
     if (rc < 0)
     {
         fprintf(stderr, "unable to set hwparameters: %s\n", snd_strerror(rc));
         exit(1);
     }
     snd_pcm_hw_params_get_period_size(params, &frames, &dir);
     frames = 320;
     capture_size = frames * 2 * 2; /* 2 bytes/sample, 2 channels */
     capture_buff = (char *) malloc(capture_size);
     printf("capture_size %d",capture_size);
     if(capture_buff == NULL)
     {
         fprintf(stderr, "Not enough Memory!\n");
     }
     snd_pcm_hw_params_get_period_time(params, &val, &dir);
     //printf("val %d\n",val);
     while (1)                                                            //语音线程循环
     {
         if(audio_start_cap == 1)
         {
//        	 audio_start_cap = 0;
        	 struct SND_PARA snd_para;
        	 snd_para.handle = handle;
        	 snd_para.params = params;
        	 pthread_t th_cap;
        	 int res = 0;
        	 int ret_cap=0;
        	 if((res = pthread_create(&th_cap,NULL,test_open_capture,(void *)&snd_para))!=0)
        	 {
        		 perror("th_cap pthread create failed\n");
        		 audio_deal_cap = DEV_OK;
        	 }
        	 else
        	 {
				 pthread_join(th_cap,(void **)&ret_cap);
				 audio_deal_cap = ret_cap;
        	 }
         	 audio_end_cap = 1;
         	 printf("test open cap .....................%d\n",audio_deal_cap);
         	 sleep(5);
         }
         else if(radio_pass)                                                   //如果接通，采集并发送
    	 {
			  rc = snd_pcm_readi(handle, capture_buff, frames);
			  printf("read a frames date\n");
			  if (rc == -EPIPE)
			  {
				fprintf(stderr, "overrun occurred\n");                  //EPIPE means overrun
				snd_pcm_prepare(handle);
			  }
			  else if (rc < 0)
			  {
				fprintf(stderr,"error from read: %s\n",snd_strerror(rc));
			  }
			  else if (rc != (int)frames)
			  {
				fprintf(stderr, "short read, read %d frames\n", rc);
			  }
			  sendto(sockfd,capture_buff,capture_size,0,(struct sockaddr * )& s_addr,sizeof(struct sockaddr));
    	 }
     }
     snd_pcm_drop(handle);
     snd_pcm_drain(handle);
     snd_pcm_close(handle);
     free(capture_buff);
}
void *open_play(void * arg)
{
    snd_pcm_t *handle;
    snd_pcm_hw_params_t *params;
    snd_pcm_uframes_t frames;
    unsigned int val;
    int rc;
    int dir;

    struct sockaddr_in s_addr;
    struct sockaddr_in c_addr;
    int sock;
    socklen_t addr_len;
    if((sock=socket(AF_INET,SOCK_DGRAM,0))==-1)  //使用UDP方式
    {
        perror("socket");
        exit(errno);
    }
    else
        printf("creat socket success.\n\r");
    memset(&s_addr,0,sizeof(struct sockaddr_in));
    s_addr.sin_family=AF_INET;   //协议设为AF_INET
    s_addr.sin_port=htons(7000);  //接受端口为7000
    s_addr.sin_addr.s_addr=INADDR_ANY;  //本地任意IP

    if((bind(sock,(struct sockaddr *) &s_addr,sizeof(s_addr)))==-1)
    {
        perror("bind");
        exit(errno);
    }
    else
        printf("bind address to socket.\n\r");
    addr_len=sizeof(c_addr);
    rc = snd_pcm_open(&handle, "default", SND_PCM_STREAM_PLAYBACK, SND_PCM_NONBLOCK);
    if (rc < 0)
    {
        fprintf(stderr, "unable to open pcm device: %s\n", snd_strerror(rc));
    }
    snd_pcm_hw_params_alloca(&params);
    snd_pcm_hw_params_any(handle, params);
    snd_pcm_hw_params_set_access(handle, params, SND_PCM_ACCESS_RW_INTERLEAVED);
    snd_pcm_hw_params_set_format(handle, params, SND_PCM_FORMAT_S16);
    snd_pcm_hw_params_set_channels(handle, params, 2);
    val = 8000;
    snd_pcm_hw_params_set_rate_near(handle, params, &val, &dir);
    frames = 32;
    snd_pcm_hw_params_set_period_size_near(handle, params, &frames, &dir);
    rc = snd_pcm_hw_params(handle, params);
    if (rc < 0)
    {
        fprintf(stderr, "unable to set hw parameters: %s\n", snd_strerror(rc));
    }
    snd_pcm_hw_params_get_period_size(params, &frames, &dir);
    frames = 250;
    play_size = frames * 2 * 2; /* 2 bytes/sample, 2 channels */
//    printf("play_size %d",play_size);
    play_buff = (char *) malloc(play_size);
    if(play_buff == NULL)
    {
        fprintf(stderr, "Not enough Memory!\n");
    }
    snd_pcm_hw_params_get_period_time(params, &val, &dir);
    //va_g729a_init_decoder();
    while (1)
    {
        if(audio_start_play == 1)      //测试语音代码
        {
        	struct SND_PARA snd_para;
        	snd_para.handle = handle;
        	snd_para.params = params;
        	pthread_t pth_snd;
        	int pth_ret;
        	int res =0;

        	if((res = pthread_create(&pth_snd,NULL,test_open_play,(void *)&snd_para)) !=0)
        	{
        		perror("pth_snd thread create failed\n");
        		audio_deal_play = DEV_OK;
        	}
        	else
        	{
//				res = pthread_create(&pth_snd,NULL,test_open_play,(void *)&snd_para);
        		printf("wait pth_snd end.............\n");
				pthread_join(pth_snd,(void**)&pth_ret);
				if(pth_ret == -1)
				{
					audio_deal_play = 0;            //歌曲文件失败，默认扬声器好
					printf("open wav file failed\n");
				}
				else
				{
					audio_deal_play = pth_ret;
				}
				printf("had pth_snd return\n");
				audio_end_play = 1;
				printf("test open play .....................%d\n",audio_deal_play);
        	}
        }
        else if(radio_pass)
    	{
    		rc = recvfrom(sock,play_buff,play_size,0,(struct sockaddr *)&c_addr,&addr_len);
			rc = snd_pcm_writei(handle, play_buff, frames);
			if (rc == -EPIPE) {
				/* EPIPE means underrun */
				fprintf(stderr, "underrun occurred\n");
				snd_pcm_prepare(handle);
			} else if (rc < 0) {
				fprintf(stderr,"error from writei: %s\n", snd_strerror(rc));
			} else if (rc != (int)frames) {
				fprintf(stderr,"short write, write %d frames\n", rc);
			}
    	}
    }
    snd_pcm_drop(handle);
    snd_pcm_drain(handle);
    snd_pcm_close(handle);
    free(play_buff);
    return 0;
}
//测试麦克风,
//返回值：0->正常，1->不正常
void* test_open_capture(void* pcm)
{
	int test_capture_size;        //获取捕获大小
	char *test_capture_buff;      //捕获缓冲
    snd_pcm_uframes_t frames;     //帧数
    unsigned int i;
    int ret_capture=0;
    SND_PARA* snd_pcm = (SND_PARA *)pcm;
	snd_pcm_t* handle = snd_pcm->handle; //PCI设备句柄
	snd_pcm_hw_params_t* params = snd_pcm->params;//硬件信息和PCM流配置
	frames = 320;
	test_capture_size = frames * 2 * 2; /* 2 bytes/sample, 2 channels */
	test_capture_buff = (char *) malloc(test_capture_size);
	memset(test_capture_buff,0,test_capture_size);
	if(test_capture_buff == NULL)
	{
	  fprintf(stderr, "Not enough Memory!\n");
	}
	//测试代码，只需要读一次数据就行
	int rc = snd_pcm_readi(handle, test_capture_buff, frames);
	if (rc == -EPIPE) {
	 fprintf(stderr, "overrun occurred...\n");
	 snd_pcm_prepare(handle);
	} else if (rc < 0) {
	 fprintf(stderr, "error from read:%s...\n", snd_strerror(rc));
	} else if (rc != (int)frames) {
	 fprintf(stderr, "shord read, read %d frames...\n", rc);
	}
	//对采集到的数据进行判断，若为全0也证明采集器不存在
	i = 0;
	while (i < 1300)
	{
	 if (test_capture_buff[i] != 0)break;
	 i++;
	}
	printf("pcm test length is %d...\n", i);
	if (i < 1300)ret_capture = DEV_OK;
	else ret_capture = DEV_ERR;
	printf("...........ret_capture is ...........%d\n",ret_capture);
	free(test_capture_buff);
//	return ret_capture;
	pthread_exit((void *)ret_capture);
}
//测试扬声器
void* test_open_play(void* pcm)
{
	int rc,dir;
	int ret;
	int size;
	int undercont = 0;
	int ret_open = 0;
/**************************取通用指针void* 中元素的常用写法********************/
	SND_PARA* snd_pcm = (SND_PARA *)pcm;
	snd_pcm_t* handle = snd_pcm->handle; //PCI设备句柄
	snd_pcm_hw_params_t* params = snd_pcm->params;//硬件信息和PCM流配置
/***********************************************************************/

/**************************取通用指针void* 的另一种写法********************/
//	snd_pcm_t* handle = ((SND_PARA *)pcm)->handle;
//	snd_pcm_hw_params_t* params = ((SND_PARA *)pcm)->params;
/*********************************************************************/
	unsigned int val = 0;
	int nread=0;
	snd_pcm_uframes_t frames;
	char *buffer;
	FILE *fp;
	fp=fopen("test.wav","rb");
	if(fp==NULL)
	{
		perror("open file failed:\n");
		ret_open = -1;
		pthread_exit((void *)&ret_open);
	}
	nread=fread(&wav_header,1,sizeof(wav_header),fp);
//	int channels=wav_header.wChannels;
//	int frequency=wav_header.nSamplesPersec;
//	int bit=wav_header.wBitsPerSample;
	int datablock=wav_header.wBlockAlign;
	rc=snd_pcm_hw_params_get_period_size(params, &frames, &dir); /*获取周期长度*/
	if(rc<0)
	{
		perror("\nsnd_pcm_hw_params_get_period_size:");
		ret_open = -2;
		pthread_exit((void *)&ret_open);
	}
	size = frames * datablock;  /*4 代表数据快长度*/
	buffer = (char*)malloc(size);
	fseek(fp,58,SEEK_SET);     //定位歌曲到数据区
	while (1)
	{
		memset(buffer,0,sizeof(size * sizeof(char*)));
		ret = fread(buffer, 1, size, fp);
		if(ret == 0)
		{
			printf("歌曲写入结束\n");
			break;
		}
		while((ret = snd_pcm_writei(handle, buffer, frames))<0) // 写音频数据到PCM设备
		{
			 usleep(1000);
			 if (ret == -EPIPE)
			 {
				  /* EPIPE means underrun */
				  fprintf(stderr, "underrun occurred\n");
				  //完成硬件参数设置，使设备准备好
				  snd_pcm_prepare(handle);
				  if(undercont++>10)
				  {
						ret_open = -3;
						pthread_exit((void *)&ret_open);
				  }
			 }
		}
	}
	free(buffer);
	ret_open = DEV_OK;
	pthread_exit((void *)ret_open);
}

