/*
 * test.h
 *
 *  Created on: Jun 12, 2018
 *      Author: xz
 */

#ifndef TEST_H_
#define TEST_H_

#include <sys/types.h>
#include <sys/stat.h>
#include <alsa/asoundlib.h>
#include <stdio.h>
#include <fcntl.h>
#include <unistd.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <error.h>

typedef unsigned char u8;
typedef unsigned short u16;

#define MAXSIZE 1024

//extern u8 ret_camera;
//extern u8 ret_audio;
//extern u8 ret_wifi;
//extern u8 ret_sensor1;
//extern u8 ret_sensor2;
//extern u8 ret_MPU;

//struct WAV_HEADER
//{
//    char rld[4]; //riff 标志符号
//    int rLen;
//    char wld[4]; //格式类型（wave）
//    char fld[4]; //"fmt"
//
//    int fLen; //sizeof(wave format matex)
//
//    short wFormatTag; //编码格式
//    short wChannels; //声道数
//    int nSamplesPersec ; //采样频率
//    int nAvgBitsPerSample;//WAVE文件采样大小
//    short wBlockAlign; //块对齐
//    short wBitsPerSample; //WAVE文件采样大小
//
//    char dld[4]; //”data“
//    int wSampleLength; //音频数据的大小
//
//};

int test_camera();
int test_audio();
int test_sensor1();
int test_sensor2();
int test_MPU();
int test_wifi();
int test_open_capture();
int test_open_play();

#endif /* TEST_H_ */
