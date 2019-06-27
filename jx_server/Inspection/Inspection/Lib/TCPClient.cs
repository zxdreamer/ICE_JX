using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Net;
using System.IO;
namespace DataCollect.Lib
{
    public class CollectTCPClient
    {
        /// <summary>
        /// 当前客户端名称
        /// </summary>
        private string _Name = "未定义";
        public string Name
        {
            get {
                return _Name;
            }
        }
        public void SetName(string addr,int fun)
        {
            if (_NetWork.Connected)
            {
                string[] ip;
                IPEndPoint iepR = (IPEndPoint)_NetWork.Client.RemoteEndPoint;
                ip = iepR.ToString().Split(':');
                if (fun == 0)
                {
                    try
                    {
                        StreamReader fileReader = new StreamReader("addr_ip.rng", Encoding.Default);
                        string line;
                        while ((line = fileReader.ReadLine()) != null)
                        {
                            string[] s = line.Split('-');
                            if ((s[0] == ip[0]) && s[1] != null)
                            {
                                addr = s[1];
                            }
                        }
                        fileReader.Close();
                    }
                    catch { }
                }
                if (addr == null)
                    addr = "***";
                _Name =ip[0]+"-"+addr;

            }
        }
        /// <summary>
        /// TCP客户端
        /// </summary>
        private TcpClient _NetWork = null;
        public TcpClient NetWork
        {
            get
            {
                return _NetWork;
            }
            set
            {
                _NetWork = value;
                SetName(null,0);
            }
        }
        /// <summary>
        /// 数据接收缓存区
        /// </summary>
        public byte[] buffer = new byte[1024*40];
        public int heart_counter = 0;
        public bool msg_flg = false;
        public bool picture_flg = false;
        public bool Core_Load_flg = false;
        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public void DisConnect()
        {
            try
            {
                if (_NetWork != null && _NetWork.Connected)
                {
                    NetworkStream ns = _NetWork.GetStream();
                    ns.Close();
                    _NetWork.Close();
                    
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
