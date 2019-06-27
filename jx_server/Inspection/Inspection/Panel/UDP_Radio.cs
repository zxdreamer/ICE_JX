using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;
namespace Inspection.Panel
{
    public partial class UDP_Radio : UserControl
    {
        public UDP_Radio()
        {
            InitializeComponent();
        }
        private Socket server;
        private int iNotifySize = 0;//通知大小
        private int iBufferSize = 0;//捕捉缓冲区大小 
       // private MemoryStream memstream;//内存流
        private SecondaryBuffer secBuffer;//辅助缓冲区
        private CaptureBuffer capturebuffer;//捕捉缓冲区对象
        private Capture capture;//捕捉设备对象
        private Device PlayDev;//播放设备对象
        private BufferDescription buffDiscript;
        private AutoResetEvent notifyEvent;//通知事件
        private Thread notifyThread;//通知线程
        private int iNotifyNum = 0;//通知个数
        private Notify myNotify;//通知对象    
        private int iBufferOffset = 0;//捕捉缓冲区位移
       // private IntPtr intptr;//窗口句柄
        public EndPoint point;
        Thread th;
        public void Init()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Any, 7000));//绑定端口号和IP
            //server.Blocking = false;
            server.ReceiveTimeout = 100;
            iNotifyNum = 100;
            try
            {
                InitVoice();
                capturebuffer.Start(true);
                th = new Thread(ReciveMsg);
                th.Start();//开启接收消息线程
            }
            catch
            {
                server.Close();
                MessageBox.Show("提醒：\n     请先插入麦克风与扬声器，在点击确定", "提醒");
                Init();                     //递归调用，如果一直没有对讲设备，一直提醒用户
            }

        }
        public void Deint()
        {
            server.Close();
            server = null;
            capturebuffer.Stop();
            if (notifyEvent != null)
            {
                notifyEvent.Set();
            }
            if (notifyThread != null && notifyThread.IsAlive == true)
            {
                notifyThread.Abort();
            }
            th.Abort();
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
            PlayDev.SetCooperativeLevel(this.Handle, CooperativeLevel.Normal);
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
            buffDiscript.BufferBytes = 2048;//
            buffDiscript.ControlPan = true;
            buffDiscript.ControlFrequency = true;
            buffDiscript.ControlVolume = true;
            buffDiscript.GlobalFocus = true;
            secBuffer = new SecondaryBuffer(buffDiscript, PlayDev);
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
                if (point != null)
                {
                    // 等待缓冲区的通知消息
                    notifyEvent.WaitOne(Timeout.Infinite, true);
                    // 录制数据
                    RecordCapturedData(point);
                }
            }
        }
        //真正转移数据的事件，其实就是把数据传送到网络上去。
        private void RecordCapturedData(EndPoint  Client)
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
                server.SendTo(capturedata, point);
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
        /// <summary>
        /// 向特定ip的主机的端口发送数据报
        /// </summary>
        //void sendMsg()
        //{

        //    capturebuffer.Start(true);
        //    while (true)
        //    {
        //        byte[] capturedata = null;
        //        capturedata = (byte[])capturebuffer.Read(0, typeof(byte), LockFlag.FromWriteCursor, 1280);
        //        server.SendTo(capturedata, point);
        //        Thread.Sleep(20);
        //    }
        //}

        public int vioce_count(byte[] data)
        {
            int count = 0;
            for (int i = 0; i < data.Length; i++)
                if (data[i] >= 0x30)
                    count++;
            return count;
        }
        byte[] buffer = new byte[2048];
        /// <summary>
        /// 接收发送给本机ip对应端口号的数据报
        /// </summary>
        public void ReciveMsg()
        {
          
            while (true)
            {
                try
                {
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                    int length = server.ReceiveFrom(buffer, ref point);//接收数据报
                    secBuffer.Play(0, BufferPlayFlags.Looping);
                    secBuffer.Write(0, buffer, LockFlag.FromWriteCursor);
                }
                catch
                {
                    secBuffer.Stop();
                };
                Array.Clear(buffer,0,2048);
            }
        }

        private WaveFormat SetWaveFormat()
        {
            WaveFormat format = new WaveFormat();
            format.FormatTag = WaveFormatTag.Pcm;//设置音频类型
            format.SamplesPerSecond = 8000;//采样率（单位：赫兹）典型值：11025、22050、44100Hz
            format.BitsPerSample = 16;//采样位数
            format.Channels = 2;//声道
            format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));//单位采样点的字节数
            format.AverageBytesPerSecond = format.BlockAlign * format.SamplesPerSecond;
            return format;
            //按照以上采样规格，可知采样1秒钟的字节数为22050*2=44100B 约为 43K
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void UDP_Radio_Load(object sender, EventArgs e)
        {

        }

    }
}
