/*
 * pcm.hpp
 *
 *  Created on: 2017年12月22日
 *      Author: zh
 */

#ifndef PCM_HPP_
#define PCM_HPP_

#include <alsa/asoundlib.h>

struct WAV_HEADER
{
    char rld[4]; //riff 标志符号
    int rLen;
    char wld[4]; //格式类型（wave）
    char fld[4]; //"fmt"

    int fLen; //sizeof(wave format matex)

    short wFormatTag; //编码格式
    short wChannels; //声道数
    int nSamplesPersec ; //采样频率
    int nAvgBitsPerSample;//WAVE文件采样大小
    short wBlockAlign; //块对齐
    short wBitsPerSample; //WAVE文件采样大小

    char dld[4]; //”data“
    int wSampleLength; //音频数据的大小

};

void * open_capture(void *arg);
void * open_play(void * arg);
//int test_open_play(snd_pcm_t *handle,snd_pcm_hw_params_t *params);
void* test_open_play(void* pcm);
//void *thrf1(void *);
void* test_open_capture(void* pcm);
#endif /* PCM_HPP_ */
