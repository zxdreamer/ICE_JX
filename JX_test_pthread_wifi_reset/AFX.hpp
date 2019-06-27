/*
 * AFX.hpp
 *  声明项目中的全局变量，宏定义
 *  Created on: 2018年7月20日
 *      original Author: 张旭东
 */

#ifndef AFX_HPP_
#define AFX_HPP_
//#include <pthread.h>
//#include <semaphore.h>
#include "XJ.hpp"

typedef unsigned char u8;
typedef	unsigned short u16;
typedef	unsigned int u32;

#define Flag_ON     1            //某项操作开启
#define Flag_OFF    0            //某项操作关闭

//#define DEV_OK   0             //设备正常
#define DEV_OK   0
#define DEV_ERR -1               //设备出错

#define CMD_COUNT_HEART 0x37    //心跳包标识
#define CMD_SOUND_FIRST 0x81    //语音对讲标识符
#define CMD_CHECK_MSG   0X32    //查询上报标识符
#define CMD_CHECK_TIM   0x34    //查询时间设置
#define CMD_RESET_DEV   0x38    //复位指令码
//#define RESET_DEV_NUM(k)    Reset_dev[k] //复位设备号
#define CMD_TEST_SECOND 0x01    //测试指令标识符
#define CMD_RESET_JX    0x31    //复位机箱（nanopai）
#define CMD_WIFI_NAME   0x56    //修改wifi名字
#define CMD_WIFI_PASSWORD 0x55  //修改wifi名字
#define TCP_BUF_MAXSIZE 1024        //定义TCP接受1K

#define UART2_DEV  "/dev/ttySAC2"  //定义串口2设备名称
#define UART3_DEV  "/dev/ttySAC3"  //定义串口3设备名称

extern int audio_deal_play;     //测试语音播放处理位
extern int audio_deal_cap;      //测试语音捕获处理位
extern int audio_end_cap;       //测试语音捕获结束位
extern int audio_end_play;      //测试语音播放结束位
extern int audio_start_cap;     //测试语音捕获开始位
extern int audio_start_play;    //测试语音播放开始位

extern int camtest_flag;

extern u8 flg_heart;          //心跳包发送标志位
extern u8 flg_gate;			  //拍照(箱门)上传开始标志位
extern u8 flg_picture;        //拍照定时上传标志位
extern u8 wifi_set_flg;       //wifi打卡标志位
extern u8 flg_report;         //上报数据标志位
extern u8 radio_pass;         //语音对讲开启标志位
extern u8 radio_upass;        //语音对讲结束标志位
extern u8 radio_open;         //语音按键状态标志
extern u8 reset_flg;          //复位其它设备指令标志
extern u8 reset_jx;   		  //复位机箱(nanopai)
extern u8 UpRepTim;
extern u8 ID_card;

//extern u8 wifi_password;
//extern u8 wifi_name;
//extern 	char newssid[60];
//extern char newpass[60];

extern struct XJ_Arg xj_arg;
extern struct XJ_Data xj_data;

extern char send_buf[TCP_BUF_MAXSIZE];
extern char recv_buf[TCP_BUF_MAXSIZE];
extern char remote_ip[20];
extern u8  wifi_id[50];
//u8 Reset_dev[]={0x01,0x02,0x04,0x08,0x10,0x20,0x40,0x80};                //复位设备列表
//extern pthread_mutex_t test_mutex;
//extern sem_t test_sem;
#endif /* AFX_HPP_ */
