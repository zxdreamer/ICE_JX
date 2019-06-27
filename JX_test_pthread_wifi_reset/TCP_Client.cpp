/*
 * TCP_Client.cpp
 *
 *  Created on: 2016年12月23日
 *      Author: zh
 *      Modifier :张旭东
 */
#include "TCP_Client.hpp"
#include "fa_cam202.hpp"
#include <string.h>
#include "gpio.hpp"
#include "AFX.hpp"
#include "test.hpp"
#include "Uart0.hpp"
#include "wifi_confl.h"

//测试标识符
u8 Ftest_net      =  Flag_OFF;
u8 Ftest_camera   =  Flag_OFF;
u8 Ftest_audio    =  Flag_OFF;
u8 Ftest_wifi     =  Flag_OFF;
u8 Ftest_sensor1  =  Flag_OFF;
u8 Ftest_sensor2  =  Flag_OFF;
u8 Ftest_MPU      =  Flag_OFF;

u8 answer_net[10]     =  {0xaa, 0xbb, 0x00, 0x01, 0x88, 0xff};   //测试指令网络
u8 answer_camera[10]  =  {0xaa, 0xbb, 0x01, 0x01, 0x88, 0xff};   //测试指令摄像头
u8 answer_audio[10]   =  {0xaa, 0xbb, 0x02, 0x01, 0x88, 0xff};   //测试指令语音模块
u8 answer_wifi[10]    =  {0xaa, 0xbb, 0x03, 0x01, 0x88, 0xff};   //测试指令wifi模块
u8 answer_sensor1[10] =  {0xaa, 0xbb, 0x04, 0x01, 0x88, 0xff};   //测试指令温度传感器1
u8 answer_sensor2[10] =  {0xaa, 0xbb, 0x05, 0x01, 0x88, 0xff};   //测试指令温度传感器2
u8 answer_MPU[10]     =  {0xaa, 0xbb, 0x06, 0x01, 0x88, 0xff};   //测试指令MPU6050

u8 heart_beat[10]     =  {0xaa,0xdd,0x03,0x37,0x88,0xee};        //心跳包
u8 radio_request[10]  =  {0xaa,0xdd,0x03,0x22,0x88,0xee};        //请求语音包
u8 radio_quit[10]     =  {0xaa,0xdd,0x03,0x23,0x88,0xee};        //请求断开

