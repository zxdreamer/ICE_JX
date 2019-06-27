/*
 * TCP_Client.hpp
 *
 *  Created on: 2016年12月23日
 *      Author: root
 */

#ifndef TCP_CLIENT_HPP_
#define TCP_CLIENT_HPP_
#include <unistd.h>
#include <stdlib.h>
#include <string.h>
#include <iostream>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <stdio.h>
#include <string>
#include <fcntl.h>
#include <sys/types.h>
#include "XJ.hpp"
#define MAX_SOCK_FD FD_SETSIZE

int tcp_poll();
void *tcp_client(void *arg );
int TCP_Send(u8 *data,u16 len);
void recv_testdeal(u8 MSG);
void recv_deal(u8 type);

#endif /* TCP_CLIENT_HPP_ */
