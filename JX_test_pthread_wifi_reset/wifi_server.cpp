/*
 * wifi_server.cpp
 *
 *  Created on: 2018年1月16日
 *      Author: zh
 */
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/un.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <unistd.h>
#include "XJ.hpp"
#include "TCP_Client.hpp"
#include "AFX.hpp"
//u8 wifi_set_flg=0;
//u8 flg_gate =0;
u8  r_buf[128];
u8  s_buf[128];
u8  wifi_id[50];
u8 ID_card=0;
u16 port = 8000;
u8 load_ask[10] = {0xaa,0xdd,0x07,0x53,0x01,0x88,0xee};
u8 check_ask[10] = {0xaa,0xdd,0x07,0x54,0x88,0xee};
const char* ip = "10.5.5.1";
void server_poll(int accept_fd);
void *wifi_ap(void *arg)
{
      //创建套接字,即创建socket
      int ser_sock = socket(AF_INET, SOCK_STREAM, 0);
      if(ser_sock < 0)
      {
         //创建失败
          perror("socket");
      }
      //绑定信息，即命名socket
      struct sockaddr_in addr;
      addr.sin_family = AF_INET;
      addr.sin_port = htons(port);
      //inet_addr函数将用点分十进制字符串表示的IPv4地址转化为用网络
      //字节序整数表示的IPv4地址
      addr.sin_addr.s_addr = inet_addr(ip);
      if(bind(ser_sock, (struct sockaddr*)&addr, sizeof(addr)) < 0)
      {
           perror("bind");
      }
      //监听socket
      int listen_sock = listen(ser_sock, 5);
      if(listen_sock < 0)
      {
          //监听失败
          perror("listen");
      }
      struct sockaddr_in peer;
      socklen_t peer_len;
      while(1)
      {
    	  peer_len = sizeof(peer_len);
          int accept_fd = accept(ser_sock, (struct sockaddr*)&peer, &peer_len);
          if(accept_fd < 0)
          {
              perror("accept");
          }
          else
          {
        	  server_poll(accept_fd);
             //printf("connected with ip: %s  and port: %d\n", inet_ntop(AF_INET,&peer.sin_addr, buf, 1024), ntohs(peer.sin_port));
          }
      }
}

int Server_Send(int accept_fd,unsigned char*data,int len)
{
	if(accept_fd > 0)
	{
		if( (send(accept_fd,data,len,MSG_DONTWAIT)) <= 0 )
		{
			//close(socket_fd);
			//printf("Client %s:%d is left\n",(char*) inet_ntoa(remote_addr.sin_addr),remote_addr.sin_port);
			return -1;
		}
    	delay_ms(100);
	}
	else
		printf("tcp not link!\n");
	return 0;
}
void server_poll(int accept_fd)
{
	while(1)
	{
		memset(r_buf, 0, sizeof(r_buf));
		ssize_t size = recv(accept_fd,r_buf,sizeof(r_buf),MSG_DONTWAIT);
		if(size > 0)
		{
			if(r_buf[0]==0xaa &&r_buf[1]==0xdd&&r_buf[3]==0x53)
			{
				printf("Received load\n");
				for(int i=0;i<30;i++)
					printf("%x ",r_buf[i]);
				printf("\n");
				Server_Send(accept_fd,load_ask,7);
			}
			if(r_buf[0]==0xaa &&r_buf[1]==0xdd&&r_buf[3]==0x54)
			{
				printf("Received check\n");
				for(int i=0;i<30;i++)
				{
					printf("%x ",r_buf[i]);
					wifi_id[i] = r_buf[i];
				}
				printf("\n");
				ID_card = 1;
				Server_Send(accept_fd,check_ask,6);
			}
			if(r_buf[0]==0xaa &&r_buf[1]==0xdd&&r_buf[3]==0x51)
			{
				printf("Received wifi chek\n");
				s_buf[0]=0xaa;
				s_buf[1]=0xdd;
				s_buf[2]=36;
				s_buf[3]=0x51;
				s_buf[4]=xj_data.temph;
				s_buf[5]=xj_data.templ;
				s_buf[6]=-1;
				s_buf[7]=1;
				s_buf[8]=xj_data.huin;
				s_buf[9]= xj_data.accx;
				s_buf[10]= xj_data.accy;
				s_buf[11]=flg_gate;
				s_buf[12]=xj_arg.local_ip[0];
				s_buf[13]=xj_arg.local_ip[1];
				s_buf[14]=xj_arg.local_ip[2];
				s_buf[15]=xj_arg.local_ip[3];
				s_buf[16]=xj_arg.mask[0];
				s_buf[17]=xj_arg.mask[1];
				s_buf[18]=xj_arg.mask[2];
				s_buf[19]=xj_arg.mask[3];
				s_buf[20]=xj_arg.getway[0];
				s_buf[21]=xj_arg.getway[1];
				s_buf[22]=xj_arg.getway[2];
				s_buf[23]=xj_arg.getway[3];
				s_buf[24]=xj_arg.server_ip[0];
				s_buf[25]=xj_arg.server_ip[1];
				s_buf[26]=xj_arg.server_ip[2];
				s_buf[27]=xj_arg.server_ip[3];
				s_buf[28]=xj_arg.server_port/256;
				s_buf[29]=xj_arg.server_port%256;
				s_buf[30]=0;
				s_buf[31]=0;
				s_buf[32]=0;
				s_buf[33]=0;
				s_buf[34]=0x88;
				s_buf[35]=0xee;
				Server_Send(accept_fd,s_buf,36);
			}
			if((u8)r_buf[0]==0xaa && (u8)r_buf[1]== 0xdd&&(u8)r_buf[3]==0x52)
			{
				printf("Received wifi set\n");
				memcpy(xj_arg.local_ip,&r_buf[4],4);
				memcpy(xj_arg.mask,&r_buf[8],4);
				memcpy(xj_arg.getway,&r_buf[12],4);
				memcpy(xj_arg.server_ip,&r_buf[16],4);
				xj_arg.server_port = (u8)r_buf[20]*256 + (u8)r_buf[21];
				disp_config(xj_arg);
				down_config(xj_arg);
				set_config(xj_arg,"eth0");
				wifi_set_flg = 1;
				s_buf[0]=0xaa;
				s_buf[1]=0xdd;
				s_buf[2]=36;
				s_buf[3]=0x52;
				s_buf[4]=0x88;
    			s_buf[5]=0xee;
    			Server_Send(accept_fd,s_buf,6);
			}
		}
		if(size == 0)
		{
			printf("phone is close connect...\n");
			close(accept_fd);
			break;
		}
	}
}

