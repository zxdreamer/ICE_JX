/*
 * test.c
 *
 *  Created on: Jun 12, 2018
 *      Author: xz
 */

#include "test.hpp"
#include "Uart0.hpp"
#include "AFX.hpp"
#include "fa_cam202.hpp"

int test_camera() {
	int fd;
	u8 ret_camera = DEV_ERR;
	if((camtest_flag & 0x01) != 0)   //摄像头已经打开，设备正常
	{
		printf("camera first back\n");
		ret_camera = DEV_OK;
	}
	else
	{
		fd = open("/dev/video0",O_RDWR,0);
		if(fd == -1)
		{
			perror("open cam2 failed\n");
			ret_camera = DEV_ERR;
		}
		else ret_camera = DEV_OK;
		close(fd);
	}
	printf("ret_camera>>>>>>>%d\n",ret_camera);
	return ret_camera;
}
//测试语音对讲的麦克风与扬声器
//返回值：0-全正常
        //1-仅麦克风有问题
		//2-仅扬声器有问题
		//3-全有问题
int test_audio() {
	u8 ret_audio=DEV_ERR;
//	pthread_mutex_lock(&test_mutex);
//	sem_wait(&test_sem);
	printf("posix is test pthread\n");
	if(audio_deal_play == DEV_OK || audio_deal_cap == DEV_OK)ret_audio = 0;
	else if(audio_deal_play == DEV_OK || audio_deal_cap == DEV_ERR)ret_audio = 1;
	else if(audio_deal_play == DEV_ERR  || audio_deal_cap == DEV_OK)ret_audio = 2;
	else if(audio_deal_play == DEV_ERR || audio_deal_cap == DEV_ERR)ret_audio = 3;
//	pthread_mutex_unlock(&test_mutex);
	return ret_audio;
}
//void test_sensor()
//{
//	u8 senFlag=0; //记录温度，湿度，加速度传感器的状态
//	XJ_Data sensor={0xff,0xff,0xff,0xff,0xff,0xff,{0},{0}};
//	collect_data(sensor);
//	senFlag |= 111<<3;
//	if(sensor.temph!=0||sensor.templ!=0)
//	{
//		senFlag |= 1<<0;     //温度正常
////		senFlag |= 1<<4;     //温度测试状态标志置位
//	}
//	if(sensor.huin != 0)
//	{
//		senFlag |= 1<<2;    //湿度正常
////		senFlag |= 1<<5;    //湿度测试状态位置位
//	}
//	if(sensor.accx !=0 || sensor.accy != 0 || sensor.accz != 0)
//	{
//		senFlag |= 1<<3;    //MPU正常
////		senFlag |= 1<<6;    //MPU测试状态位置位
//	}
//	return senFlag;
//}
int test_sensor1() {           //温度传感器
	u8 ret_sensor1 = DEV_ERR;
	XJ_Data sensor1={0x00,0x00,0x00,0x00,0x00,0x00,{0},{0}};
	collect_data2(sensor1);
	if (sensor1.temph != 0x00 || sensor1.templ != 0x00)
		ret_sensor1 = DEV_OK;
	return ret_sensor1;
}
int test_sensor2() {
	u8 ret_sensor2 = DEV_ERR;
	XJ_Data sensor2={0x00,0x00,0x00,0x00,0x00,0x00,{0},{0}};
	collect_data2(sensor2);
	if (sensor2.huin != 0x00)
		ret_sensor2 = DEV_OK;
	return ret_sensor2;
}

int test_MPU() {
	u8 ret_MPU = DEV_ERR;
	XJ_Data senmpu={0x00,0x00,0x00,0x00,0x00,0x00,{0},{0}};
	collect_data2(senmpu);
	if (senmpu.accx != 0x00 || senmpu.accy != 0x00 || senmpu.accz != 0x00)
		ret_MPU = DEV_OK;
	return ret_MPU;
}

