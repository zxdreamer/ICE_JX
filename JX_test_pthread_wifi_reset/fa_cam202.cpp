/*
 * fa-cam202.cpp
 *
 *  Created on: 2017年12月27日
 *      Author: zh
 */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <getopt.h>
#include <fcntl.h>
#include <unistd.h>
#include <errno.h>
#include <malloc.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/time.h>
#include <sys/mman.h>
#include <sys/ioctl.h>
#include <asm/types.h>
#include <linux/videodev2.h>
#include <jpeglib.h>
#include "TCP_Client.hpp"
#include "AFX.hpp"
#define OUTPUT_BUF_SIZE  4096
#define CLEAR(x) memset (&(x), 0, sizeof (x))
#define WIDTH 320
#define HEIGHT 240
unsigned char *jpeg_send;
int camtest_flag=0;
struct CAM_buffer {
	void *start;
	size_t length;
};

typedef struct {
	struct jpeg_destination_mgr pub;
	JOCTET * buffer;
	unsigned char *outbuffer;
	int outbuffer_size;
	unsigned char *outbuffer_cursor;
	int *written;
} mjpg_destination_mgr;
typedef mjpg_destination_mgr *mjpg_dest_ptr;
const char * dev_name = "/dev/video0";
static int fd = -1;
struct CAM_buffer * buffers = NULL;
static unsigned int n_buffers = 0;
//FILE *file_fd;
static unsigned long file_length;
//static unsigned char *file_name;
METHODDEF(void) init_destination(j_compress_ptr cinfo) {
	mjpg_dest_ptr dest = (mjpg_dest_ptr) cinfo->dest;
	dest->buffer = (JOCTET *) (*cinfo->mem->alloc_small)((j_common_ptr) cinfo,
			JPOOL_IMAGE, OUTPUT_BUF_SIZE * sizeof(JOCTET));
	*(dest->written) = 0;
	dest->pub.next_output_byte = dest->buffer;
	dest->pub.free_in_buffer = OUTPUT_BUF_SIZE;
}
METHODDEF(boolean) empty_output_buffer(j_compress_ptr cinfo) {
	mjpg_dest_ptr dest = (mjpg_dest_ptr) cinfo->dest;
	memcpy(dest->outbuffer_cursor, dest->buffer, OUTPUT_BUF_SIZE);
	dest->outbuffer_cursor += OUTPUT_BUF_SIZE;
	*(dest->written) += OUTPUT_BUF_SIZE;
	dest->pub.next_output_byte = dest->buffer;
	dest->pub.free_in_buffer = OUTPUT_BUF_SIZE;
	return TRUE;
}
METHODDEF(void) term_destination(j_compress_ptr cinfo) {
	mjpg_dest_ptr dest = (mjpg_dest_ptr) cinfo->dest;
	size_t datacount = OUTPUT_BUF_SIZE - dest->pub.free_in_buffer;
	/* Write any data remaining in the buffer */
	memcpy(dest->outbuffer_cursor, dest->buffer, datacount);
	dest->outbuffer_cursor += datacount;
	*(dest->written) += datacount;
}
void dest_buffer(j_compress_ptr cinfo, unsigned char *buffer, int size,
		int *written) {
	mjpg_dest_ptr dest;
	if (cinfo->dest == NULL) {
		cinfo->dest
				= (struct jpeg_destination_mgr *) (*cinfo->mem->alloc_small)(
						(j_common_ptr) cinfo, JPOOL_PERMANENT,
						sizeof(mjpg_destination_mgr));
	}

	dest = (mjpg_dest_ptr) cinfo->dest;
	dest->pub.init_destination = init_destination;
	dest->pub.empty_output_buffer = empty_output_buffer;
	dest->pub.term_destination = term_destination;
	dest->outbuffer = buffer;
	dest->outbuffer_size = size;
	dest->outbuffer_cursor = buffer;
	dest->written = written;
}

