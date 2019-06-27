using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Net.Sockets;
using System.Net;
using DataCollect.Lib;
namespace Inspection.Panel
{
    class VoiceCapture
    {
        private MemoryStream memstream;//内存流
        private SecondaryBuffer secBuffer;//辅助缓冲区
        private int iNotifySize = 0;//通知大小
        private int iBufferSize = 0;//捕捉缓冲区大小 
        private CaptureBuffer capturebuffer;//捕捉缓冲区对象
        private AutoResetEvent notifyEvent;//通知事件
        private Thread notifyThread;//通知线程
        private int iNotifyNum=0;//通知个数
        private Notify myNotify;//通知对象
        private Capture capture;//捕捉设备对象
        private Device PlayDev;//播放设备对象
        private BufferDescription buffDiscript;
        private RadioTCPClient Client;
        private int iBufferOffset=0;//捕捉缓冲区位移
        private IntPtr intptr;//窗口句柄

        public IntPtr Intptr
        {
            set
            {
                intptr = value;
            }
        }

        public int NotifySize
        {
            set
            {
                iNotifySize = value;
            }

        }

        public int NotifyNum
        {
            set
            {
                iNotifyNum = value;
            }
        }

        public RadioTCPClient LocalClient
        {
            set
            {
                Client = value;
            }
        }
        /// <summary>
        /// 初始化相关操作
        /// </summary>
        public void InitVoice()
        {//初始化声音相关设置：（1）捕捉缓冲区（2）播放缓冲区
            if (!CreateCaputerDevice())
            {
                throw new Exception();
            }//建立设备对象
            CreateCaptureBuffer();//建立缓冲区对象
            CreateNotification();//设置通知及事件
            //======（2）==============
            if (!CreatePlayDevice())
            {
                throw new Exception();
            }
            CreateSecondaryBuffer();
        }

        /// <summary>
        /// 启动声音采集
        /// </summary>
        public void StartVoiceCapture()
        {
            capturebuffer.Start(true);//true表示设置缓冲区为循环方式，开始捕捉
        }

        /// <summary>
        /// 创建用于播放的音频设备对象
        /// </summary>
        /// <returns>创建成功返回true</returns>
        private bool CreatePlayDevice()
        {
            DevicesCollection dc = new DevicesCollection();
            Guid g;
            if (dc.Count > 0)
            {
                g = dc[0].DriverGuid;
            }
            else
            { return false; }
            PlayDev = new Device(g);
            PlayDev.SetCooperativeLevel(intptr, CooperativeLevel.Normal);
            return true;
        }

        /// <summary>
        /// 创建辅助缓冲区
        /// </summary>
        private void CreateSecondaryBuffer()
        {
            buffDiscript = new BufferDescription();
            WaveFormat mWavFormat = SetWaveFormat();
            buffDiscript.Format = mWavFormat;
            iNotifySize = mWavFormat.AverageBytesPerSecond / iNotifyNum;//设置通知大小
            iBufferSize = iNotifyNum * iNotifySize;
            buffDiscript.BufferBytes = iBufferSize;
            buffDiscript.ControlPan = true;
            buffDiscript.ControlFrequency = true;
            buffDiscript.ControlVolume = true;
            buffDiscript.GlobalFocus = true;
            secBuffer = new SecondaryBuffer(buffDiscript, PlayDev);
            byte[] bytMemory = new byte[320*1024];
            memstream = new MemoryStream(bytMemory, 0,320* 1024, true, true);
        }

        /// <summary>
        /// 创建捕捉设备对象
        /// </summary>
        /// <returns>如果创建成功返回true</returns>
        private bool CreateCaputerDevice()
        {
            //首先要玫举可用的捕捉设备
            CaptureDevicesCollection capturedev = new CaptureDevicesCollection();
            Guid devguid;
            if (capturedev.Count > 0)
            {
                devguid = capturedev[0].DriverGuid;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("当前没有可用于音频捕捉的设备", "系统提示");
                return false;
            }
            //利用设备GUID来建立一个捕捉设备对象
            capture = new Capture(devguid);
            return true;
        }

