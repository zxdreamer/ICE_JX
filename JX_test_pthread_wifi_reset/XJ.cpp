/*
 * XJ.cpp
 *
 *  Created on: 2017年12月20日
 *      Author: zh
 *      Modifier :张旭东s
 */
#include "XJ.hpp"
#include <string.h>
#include <stdio.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <net/if.h>
#include <error.h>
#include<fcntl.h>
#include <net/route.h>
#include <unistd.h>
#include <stddef.h>
#include "AFX.hpp"
XJ_Arg xj_arg;
XJ_Data xj_data;
void delay_ms(int ms)
{
	usleep(1000*ms);
}
void IptoString(char *ip_str,u8 *ip)
{
	sprintf(ip_str,"%d.%d.%d.%d",ip[0],ip[1],ip[2],ip[3]);
}
void WlnInit(XJ_Arg &arg)
{
	arg.local_ip[0] = 192;
	arg.local_ip[1] = 168;
	arg.local_ip[2] = 1;
	arg.local_ip[3] = 199;
	arg.mask[0] = 255;
	arg.mask[1] = 255;
	arg.mask[2] = 255;
	arg.mask[3] = 0;
	arg.getway[0] = 192;
	arg.getway[1] = 168;
	arg.getway[2] = 1;
	arg.getway[3] = 1;
	arg.server_ip[0] = 192;
	arg.server_ip[1] = 168;
	arg.server_ip[2] = 1;
	arg.server_ip[3] = 100;
	arg.server_port = 8080;
}
void init_Arg(XJ_Arg &arg)            		  //回复出厂设置
{
	int fd;
	u8 readbuf = 0;
	fd = access("config",F_OK|R_OK);
	if(fd == 0)
	{
		printf("file exited\n");
		fd = open("config",O_RDWR,0777);    //所有用户可读写执行
		if(read(fd,&readbuf,1)==(ssize_t)NULL)         			//文件存在但为空
		{
			printf("file exited and is NULL\n");
			WlnInit(arg);
			write(fd,(u8*)&arg,sizeof(XJ_Arg));
		}
		close(fd);
		return;
	}
	else
	{
		printf("file unexited and need to create\n");
		WlnInit(arg);
		fd = open("config",O_RDWR|O_CREAT|O_TRUNC,0777);       //所有用户可读写执行
		write(fd,(u8*)&arg,sizeof(XJ_Arg));
		close(fd);
		return;
	}
}
void down_config(XJ_Arg &arg)       //下载配置保存
{
	int fd;
	fd = open("config",O_RDWR|O_CREAT,0777);    //所有用户可读写执行
	if(fd == -1)
	{
		perror("Can't Open Config file");
	}
	write(fd,(u8*)&arg,sizeof(XJ_Arg));
	close(fd);
}
void load_config(XJ_Arg &arg)        //载入配置
{
	int fd;
	fd = open("config",O_RDWR);
	if(fd == -1)
	{
		perror("Can't Open Config file");
	}
	read(fd,(u8*)&arg,sizeof(XJ_Arg));
	close(fd);
}
void disp_config(XJ_Arg &arg)
{
	printf("local_ip:%d.%d.%d.%d\n",arg.local_ip[0],arg.local_ip[1],arg.local_ip[2],arg.local_ip[3]);
	printf("mask:%d.%d.%d.%d\n",arg.mask[0],arg.mask[1],arg.mask[2],arg.mask[3]);
	printf("gateway:%d.%d.%d.%d\n",arg.getway[0],arg.getway[1],arg.getway[2],arg.getway[3]);
	printf("server_ip:%d.%d.%d.%d\n",arg.server_ip[0],arg.server_ip[1],arg.server_ip[2],arg.server_ip[3]);
	printf("server_port:%d\n",arg.server_port);
}
void set_config(XJ_Arg &arg,char *devname)
{
	char Ipaddr[20];
	char mask[20];
	char gateway[20];
	IptoString(Ipaddr,arg.local_ip);
	IptoString(mask,arg.mask);
	IptoString(gateway,arg.getway);
	SetIfAddr(devname, Ipaddr, mask,gateway);
}
int SetIfAddr(char *ifname, char *Ipaddr, char *mask,char *gateway)
{
    int fd;
    int rc;
    struct ifreq ifr;
    struct sockaddr_in *sin;
    struct rtentry  rt;
    fd = socket(AF_INET, SOCK_DGRAM, 0);
    if(fd < 0)
    {
            perror("socket   error");
            return -1;
    }
    memset(&ifr,0,sizeof(ifr));
    strcpy(ifr.ifr_name,ifname);
    sin = (struct sockaddr_in*)&ifr.ifr_addr;
    sin->sin_family = AF_INET;
    //ipaddr
    if(inet_aton(Ipaddr,&(sin->sin_addr)) < 0)
    {
        perror("inet_aton   error");
        return -2;
    }

    if(ioctl(fd,SIOCSIFADDR,&ifr) < 0)
    {
        perror("ioctl   SIOCSIFADDR   error");
        return -3;
    }
    //netmask
    if(inet_aton(mask,&(sin->sin_addr)) < 0)
    {
        perror("inet_pton   error");
        return -4;
    }
    if(ioctl(fd, SIOCSIFNETMASK, &ifr) < 0)
    {
        perror("ioctl");
        return -5;
    }
    //gateway
    memset(&rt, 0, sizeof(struct rtentry));
    memset(sin, 0, sizeof(struct sockaddr_in));
    sin->sin_family = AF_INET;
    sin->sin_port = 0;
    if(inet_aton(gateway, &sin->sin_addr)<0)
    {
       printf ( "inet_aton error\n" );
    }
    memcpy ( &rt.rt_gateway, sin, sizeof(struct sockaddr_in));
    ((struct sockaddr_in *)&rt.rt_dst)->sin_family=AF_INET;
    ((struct sockaddr_in *)&rt.rt_genmask)->sin_family=AF_INET;
    rt.rt_flags = RTF_GATEWAY;
    if (ioctl(fd, SIOCADDRT, &rt)<0)
    {
    	perror( "ioctl(SIOCADDRT) error in set_default_route\n");
        close(fd);
        return -1;
    }
    close(fd);
    return rc;
}