//摄像头采集的YUYV格式转换为JPEG格式
int compress_yuyv_to_jpeg(unsigned char *buf, unsigned char *buffer, int size,
		int quality) {
	struct jpeg_compress_struct cinfo;
	struct jpeg_error_mgr jerr;
	JSAMPROW row_pointer[1];
	unsigned char *line_buffer, *yuyv;
	int z;
	static int written;
	//int count = 0;
	//printf("%s\n", buf);
	line_buffer = (unsigned char *) calloc(WIDTH * 3, 1);
	yuyv = buf;//将YUYV格式的图片数据赋给YUYV指针
	printf("compress start...\n");
	cinfo.err = jpeg_std_error(&jerr);
	jpeg_create_compress (&cinfo);
	/* jpeg_stdio_dest (&cinfo, file); */
	dest_buffer(&cinfo, buffer, size, &written);

	cinfo.image_width = WIDTH;
	cinfo.image_height = HEIGHT;
	cinfo.input_components = 3;
	cinfo.in_color_space = JCS_RGB;

	jpeg_set_defaults(&cinfo);
	jpeg_set_quality(&cinfo, quality, TRUE);
	jpeg_start_compress(&cinfo, TRUE);

	z = 0;
	while (cinfo.next_scanline < HEIGHT) {
		int x;
		unsigned char *ptr = line_buffer;

		for (x = 0; x < WIDTH; x++) {
			int r, g, b;
			int y, u, v;

			if (!z)
				y = yuyv[0] << 8;
			else
				y = yuyv[2] << 8;
			u = yuyv[1] - 128;
			v = yuyv[3] - 128;

			r = (y + (359 * v)) >> 8;
			g = (y - (88 * u) - (183 * v)) >> 8;
			b = (y + (454 * u)) >> 8;

			*(ptr++) = (r > 255) ? 255 : ((r < 0) ? 0 : r);
			*(ptr++) = (g > 255) ? 255 : ((g < 0) ? 0 : g);
			*(ptr++) = (b > 255) ? 255 : ((b < 0) ? 0 : b);

			if (z++) {
				z = 0;
				yuyv += 4;
			}
		}
		row_pointer[0] = line_buffer;
		jpeg_write_scanlines(&cinfo, row_pointer, 1);
	}
	jpeg_finish_compress(&cinfo);
	jpeg_destroy_compress(&cinfo);
	free(line_buffer);
	return (written);
}

//读取一帧的内容
static int read_frame(void) {
	struct v4l2_buffer buf;
	int ret;
	CLEAR (buf);
	buf.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
	buf.memory = V4L2_MEMORY_MMAP;
	int ff = ioctl(fd, VIDIOC_DQBUF, &buf); //出列采集的帧缓冲
	if (ff < 0)
		printf("failture\n");
	jpeg_send = (unsigned char*)calloc(1,buf.length + 1 + 4);
	jpeg_send[0] = 0xaa;
	jpeg_send[1] = 0xdd;
	jpeg_send[2] = 0x00;
	jpeg_send[3] = 0x21;
	assert (buf.index < n_buffers);
	printf("buf.index dq is %d,\n", buf.index);
	ret = compress_yuyv_to_jpeg((unsigned char*)buffers[buf.index].start, &jpeg_send[4], (WIDTH * HEIGHT), 80);//数据转换
	TCP_Send(jpeg_send,ret+4);
	//fwrite(&jpeg_send[4], ret, 1, file_fd);//转换后的数据写入
	ff = ioctl(fd, VIDIOC_QBUF, &buf); //重新入列
	if (ff < 0)
		printf("failture VIDIOC_QBUF\n");
	free(jpeg_send);
	return 1;
}

