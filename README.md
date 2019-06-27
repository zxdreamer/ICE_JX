巡检终端通过串口读取底板采集的环境参数，配置AP热点与Android端建立连接并配合巡检APP 实现指纹打卡，利用 V4L2 和 ALSA 框架分别处理视频和音频。TCP 服务器与各节点建立长连接并将上报数 据存入数据库。

JX_test_pthread_wifi_reset
	运行环境：nanoPC-t2，friendlycore-os

	编程环境：eclipse

jx_server
	使用C#语言编写基于异步回调的多线程TCP服务器，使用ACCESS数据库。

