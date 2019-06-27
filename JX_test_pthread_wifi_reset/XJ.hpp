/*
 * XJ.hpp
 *
 *  Created on: 2017年12月20日
 *      Author: zh
 *      Modifier :张旭东
 */

#ifndef XJ_HPP_
#define XJ_HPP_
#include "AFX.hpp"
using namespace std;
struct XJ_Arg
{
	u32 xj_id;

	u8 local_ip[4];
	u8 mask[4];
	u8 getway[4];
	u8 server_ip[4];

	u16 server_port;
	u8 cyc_send;              //1-60min
	u8 temp_max;			  //温度阈值   40-100摄氏度

	u8 humid_max;			  //湿度阈值   20-100%
};
struct XJ_Data
{
	u8 temph;
	u8 templ;
	u8 huin;
	u8 accx;
	u8 accy;
	u8 accz;

	u8 Reset_Dev[24];
	u8 key[6];
};
//extern XJ_Arg xj_arg;
//extern XJ_Data xj_data;
void delay_ms(int ms);
void WlnInit(XJ_Arg &arg);
void init_Arg(XJ_Arg &arg);
void down_config(XJ_Arg &arg);
void load_config(XJ_Arg &arg);
void disp_config(XJ_Arg &arg);
void set_config(XJ_Arg &arg,char *devname);
void IptoString(char *ip_str,u8 *ip);
int SetIfAddr(char *ifname, char *Ipaddr, char *mask,char *gateway);
#endif /* XJ_HPP_ */