        /// <summary>
        /// 创建捕捉缓冲区对象
        /// </summary>
        private void CreateCaptureBuffer()
        {
            //想要创建一个捕捉缓冲区必须要两个参数：缓冲区信息（描述这个缓冲区中的格式等），缓冲设备。
            WaveFormat mWavFormat = SetWaveFormat();
            CaptureBufferDescription bufferdescription = new CaptureBufferDescription();
            bufferdescription.Format = mWavFormat;//设置缓冲区要捕捉的数据格式
            iNotifySize = mWavFormat.AverageBytesPerSecond / iNotifyNum;//1秒的数据量/设置的通知数  得到的每个通知大小小于0.2s的数据量，话音延迟小于200ms为优质话音
            iBufferSize = iNotifyNum * iNotifySize;
            bufferdescription.BufferBytes = iBufferSize;
            bufferdescription.ControlEffects = true;
            bufferdescription.WaveMapped = true;
            capturebuffer = new CaptureBuffer(bufferdescription, capture);//建立设备缓冲区对象

        }

        //设置通知
        private void CreateNotification()
        {
            BufferPositionNotify[] bpn = new BufferPositionNotify[iNotifyNum];//设置缓冲区通知个数
            //设置通知事件
            notifyEvent = new AutoResetEvent(false);
            notifyThread = new Thread(RecoData);//通知触发事件
            notifyThread.IsBackground = true;
            notifyThread.Start();
            for (int i = 0; i < iNotifyNum; i++)
            {
                bpn[i].Offset = iNotifySize + i * iNotifySize - 1;//设置具体每个的位置
                bpn[i].EventNotifyHandle = notifyEvent.Handle;
            }
            myNotify = new Notify(capturebuffer);
            myNotify.SetNotificationPositions(bpn);
        }
        //线程中的事件
        private void RecoData()
        {
            while (true)
            {
                // 等待缓冲区的通知消息
                notifyEvent.WaitOne(Timeout.Infinite, true);
                // 录制数据
                RecordCapturedData(Client);
            }
        }
        int count = 0;
        byte[] empty = new byte[1024 * 8];
        //真正转移数据的事件，其实就是把数据传送到网络上去。
        private void RecordCapturedData(RadioTCPClient Client)
        {
            byte[] capturedata = null;
            int readpos = 0, capturepos = 0, locksize = 0;
            capturebuffer.GetCurrentPosition(out capturepos, out readpos);
            locksize = readpos - iBufferOffset;//这个大小就是我们可以安全读取的大小
            if (locksize == 0)
            {
                return;
            }
            if (locksize < 0)
            {//因为我们是循环的使用缓冲区，所以有一种情况下为负：当文以载读指针回到第一个通知点，而Ibuffeoffset还在最后一个通知处
                locksize += iBufferSize;
            }
            capturedata = (byte[])capturebuffer.Read(iBufferOffset, typeof(byte), LockFlag.FromWriteCursor, locksize);
            //capturedata = g729.Encode(capturedata);//语音编码
            try
            {
                if (Client.NetWork.Connected)
                {
                    count++;
                    Client.NetWork.GetStream().Write(capturedata, 0, capturedata.Length);
                    if (count >= 4)
                    {
                        count = 0;
                        //Client.NetWork.GetStream().Write(empty, 0, empty.Length/2);
                    }
                }
            }
            catch
            {
                //throw new Exception();
            }
            try
            {
                iBufferOffset += capturedata.Length;
                iBufferOffset %= iBufferSize;//取模是因为缓冲区是循环的。
            }
            catch { }

        }


        public int intPosWrite = 0;//内存流中写指针位移
        public int intPosPlay = 0;//内存流中播放指针位移
        private int intNotifySize = 96*1024;//设置通知大小