//char send_buf[MAXSIZE]={0};
u8 radio_pass   = Flag_OFF;                            //语音通信建立
u8 radio_upass  = Flag_OFF;                            //断开或者忙
u8 wifi_set_flg = Flag_OFF;
u8 flg_heart    = Flag_OFF;                            //心跳标志
u8 count_heart  = Flag_OFF;                            //错过心跳计数
u8 flg_gate     = Flag_OFF;                            //箱门状态
u8 flg_picture  = Flag_OFF;                            //拍照标志
u8 flg_report   = Flag_OFF;                            //上传
u8 check_msg    = Flag_OFF;                            //上位机查询数据标志
u8 reset_flg    = Flag_OFF;                            //远程复位
u8 reset_jx     = Flag_OFF;
u8 UpRepTim		= 10;
u8 wifi_password = 0;
u8 wifi_name = 0;
u8 wifi_password_len = 0;
u8 wifi_name_len = 0;
char Reset_RS485[20] = {0xaa,0xdd,0x02,0x38,0x00,0x00,0x00,0x88,0xee};
char Back_RS485[20]  = {0xaa,0xdd,0x02,0x38,0x00,0x00,0x00,0x88,0xee};
char Back_ResetJX[20] = {0xaa,0xdd,0x02,0x31,0x01,0x01,0x00,0x88,0xee};
char send_buf[TCP_BUF_MAXSIZE]={0};
char recv_buf[TCP_BUF_MAXSIZE];
char remote_ip[20] = "192.168.1.100";                 //服务器字符串地址
int socket_fd = -1;
sockaddr_in remote_addr;
char newssid[60] = "ZNJX_ice";
char newpass[60] = "123456789";
void *tcp_client( void *arg )                            //客户端线程
{
	while(1)
	{
		delay_ms(1000);
		IptoString(remote_ip,xj_arg.server_ip);          //ip数字转字符串
		if((socket_fd = socket(PF_INET,SOCK_STREAM,IPPROTO_TCP))<0)   //sockt 客户端编程
		{
			throw "socket() failed";
		}
		memset(&remote_addr,0,sizeof(remote_addr));
		remote_addr.sin_family = AF_INET;
		remote_addr.sin_port = htons(xj_arg.server_port);
		remote_addr.sin_addr.s_addr = inet_addr(remote_ip);
		if(connect(socket_fd,(struct sockaddr *)&remote_addr,sizeof(struct sockaddr))<0)//time out 20s
		{
			//add error operate cade
			perror("connect");
			close(socket_fd);
		}
		else
		{
			cout<<"link server is succeed!"<<endl;
			link_led(ON);
			tcp_poll();                            //客户端循环
			link_led(OFF);
		}
	}
}
void recv_deal(u8 type)
{
//	static int redio_flag=0;
	switch(type)
	{
		case CMD_COUNT_HEART:
			printf("Received heart\n");
			count_heart = 0;
		break;
		case CMD_SOUND_FIRST:
			printf("recv_buf[3,4] %d,%d\n",recv_buf[3],recv_buf[4]);
			if((u8)recv_buf[4]==0x01)
			{
				printf("Radio pass\n");
				radio_open = 0;
				radio_pass = 1;
				radio_led(ON);
			}
			else
			{
				printf("Radio busy\n");
				radio_pass = 0;
				for(int l = 0;l<10;l++)
				{
					radio_led(ON);
					usleep(10);
					radio_led(OFF);
					usleep(10);
				}
			}
		break;
		case CMD_CHECK_MSG:
			printf("Received check\n");
			check_msg = 1;
		break;
		case CMD_CHECK_TIM:
			printf("change MSG TIM\n");
			UpRepTim = recv_buf[4]*100;
			UpRepTim += recv_buf[5]*10;
			UpRepTim += recv_buf[6];
		break;
		case CMD_RESET_DEV:
			printf("remote reset other Dev\n");
			reset_flg = 1;
		break;
		case CMD_RESET_JX:
			printf("remote restart JX\n");
			reset_jx = 1;
		break;
		case CMD_WIFI_NAME:
			printf("chang wifi name\n");
			wifi_name = recv_buf[2];
			wifi_password = recv_buf[4 + wifi_name];
			printf("wifi name length :%d\n",wifi_name);
			printf("wifi name length :%d\n",wifi_password);
		break;
//		case CMD_WIFI_PASSWORD:
//			 printf("chang wifi password\n");
//			 wifi_password = recv_buf[2];
//			 printf("wifi password length:%d\n",wifi_password);
//		break;
		default:break;
	}
}
void recv_testdeal(u8 MSG)
{
	if(MSG == 0x01)
	{
		printf("receive test\n");
		if((u8)recv_buf[2]==0)
		{
			printf("test net...\n");
			Ftest_net=Flag_ON;
		}
		else if((u8)recv_buf[2]==1)
		{
			printf("test camera...\n");
			Ftest_camera=Flag_ON;
		}
		else if((u8)recv_buf[2]==2)
		{
			printf("test audio...\n");
			Ftest_audio=Flag_ON;
		}
		else if((u8)recv_buf[2]==3)
		{
			printf("test wifi...\n");
			Ftest_wifi=Flag_ON;
		}
		else if((u8)recv_buf[2]==4)
		{
			printf("test sensor1...\n");
			Ftest_sensor1=Flag_ON;
		}
		else if((u8)recv_buf[2]==5)
		{
			printf("test sensor2...\n");
			Ftest_sensor2=Flag_ON;
		}
		else if((u8)recv_buf[2]==6)
		{
			printf("test MPU6050...\n");
			Ftest_MPU=Flag_ON;
		}
	}
}
int tcp_poll()
{
	while(1)
	{
		int rec = 0;
		memset(recv_buf,0,TCP_BUF_MAXSIZE);
		rec = recv(socket_fd,recv_buf,TCP_BUF_MAXSIZE,MSG_DONTWAIT);
		if(rec == 0)   //检测服务器主动断开,recv的错误处理。
		{
			close(socket_fd);
			radio_pass = 0;
			printf("Server %s:%d is OFF\n",(char*) inet_ntoa(remote_addr.sin_addr),remote_addr.sin_port);
			break;
		}
		else if(rec > 0)
		{
			if((u8)recv_buf[0]==0xaa &&(u8)recv_buf[1]==0xdd)
			{
				recv_deal(recv_buf[3]);
				if((u8)recv_buf[3]==0x36)
				{
					printf("Received set\n");
					memcpy(xj_arg.local_ip,&recv_buf[4],4);
					memcpy(xj_arg.mask,&recv_buf[8],4);
					memcpy(xj_arg.getway,&recv_buf[12],4);
					memcpy(xj_arg.server_ip,&recv_buf[16],4);
					xj_arg.server_port = (u8)recv_buf[20]*256 + (u8)recv_buf[21];
					close(socket_fd);
					radio_pass = 0;
					down_config(xj_arg);
					set_config(xj_arg,"eth0");
					break;
				}
			}
			else if((u8)recv_buf[0]==0xaa &&(u8)recv_buf[1]==0xbb)
			{
				recv_testdeal(recv_buf[3]);
			}
		}
		//send
	    if(flg_heart)
	    {
	    	TCP_Send(heart_beat,sizeof(heart_beat));
	    	flg_heart=0;
	    	count_heart++;
	    	if(count_heart==10)
	    	{
	    		count_heart = 0;
	    		close(socket_fd);
	    		radio_pass = 0;
	    		printf("net disconnect\n");
	    		break;
	    	}
	    }
	    if(flg_report == 1)
	    {
	    	send_buf[0]=0xaa;
			send_buf[1]=0xdd;
			send_buf[2]=33;
			send_buf[3]=0x33;
			send_buf[4]=  xj_data.temph;
			send_buf[5]=  xj_data.templ;
			send_buf[6]=0;
			send_buf[7]=0;
			send_buf[8]=  xj_data.huin;
			send_buf[9]=  xj_data.accx;
			send_buf[10]= xj_data.accy;
			send_buf[11]= xj_data.accz;
			send_buf[12]=0;
			send_buf[13]= flg_gate;
			send_buf[14]=0x88;
			send_buf[15]=0xee;
			TCP_Send((u8*)send_buf,16);
			flg_report= 0;
	    }
	    if(check_msg)
	    {
	    	check_msg = 0;
		    send_buf[0]=0xaa;
			send_buf[1]=0xdd;
			send_buf[2]=33;
			send_buf[3]=0x32;
			send_buf[4]=xj_data.temph;
			send_buf[5]=xj_data.templ;
			send_buf[6]=-1;
			send_buf[7]=1;
			send_buf[8]=xj_data.huin;
			send_buf[9]= xj_data.accx;
			send_buf[10]= xj_data.accy;
			send_buf[11]= xj_data.accz;
			send_buf[12]=0;
			send_buf[13]=flg_gate;
			send_buf[14]=xj_arg.local_ip[0];
			send_buf[15]=xj_arg.local_ip[1];
			send_buf[16]=xj_arg.local_ip[2];
			send_buf[17]=xj_arg.local_ip[3];
			send_buf[18]=xj_arg.mask[0];
			send_buf[19]=xj_arg.mask[1];
			send_buf[20]=xj_arg.mask[2];
			send_buf[21]=xj_arg.mask[3];
			send_buf[22]=xj_arg.getway[0];
			send_buf[23]=xj_arg.getway[1];
			send_buf[24]=xj_arg.getway[2];
			send_buf[25]=xj_arg.getway[3];
			send_buf[26]=xj_arg.server_ip[0];
			send_buf[27]=xj_arg.server_ip[1];
			send_buf[28]=xj_arg.server_ip[2];
			send_buf[29]=xj_arg.server_ip[3];
			send_buf[30]=xj_arg.server_port/256;
			send_buf[31]=xj_arg.server_port%256;
			send_buf[32]=0;
			send_buf[33]=0;
			send_buf[34]=0;
			send_buf[35]=10/256;
			send_buf[36]=10%256;
			send_buf[37]=0x88;
			send_buf[38]=0xee;
			TCP_Send((u8*)send_buf,40);
	    }
	    if(radio_open == 1 && flg_gate == 0)  //箱门打开且语音对讲按键按下
	    {
	    	TCP_Send((u8*)radio_request,sizeof(radio_request));
//	    	radio_open = 0;
	    }
	    if(radio_upass == 1 )
	    {
	    	radio_led(OFF);
	    	TCP_Send((u8*)radio_quit,sizeof(radio_quit));
	    	radio_upass = 0;
	    }
	    if(flg_picture == 1 && flg_gate == 0)  //箱门打开且定时10s
	    {
//	    	printf(".........start capture picture..........\n");
	    	cam_led(ON);
	    	get_picture();
	    	cam_led(OFF);
	    	flg_picture=0;
//	    	printf(".............end picture................\n");
	    }
	    if(wifi_set_flg==1)
	    {
	    	wifi_set_flg = 0;
	    	close(socket_fd);
			radio_pass = 0;
			delay_ms(100);
			break;
	    }
	    if(reset_flg == 1)
	    {
	    	Reset_RS485[4] = recv_buf[4];
	    	Reset_RS485[5] = recv_buf[5];
	    	Reset_RS485[6] = recv_buf[6];
			collect_reset(Back_RS485,Reset_RS485);
			printf("Reset_RS485 data\n");
			for(int i=0;i<9;i++)
			{
				printf("%x ",Back_RS485[i]);
			}
			TCP_Send((u8*)Back_RS485,10);
	    	reset_flg = 0;
	    }
	    if(reset_jx == 1)               //机箱重启，通过管道调用shell命令
	    {
	    	reset_jx = 0;
	    	FILE *ptr = NULL;
	    	ptr = popen("reboot","r");
	    	if(ptr != NULL)           //机箱重启成功
	    	{
	    		TCP_Send((u8*)Back_ResetJX,20);
	    		printf("jx restart successful\n");
	    	}
	    	else
	    	{
	    		Back_ResetJX[5] = 0x00;
	    		TCP_Send((u8*)Back_ResetJX,20);
	    		printf("jx restart failed\n");
	    	}
	    	pclose(ptr);
	    }
	    if(ID_card == 1)
	    {
	    	wifi_id[3] = 0x41;
	    	TCP_Send((u8 *)wifi_id,30);
	    	ID_card = 0;
	    	memset(wifi_id,0,sizeof(wifi_id));
	    }

	    if(wifi_name != 0 && wifi_password != 0)
	    {
	    	for(int i = 0;i<6+wifi_name+1+wifi_password;i++)
	    	{
	    		printf("%d ",recv_buf[i]);
	    		printf("\n");
	    	}
	    	memset(newssid,'\0',sizeof(newssid));
	    	memset(newpass,'\0',sizeof(newpass));
	    	memmove(newssid,&recv_buf[4],wifi_name);
	    	memmove(newpass,&recv_buf[4 + wifi_name + 1],wifi_password);
	    	wifi_set(newssid,newpass);
	    	wifi_name = 0;
	    	wifi_password = 0;
	    }
/******************************测试上报代码******************************/
	    if(Ftest_net == 1)           //测试网络通断
	    {
	    	Ftest_net = Flag_OFF;
			TCP_Send((u8 *)answer_net, 10);//能收到消息表明网络功能正常
	    }
	    if(Ftest_camera == 1)        //测试摄像头
	    {
			int ret;
			ret = test_camera();
			if (ret != DEV_OK)answer_camera[3] = 0x00;//若出错，更改返回数据包
			TCP_Send(answer_camera, 10);
			Ftest_camera = Flag_OFF;
	    }
	    if(Ftest_audio == 1)        //测试扬声器与麦克风
	    {
	    	audio_start_play = 1;
	    	audio_start_cap  = 1;
			int ret;
			int num;
//			pthread_mutex_lock(&test_mutex);
			if(audio_end_play == 1 && audio_end_cap == 1)
			{
				audio_end_play  = 0;
				audio_end_cap   = 0;
				ret = test_audio();
//				pthread_mutex_unlock(&test_mutex);
				printf("audio ret test_audio %d\n",ret);
				answer_audio[3] = ret;
				TCP_Send(answer_audio, 10);
				Ftest_audio = Flag_OFF;
				audio_start_cap = 0;
				audio_start_play = 0;
				printf(">>>>>>>>>>>>>>>>>>>>>>>>>Test audio and cap finished !!!>>>>>>>>>>>>>>>>>>>>!\n");
			}
	    }
	    if(Ftest_wifi == 1)        //测试wifi通断
	    {
			int ret;
			ret = test_wifi();
			if (ret == DEV_ERR)answer_wifi[3] = 0x00;
			TCP_Send(answer_wifi, 10);
			Ftest_wifi = Flag_OFF;
	    }
	    if(Ftest_sensor1 == 1)           //测试温度传感器1
	    {
			int ret;
			ret = test_sensor1();
			printf("sensor1 ret %d\n",ret);
			if (ret != DEV_OK)answer_sensor1[3] = 0x00;
			TCP_Send(answer_sensor1, 10);
			Ftest_sensor1 = Flag_OFF;
	    }
	    if(Ftest_sensor2 == 1)   //测试温度传感器2
	    {
			int ret;
			ret = test_sensor2();
			if (ret != DEV_OK)answer_sensor2[3] = 0x00;
			TCP_Send(answer_sensor2, 10);
	    	Ftest_sensor2 = Flag_OFF;
	    }
	    if(Ftest_MPU == 1)     //测试MPU6050
	    {
			int ret;
			ret = test_MPU();
			if (ret != DEV_OK)answer_MPU[3] = 0x00;
			TCP_Send(answer_MPU, 10);
			Ftest_MPU = Flag_OFF;
	    }
/************************************************************************/
	}
	return 0;
}

int TCP_Send(u8 *data,u16 len)
{
	if(socket_fd > 0)
	{
		if( (send(socket_fd,data,len,MSG_DONTWAIT)) <= 0 )
		{
			//close(socket_fd);
			printf("Client %s:%d is left\n",(char*) inet_ntoa(remote_addr.sin_addr),remote_addr.sin_port);
			return -1;
		}
    	delay_ms(100);
	}
	else
		printf("tcp not link!\n");
	return 0;
}


