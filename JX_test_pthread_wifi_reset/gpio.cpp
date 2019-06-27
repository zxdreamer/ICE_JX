/*
 * gpio.cpp
 *
 *  Created on: 2018年1月2日
 *      Author: zh
 */
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <unistd.h>
#include <fcntl.h>  //define O_WRONLY and O_RDONLY
#include "gpio.hpp"
#define MSG(args...) printf(args)

int gpio_export(int pin) {
	char buffer[64];
	int len;
	int fd;

	fd = open("/sys/class/gpio/export", O_WRONLY);
	if (fd < 0) {
		MSG("Failed to open export for writing!\n");
		return (-1);
	}

	len = snprintf(buffer, sizeof(buffer), "%d", pin);
	if (write(fd, buffer, len) < 0) {
		MSG("Failed to export gpio!");
		return -1;
	}
	close(fd);
	return 0;
}

int gpio_unexport(int pin) {
	char buffer[64];
	int len;
	int fd;
	fd = open("/sys/class/gpio/unexport", O_WRONLY);
	if (fd < 0) {
		MSG("Failed to open unexport for writing!\n");
		return -1;
	}
	len = snprintf(buffer, sizeof(buffer), "%d", pin);
	if (write(fd, buffer, len) < 0) {
		MSG("Failed to unexport gpio!");
		return -1;
	}

	close(fd);
	return 0;
}

//dir: 0-->IN, 1-->OUT
int gpio_direction(int pin, int dir) {
	const char dir_str[] = "in\0out";
	char path[64];
	int fd;

	snprintf(path, sizeof(path), "/sys/class/gpio/gpio%d/direction", pin);
	fd = open(path, O_WRONLY);
	if (fd < 0) {
		MSG("Failed to open gpio direction for writing!\n");
		return -1;
	}

	if (write(fd, &dir_str[dir == 0 ? 0 : 3], dir == 0 ? 2 : 3) < 0) {
		MSG("Failed to set direction!\n");
		return -1;
	}

	close(fd);
	return 0;
}

//value: 0-->LOW, 1-->HIGH
int gpio_write(int pin, int value) {
	const char values_str[] = "01";
	char path[64];
	int fd;

	snprintf(path, sizeof(path), "/sys/class/gpio/gpio%d/value", pin);
	fd = open(path, O_WRONLY);
	if (fd < 0) {
		MSG("Failed to open gpio value for writing!\n");
		return -1;
	}

	if (write(fd, &values_str[value == 0 ? 0 : 1], 1) < 0) {
		MSG("Failed to write value!\n");
		return -1;
	}

	close(fd);
	return 0;
}

int gpio_read(int pin) {
	char path[64];
	char value_str[3];
	int fd;

	snprintf(path, sizeof(path), "/sys/class/gpio/gpio%d/value", pin);
	fd = open(path, O_RDONLY);
	if (fd < 0) {
		MSG("Failed to open gpio value for reading!\n");
		return -1;
	}

	if (read(fd, value_str, 3) < 0) {
		MSG("Failed to read value!\n");
		return -1;
	}

	close(fd);
	return (atoi(value_str));
}

// none表示引脚为输入，不是中断引脚
// rising表示引脚为中断输入，上升沿触发
// falling表示引脚为中断输入，下降沿触发
// both表示引脚为中断输入，边沿触发
// 0-->none, 1-->rising, 2-->falling, 3-->both
int gpio_edge(int pin, int edge) {
	const char dir_str[] = "none\0rising\0falling\0both";
	char ptr;
	char path[64];
	int fd;
	switch (edge) {
	case 0:
		ptr = 0;
		break;
	case 1:
		ptr = 5;
		break;
	case 2:
		ptr = 12;
		break;
	case 3:
		ptr = 20;
		break;
	default:
		ptr = 0;
	}

	snprintf(path, sizeof(path), "/sys/class/gpio/gpio%d/edge", pin);
	fd = open(path, O_WRONLY);
	if (fd < 0) {
		MSG("Failed to open gpio edge for writing!\n");
		return -1;
	}

	if (write(fd, &dir_str[ptr], strlen(&dir_str[ptr])) < 0) {
		MSG("Failed to set edge!\n");
		return -1;
	}

	close(fd);
	return 0;
}
void run_led(int val)
{
//    gpio_export(GPIO_C8);
//    gpio_direction(GPIO_C8,MODE_OUT);				//GPIOB30初始化
//    gpio_write(GPIO_C8, !val);
//    gpio_unexport(GPIO_C8);
}
void link_led(int val)
{
    gpio_export(GPIO_C8);
    gpio_direction(GPIO_C8,MODE_OUT);				//GPIOB30初始化
    gpio_write(GPIO_C8, val);
    gpio_unexport(GPIO_C8);

//    gpio_export(GPIO_B31);
//    gpio_direction(GPIO_B31,MODE_OUT);
//    gpio_write(GPIO_B31, !val);
//    gpio_unexport(GPIO_B31);
}
void radio_led(int val)
{
//    gpio_export(GPIO_B29);
//    gpio_direction(GPIO_B29,MODE_OUT);
//    gpio_write(GPIO_B29, val);
//    gpio_unexport(GPIO_B29);
    gpio_export(GPIO_C4);
    gpio_direction(GPIO_C4,MODE_OUT);				//GPIOB30初始化
    gpio_write(GPIO_C4, val);
    gpio_unexport(GPIO_C4);
}
void res_led(int val)
{
    gpio_export(GPIO_B31);
    gpio_direction(GPIO_B31,MODE_OUT);
    gpio_write(GPIO_B31, val);
    gpio_unexport(GPIO_B31);
}
void cam_led(int val)
{
    gpio_export(GPIO_C28);
    gpio_direction(GPIO_C28,MODE_OUT);
    gpio_write(GPIO_C28, !val);                 //0 1
    gpio_unexport(GPIO_C28);
}
