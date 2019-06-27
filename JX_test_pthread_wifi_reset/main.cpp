/*
 * main.cpp
 *
 *  Created on: 2016年12月23日
 *      Author: zh
 *      Modifier :张旭东
 */
#include <pthread.h>
#include <signal.h>
#include <sys/time.h>
#include "XJ.hpp"
#include "pcm.hpp"
#include "gpio.hpp"
#include "Uart0.hpp"
#include "TCP_Client.hpp"
#include "AFX.hpp"
void *wifi_ap(void *arg);
uint64_t local_time = 0;//记录本地时间s
u8 gate_last = 0;		//判断上一次gate状态
u8 radio_down = 0;
u8 radio_open = 0;
//u8 flg_gate=0;
//pthread_mutex_t test_mutex;
//sem_t test_sem;

char Check_Sigdata[10] = {0xaa,0xdd,0x01,'t',0x88,0xee};

void setall_led(int val)
{
    gpio_export(GPIO_B29);              //led 1灭 0亮
    gpio_export(GPIO_B31);
    gpio_export(GPIO_C4);
    gpio_export(GPIO_C8);
    gpio_export(GPIO_C28);
    gpio_direction(GPIO_C28,MODE_OUT);
    gpio_direction(GPIO_C8,MODE_OUT);
    gpio_direction(GPIO_C4,MODE_OUT);
    gpio_direction(GPIO_B31,MODE_OUT);
    gpio_direction(GPIO_B29,MODE_OUT);
    gpio_write(GPIO_C28,!val);       //补光灯 0灭 1亮
    gpio_write(GPIO_C8, val);
    gpio_write(GPIO_C4, val);
    gpio_write(GPIO_B31, val);
    gpio_write(GPIO_B29, val);
    gpio_unexport(GPIO_B29);
    gpio_unexport(GPIO_B31);
    gpio_unexport(GPIO_C4);
    gpio_unexport(GPIO_C8);
    gpio_unexport(GPIO_C28);
}
void check_gate()									//gate IO检测函数
{
	int gate = 1;
	gate = gpio_read(GPIO_B28);                     //读取端口状态
	if(gate==0)
	{
		delay_ms(10);
		gate = gpio_read(GPIO_B28);
		if(gate==0&&gate_last==0)                   //已经按下不再管
		{
			flg_picture = 1;
			flg_gate    = 0;
			gate_last   = 1;
			printf("gate_open %d\n",gate);
		}
	}
	else											//抬起或没按下赋值
	{
		flg_gate  = 1;
		gate_last = 0;
	}
}
void check_radio()                                 //语音按钮检测
{
	int radio = 1;
	radio = gpio_read(GPIO_B26);
	if(radio==0)
	{
		delay_ms(10);
		radio = gpio_read(GPIO_B26);
		if(radio == 0 && radio_down == 0)
		{
			radio_open = 1;
			radio_down = 1;
			printf("radio_open %d\n",radio_open);
		}
	}
	else if(radio == 1)
	{
		delay_ms(10);
		radio = gpio_read(GPIO_B26);
		if(radio == 1 && radio_down == 1)
		{
			radio_upass = 1;
			radio_down = 0;
			radio_open = 0;
	//		radio_led(OFF);
			printf(">>>>>>>>>>>>>>>radio_close %d\n",radio_upass);
		}
	}
}

void time_func(int t)            //定时服务1s调用一次
{
	local_time++;
	run_led(local_time%2);
	if(local_time%5==0)          //5s一次心跳，可调
		flg_heart=1;
	if(local_time%UpRepTim==0)		//10s一次数据
		flg_report = 1;
	if(local_time%20==0&&gate_last==1)	//gate常开，20s一张图
		flg_picture=1;
}
void init_time()                  //配置定时器
{
	struct itimerval val;
	struct sigaction act;
	act.sa_handler = time_func; //设置处理信号的函数
	act.sa_flags  = 0;
	sigemptyset(&act.sa_mask);
	sigaction(SIGPROF, &act, NULL);//时间到发送SIGROF信号
	val.it_value.tv_sec = 2; //
	val.it_value.tv_usec = 0;
	val.it_interval = val.it_value; //定时器间隔为1s
	setitimer(ITIMER_PROF, &val, NULL);
}

int main()
{
	pthread_t p_client,p_capture,p_aplay,p_wifi;
//	char* uart2_dev="/dev/ttySAC2";
	setall_led(OFF);                     //关灯
	init_Arg(xj_arg);											     //初始化参数，出厂参数运行一次可以注释掉
	load_config(xj_arg);                                             //载入参数
	disp_config(xj_arg);											 //打印参数，不需要时可注释掉
	init_time();                                                  	 //启动定时器
	set_config(xj_arg,"eth0");										 //设置参数，调用ioctl改ip mask gateway
//	pthread_mutex_init(&test_mutex,NULL);                            // 互斥锁初始化
//	sem_init(&test_sem,0,0);
    if( pthread_create( &p_client, NULL, tcp_client, NULL) != 0 )    //创建网络线程
    {
        printf( "Create thread error!\n");
        return -1;
    }
    if( pthread_create( &p_capture, NULL, open_capture, NULL) != 0 )  //语音捕获线程
    {
        printf( "Create thread error!\n");
        return -1;
    }
	 if( pthread_create( &p_aplay, NULL, open_play, NULL) != 0 )     //语音播放线程
	 {
		  printf( "Create thread eextern u8 radio_open;rror!\n");
		  return -1;
	  }
	  if( pthread_create( &p_wifi, NULL, wifi_ap, NULL) != 0 )     //wifi配置
	  {
		  printf( "Create thread extern u8 radio_open;rror!\n");
		  return -1;
	  }
    printf( "This is the main process.\n" );                         //采集主程序
    gpio_export(GPIO_B26);
    gpio_direction(GPIO_B26,MODE_IN);								//语音打开
    gpio_export(GPIO_B28);
    gpio_direction(GPIO_B28,MODE_IN);								//箱门打开
    while(1)
    {
    	check_gate();                                                //箱门
    	check_radio();												 //语音按钮
    	collect_data2(xj_data);									     //单片机数据采集
    	sleep(1);
    }
//    link_led(OFF);
//    radio_led(OFF);
//    cam_led(OFF);
//    res_led(OFF);
	return 0;
}


