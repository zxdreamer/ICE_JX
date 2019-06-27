/*
 * gpio.hpp
 *
 *  Created on: 2018年1月2日
 *      Author: zh
 */

#ifndef GPIO_HPP_
#define GPIO_HPP_
//函数声明
#define GPIO_BASE_B 32
#define GPIO_BASE_C 64
#define MODE_OUT 1
#define MODE_IN  0
#define GPIO_B29   (GPIO_BASE_B + 29)
#define GPIO_B28   (GPIO_BASE_B + 28)
#define GPIO_B31   (GPIO_BASE_B + 31)
#define GPIO_B30   (GPIO_BASE_B + 30)
#define GPIO_C4    (GPIO_BASE_C + 4)
#define GPIO_C7    (GPIO_BASE_C + 7)
#define GPIO_C8    (GPIO_BASE_C + 8)
#define GPIO_C24   (GPIO_BASE_C + 24)
#define GPIO_C28   (GPIO_BASE_C + 28)
#define GPIO_B26   (GPIO_BASE_B + 26)
#define ON 0
#define OFF 1
int gpio_export(int pin);               //载入
int gpio_unexport(int pin);             //卸载
int gpio_direction(int pin, int dir);   //定义方向 I O
int gpio_write(int pin, int value);
int gpio_read(int pin);
void run_led(int val);
void link_led(int val);
void radio_led(int val);
void res_led(int val);
void cam_led(int val);
#endif /* GPIO_HPP_ */
