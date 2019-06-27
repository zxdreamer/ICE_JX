/*
 * Uart0.hpp
 *
 *  Created on: 2017年12月18日
 *      Author: zh
 */

#ifndef UART0_HPP_
#define UART0_HPP_

#include "XJ.hpp"
int UART0_Open(char* port);
int UART0_Set (int fd,int speed,int flow_ctrl,int databits,int stopbits,int parity);
int UART0_Recv(int fd, char *rcv_buf,int data_len);
int UART0_Send(int fd, char *send_buf,int data_len);
void UART0_Close(int fd);
void collect_data2(XJ_Data &data);          //采集单片机数据
int collect_reset(char RX485[],char TX485[]);
#endif /* UART_HPP_ */