int get_picture()
{
	struct v4l2_capability cap;
	struct v4l2_format fmt;
	unsigned int i;
	int errno;
	enum v4l2_buf_type type;
	//file_fd = fopen("test-mmap.jpg", "w");
	if((fd = open(dev_name, O_RDWR , 0)) == -1)
	{
		perror("cam open failed\n");
		return 1;
	}
	else camtest_flag|=1<<0;                     //摄像头正常打开，记录工作状态
	printf("cam successful %d\n",fd);
	int ff = ioctl(fd, VIDIOC_QUERYCAP, &cap);   //获取摄像头参数
	if (ff < 0)
	{
		printf("failture VIDIOC_QUERYCAP\n");
		return 2;
	}
	struct v4l2_fmtdesc fmt1;
	int ret;
	memset(&fmt1, 0, sizeof(fmt1));
	fmt1.index = 0;
	fmt1.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
	while ((ret = ioctl(fd, VIDIOC_ENUM_FMT, &fmt1)) == 0) //查看摄像头所支持的格式
	{
		fmt1.index++;
		printf("{ pixelformat = '%c%c%c%c', description = '%s' }\n",
				fmt1.pixelformat & 0xFF, (fmt1.pixelformat >> 8) & 0xFF,
				(fmt1.pixelformat >> 16) & 0xFF, (fmt1.pixelformat >> 24)
						& 0xFF, fmt1.description);
	}
	CLEAR (fmt);
	fmt.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
	fmt.fmt.pix.width = WIDTH;
	fmt.fmt.pix.height = HEIGHT;
	fmt.fmt.pix.pixelformat = V4L2_PIX_FMT_YUYV;
	fmt.fmt.pix.field = V4L2_FIELD_INTERLACED;
	ff = ioctl(fd, VIDIOC_S_FMT, &fmt); //设置图像格式
	if (ff < 0)
	{
		printf("failture VIDIOC_S_FMT\n");
		return 3;
	}
	file_length = fmt.fmt.pix.bytesperline * fmt.fmt.pix.height; //计算图片大小
	struct v4l2_requestbuffers req;   //视频帧分配内存
	CLEAR (req);
	req.count = 1;                    //缓存中有多少张图片
	req.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
	req.memory = V4L2_MEMORY_MMAP;

	ioctl(fd, VIDIOC_REQBUFS, &req); //申请缓冲，count是申请的数量
	if (ff < 0)
		printf("failture VIDIOC_REQBUFS\n");
	if (req.count < 1)
		printf("Insufficient buffer memory\n");
	buffers = (struct CAM_buffer *) calloc(req.count, sizeof(struct CAM_buffer *));//内存中建立对应空间
	for (n_buffers = 0; n_buffers < req.count; ++n_buffers) {
		struct v4l2_buffer buf;
		CLEAR (buf);
		buf.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
		buf.memory = V4L2_MEMORY_MMAP;
		buf.index = n_buffers;
		if (-1 == ioctl(fd, VIDIOC_QUERYBUF, &buf)) //映射用户空间
			printf("VIDIOC_QUERYBUF error\n");
		buffers[n_buffers].length = buf.length;
		buffers[n_buffers].start = mmap(NULL, buf.length, PROT_READ
				| PROT_WRITE, MAP_SHARED, fd, buf.m.offset); //通过mmap建立映射关系
		if (MAP_FAILED == buffers[n_buffers].start)
			printf("mmap failed\n");
	}
	for (i = 0; i < n_buffers; ++i) {
		struct v4l2_buffer buf;
		CLEAR (buf);
		buf.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
		buf.memory = V4L2_MEMORY_MMAP;
		buf.index = i;
		if (-1 == ioctl(fd, VIDIOC_QBUF, &buf))//申请到的缓冲进入列队
			printf("VIDIOC_QBUF failed\n");
	}
	type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
	if (-1 == ioctl(fd, VIDIOC_STREAMON, &type)) //开始捕捉图像数据
		printf("VIDIOC_STREAMON failed\n");

	for (;;) //这一段涉及到异步IO
	{
		fd_set fds;
		struct timeval tv;
		int r;
		FD_ZERO(&fds);   //将指定的文件描述符集清空
		FD_SET(fd, &fds);//在文件描述符集合中增加一个新的文件描述符
		/* Timeout. */
		tv.tv_sec = 2;
		tv.tv_usec = 0;
		r = select(fd + 1, &fds, NULL, NULL, &tv);//判断是否可读（即摄像头是否准备好），tv是定时
		if (-1 == r) {
			if (EINTR == errno)
				continue;
			printf("select err\n");
		}
		if (0 == r) {
			fprintf(stderr, "select timeout\n");
			return 4;
		}

		if (read_frame())//如果可读，执行read_frame函数
			break;
	}
	unmap: for (i = 0; i < n_buffers; ++i)
		if (-1 == munmap(buffers[i].start, buffers[i].length))
			printf("munmap error");
	type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
	if (-1 == ioctl(fd, VIDIOC_STREAMOFF, &type))
		printf("VIDIOC_STREAMOFF");
	close(fd);
	camtest_flag &= ~(1<<0);
	//fclose(file_fd);
	//exit(EXIT_SUCCESS);
	return DEV_OK;
}