int test_wifi() {
	u8 ret_wifi = DEV_OK;
	//在加东西
	return ret_wifi;
}
////测试麦克风,
////返回值：0->正常，1->不正常
//int test_open_capture()   //语音捕获线程
//{
//	int test_capture_size;        //获取捕获大小
//	char *test_capture_buff;      //捕获缓冲
//	snd_pcm_t *handle;            //snd pcm句柄
//    snd_pcm_hw_params_t *params;  //硬件参数
//    snd_pcm_uframes_t frames;     //帧数
//    unsigned int val;
//    int rc,dir;
//    unsigned int len;
//    unsigned int i;
//    int ret_capture=DEV_ERR;
//    if(fd_audio_open == DEV_OK)
//    {
//    	printf("audio capture first back\n");
//    	ret_capture = DEV_OK;
//    }
//    else
//    {
//        rc = snd_pcm_open(&handle, "default", SND_PCM_STREAM_CAPTURE, 0);   //开语音设备
//        if (rc < 0)
//        {
//            fprintf(stderr, "unable to open pcm device: %s\n", snd_strerror(rc));
//        }
//         snd_pcm_hw_params_alloca(&params);                                        //配置
//         snd_pcm_hw_params_any(handle, params);
//         snd_pcm_hw_params_set_access(handle, params, SND_PCM_ACCESS_RW_INTERLEAVED);
//         snd_pcm_hw_params_set_format(handle, params, SND_PCM_FORMAT_S16);
//         snd_pcm_hw_params_set_channels(handle, params, 2);
//         val = 8000;
//         snd_pcm_hw_params_set_rate_near(handle, params, &val, &dir);
//         frames = 32;
//         snd_pcm_hw_params_set_period_size_near(handle, params, &frames, &dir);
//         rc = snd_pcm_hw_params(handle, params);
//         if (rc < 0)
//         {
//             fprintf(stderr, "unable to set hwparameters: %s\n", snd_strerror(rc));
//             exit(1);
//         }
//         snd_pcm_hw_params_get_period_size(params, &frames, &dir);
//         frames = 320;
//         test_capture_size = frames * 2 * 2; /* 2 bytes/sample, 2 channels */
//         test_capture_buff = (char *) malloc(test_capture_size);
//         memset(test_capture_buff,0,test_capture_size);
//         printf("capture_size %d",test_capture_size);
//         if(test_capture_buff == NULL)
//         {
//             fprintf(stderr, "Not enough Memory!\n");
//         }
//         snd_pcm_hw_params_get_period_time(params, &val, &dir);
//         //printf("val %d\n",val);
//
//    //测试代码，只需要读一次数据就行
//    	 rc = snd_pcm_readi(handle, test_capture_buff, frames);
//    	 if (rc == -EPIPE) {
//    		 fprintf(stderr, "overrun occurred...\n");
//    		 snd_pcm_prepare(handle);
//    	 } else if (rc < 0) {
//    		 fprintf(stderr, "error from read:%s...\n", snd_strerror(rc));
//    	 } else if (rc != (int)frames) {
//    		 fprintf(stderr, "shord read, read %d frames...\n", rc);
//    	 }
//
//    //对采集到的数据进行判断，若为全0也证明采集器不存在
//    	 i = 0;
//    	 while (i < 100)
//    	 {
//    		 if (test_capture_buff[i] == 0)break;
//    		 i++;
//    	 }
//    	 printf("pcm test length is %d...\n", i);
//    	 if (i < 100)ret_capture = DEV_OK;
//    	 else ret_capture = DEV_ERR;
//
//         snd_pcm_drop(handle);
//         snd_pcm_drain(handle);
//         snd_pcm_close(handle);
//         free(test_capture_buff);
//    }
//     return ret_capture;
//}
////测试扬声器
//int test_open_play()
//{
//	int rc;
//	int ret;
//	int size;
//	snd_pcm_t* handle; //PCI设备句柄
//	snd_pcm_hw_params_t* params;//硬件信息和PCM流配置
//	unsigned int val;
//	int dir=0;
//	snd_pcm_uframes_t frames;
//	char *buffer;
//	FILE *fp;
//	int nread;
//	fp=fopen("test.wav","rb");
//	if(fp==NULL)
//	{
//		perror("open file failed:\n");
//		return 1;
//	}
//	nread=fread(&wav_header,1,sizeof(wav_header),fp);
//
//	int channels=wav_header.wChannels;
//	int frequency=wav_header.nSamplesPersec;
//	int bit=wav_header.wBitsPerSample;
//	int datablock=wav_header.wBlockAlign;
//	if(fd_audio_open != DEV_OK)
//	{
//
//	}
////	unsigned char ch[100]; //用来存储wav文件的头信息
//	rc=snd_pcm_open(&handle, "default", SND_PCM_STREAM_PLAYBACK, SND_PCM_NONBLOCK);
//	if(rc<0)                                  //确保不是因为已经打开声卡设备造成的打开失败
//	{
//		rc = snd_pcm_open(&handle, "default", SND_PCM_STREAM_PLAYBACK, SND_PCM_NONBLOCK);
//		if(rc != 0)
//		{
//			perror("\nopen PCM device failed:");
////			return 2;
//		}
//	}
//	snd_pcm_hw_params_alloca(&params); //分配params结构体
//	if(rc<0)
//	{
//		perror("\nsnd_pcm_hw_params_alloca:");
//		return 3;
//	}
//	rc=snd_pcm_hw_params_any(handle, params);//初始化params
//	if(rc<0)
//	{
//		perror("\nsnd_pcm_hw_params_any:");
//		return 4;
//	}
//	rc=snd_pcm_hw_params_set_access(handle, params, SND_PCM_ACCESS_RW_INTERLEAVED); //初始化访问权限
//	if(rc<0)
//	{
//		perror("\nsed_pcm_hw_set_access:");
//		return 5;
//	}
//	//采样位数
//	switch(bit/8)
//	{
//	case 1:snd_pcm_hw_params_set_format(handle, params, SND_PCM_FORMAT_U8);
//			break ;
//	case 2:snd_pcm_hw_params_set_format(handle, params, SND_PCM_FORMAT_S16_LE);
//			break ;
//	case 3:snd_pcm_hw_params_set_format(handle, params, SND_PCM_FORMAT_S24_LE);
//			break ;
//	}
//	rc=snd_pcm_hw_params_set_channels(handle, params, channels); //设置声道,1表示单声>道，2表示立体声
//	if(rc<0)
//	{
//		perror("\nsnd_pcm_hw_params_set_channels:");
//		return 6;
//	}
//	val = frequency;
//	rc=snd_pcm_hw_params_set_rate_near(handle, params, &val, &dir); //设置>频率
//	if(rc<0)
//	{
//		perror("\nsnd_pcm_hw_params_set_rate_near:");
//		return 7;
//	}
//	rc = snd_pcm_hw_params(handle, params);
//	if(rc<0)
//	{
//		perror("\nsnd_pcm_hw_params: ");
//		return 8;
//	}
//	rc=snd_pcm_hw_params_get_period_size(params, &frames, &dir); /*获取周期长度*/
//	if(rc<0)
//	{
//		perror("\nsnd_pcm_hw_params_get_period_size:");
//		return 9;
//	}
//	size = frames * datablock; /*4 代表数据快长度*/
//	buffer = (char*)malloc(size);
//	fseek(fp,58,SEEK_SET); //定位歌曲到数据区
//	while (1)
//	{
//		memset(buffer,0,sizeof(size * sizeof(char*)));
//		ret = fread(buffer, 1, size, fp);
//		if(ret == 0)
//		{
//			printf("歌曲写入结束\n");
//			break;
//		}
//		 else if (ret != size)
//		{
//		 }
//		// 写音频数据到PCM设备
//		while((ret = snd_pcm_writei(handle, buffer, frames))<0)
//		{
//			 usleep(2000);
//			 if (ret == -EPIPE)
//			 {
//				  /* EPIPE means underrun */
//				  fprintf(stderr, "underrun occurred\n");
//				  //完成硬件参数设置，使设备准备好
//				  snd_pcm_prepare(handle);
//			 }
//			 else if (ret < 0)
//			 {
//				  fprintf(stderr, "error from writei: %s\n", snd_strerror(ret));
//			 }
//		}
//	}
//	snd_pcm_drop(handle);
//	snd_pcm_drain(handle);
//	snd_pcm_close(handle);
//	free(buffer);
//	return DEV_OK;
//}