        /// <summary>
        /// 从字节数组中获取音频数据，并进行播放
        /// </summary>
        /// <param name="intRecv">字节数组长度</param>
        /// <param name="bytRecv">包含音频数据的字节数组</param>
        public void GetVoiceData(int intRecv, byte[] bytRecv)
        {
            //intPosWrite指示最新的数据写好后的末尾。intPosPlay指示本次播放开始的位置。
            if (intPosWrite + intRecv <= memstream.Capacity)
            {//如果当前写指针所在的位移+将要写入到缓冲区的长度小于缓冲区总大小
                if ((intPosWrite - intPosPlay >= 0 && intPosWrite - intPosPlay < intNotifySize) || (intPosWrite - intPosPlay < 0 && intPosWrite - intPosPlay + memstream.Capacity < intNotifySize))
                {
                    memstream.Write(bytRecv, 0, intRecv);
                    intPosWrite += intRecv;
                }
                else if (intPosWrite - intPosPlay >= 0)
                {//先存储一定量的数据，当达到一定数据量时就播放声音。
                    buffDiscript.BufferBytes = intPosWrite - intPosPlay;//缓冲区大小为播放指针到写指针之间的距离。
                    SecondaryBuffer sec = new SecondaryBuffer(buffDiscript, PlayDev);//建立一个合适的缓冲区用于播放这段数据。
                    memstream.Position = intPosPlay;//先将memstream的指针定位到这一次播放开始的位置
                    sec.Write(0, memstream, intPosWrite - intPosPlay, LockFlag.FromWriteCursor);
                    sec.Play(0, BufferPlayFlags.Default);
                    memstream.Position = intPosWrite;//写完后重新将memstream的指针定位到将要写下去的位置。
                    intPosPlay = intPosWrite;
                }
                else if (intPosWrite - intPosPlay < 0)
                {
                    buffDiscript.BufferBytes = intPosWrite - intPosPlay + memstream.Capacity;//缓冲区大小为播放指针到写指针之间的距离。
                    SecondaryBuffer sec = new SecondaryBuffer(buffDiscript, PlayDev);//建立一个合适的缓冲区用于播放这段数据。
                    memstream.Position = intPosPlay;
                    sec.Write(0, memstream, memstream.Capacity - intPosPlay, LockFlag.FromWriteCursor);
                    memstream.Position = 0;
                    sec.Write(memstream.Capacity - intPosPlay, memstream, intPosWrite, LockFlag.FromWriteCursor);
                    sec.Play(0, BufferPlayFlags.Default);
                    memstream.Position = intPosWrite;
                    intPosPlay = intPosWrite;
                }
            }
            else
            {//当数据将要大于memstream可容纳的大小时
                int irest = memstream.Capacity - intPosWrite;//memstream中剩下的可容纳的字节数。
                memstream.Write(bytRecv, 0, irest);//先写完这个内存流。
                memstream.Position = 0;//然后让新的数据从memstream的0位置开始记录
                memstream.Write(bytRecv, irest, intRecv - irest);//覆盖旧的数据
                intPosWrite = intRecv - irest;//更新写指针位置。写指针指示下一个开始写入的位置而不是上一次结束的位置，因此不用减一
            }
        }

        /// <summary>
        /// 设置音频格式，如采样率等
        /// </summary>
        /// <returns>设置完成后的格式</returns>
        private WaveFormat SetWaveFormat()
        {
            WaveFormat format = new WaveFormat();
            format.FormatTag = WaveFormatTag.Pcm;//设置音频类型
            format.SamplesPerSecond = 44100;//采样率（单位：赫兹）典型值：11025、22050、44100Hz
            format.BitsPerSample = 16;//采样位数
            format.Channels = 2;//声道
            format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));//单位采样点的字节数
            format.AverageBytesPerSecond = format.BlockAlign * format.SamplesPerSecond;
            return format;
            //按照以上采样规格，可知采样1秒钟的字节数为22050*2=44100B 约为 43K
        }
        /// <summary>
        /// 停止语音采集
        /// </summary>
        public void Stop()
        {
            capturebuffer.Stop();
            if (notifyEvent != null)
            {
                notifyEvent.Set();
            }
            if (notifyThread != null && notifyThread.IsAlive == true)
            {
                notifyThread.Abort();
            }
        }
    }
}
