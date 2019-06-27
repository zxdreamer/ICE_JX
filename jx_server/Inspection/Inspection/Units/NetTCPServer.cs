using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using DataCollect.Lib;
using System.IO;
using System.Threading;
using Inspection.Panel;
namespace DataCollect.Units
{
    public partial class NetTCPServer :UserControl, ICommunication
    {
        /// <summary>
        /// TCP服务端监听
        /// </summary>
        public TcpListener tcpsever = null;
        /// <summary>
        /// 监听状态
        /// </summary>
        public bool isListen = false;

        public event Lib.CollectEvent.DataReceivedHandler DataReceived;
        public event Lib.CollectEvent.Period_InquiryHandler Period_Inquiry;
        /// <summary>
        /// 当前已连接客户端集合
        /// </summary>
        public BindingList<CollectTCPClient> lstClient = new BindingList<CollectTCPClient>();
        //private BasePanel paf;
        public NetTCPServer()
        {
            InitializeComponent();
           // paf = parent;
        }

        /// <summary>
        /// 开启TCP监听
        /// </summary>
        /// <returns></returns>
        public void StartTCPServer()
        {
            try
            {
                tcpsever.Start();
                tcpsever.BeginAcceptTcpClient(new AsyncCallback(Acceptor),tcpsever);
                isListen = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "StartTCPServer错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 停止TCP监听
        /// </summary>
        /// <returns></returns>
        public void StopTCPServer()
        {
            tcpsever.Stop();
            ClearSelf();
            isListen = false;
        }

        /// <summary>
        /// 客户端连接初始化
        /// </summary>
        /// <param name="o"></param>
        private void Acceptor(IAsyncResult o)
        {
            TcpListener server = o.AsyncState as TcpListener;
            try
            {
                //初始化连接的客户端
                CollectTCPClient newClient = new CollectTCPClient();
                newClient.NetWork = server.EndAcceptTcpClient(o);
                lstConn.Invoke(new MethodInvoker(delegate {
                    //在lstConn中添加的是CollectTCPClient类型，所以在事件
                    //tebcontrol_SelectedIndexChanged中提取时也只能强制类型转换成
                    //CollectTCPClient,然后再进行其他操作，那么类CollectTCPClient的设计就至关重要
                    lstConn.Items.Add(newClient);  
                }));
                newClient.NetWork.GetStream().BeginRead(newClient.buffer, 0, newClient.buffer.Length, new AsyncCallback(TCPCallBack), newClient);
                server.BeginAcceptTcpClient(new AsyncCallback(Acceptor), server);//继续监听客户端连接
            }
            catch (ObjectDisposedException ex)
            { //监听被关闭
              //  MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Acceptor错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        /// <summary>
        /// 对当前选中的客户端发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public bool SendData(byte[] data)
        {
            if (lstConn.SelectedItems.Count > 0)
            {
                for (int i = 0; i < lstConn.SelectedItems.Count; i++)
                {
                    CollectTCPClient selClient = (CollectTCPClient)lstConn.SelectedItems[i];
                    try
                    {
                        selClient.NetWork.GetStream().Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                       /**
                        * Kabuto 2018/07/26
                        * 取消传输失误弹出消息框，改为放在列表消失时弹出
                        **/
                       // MessageBox.Show(selClient.Name + ":" + ex.Message, "SendData错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                //MessageBox.Show("无可用客户端", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 客户端通讯回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void TCPCallBack(IAsyncResult ar)
        {
            CollectTCPClient client = (CollectTCPClient)ar.AsyncState;
            try
            {
                if (client.NetWork.Connected)
                {

                    NetworkStream ns = client.NetWork.GetStream();
                    byte[] recdata = new byte[ns.EndRead(ar)];
                  
                    if (recdata.Length > 0)
                    {
                        Array.Copy(client.buffer, recdata, recdata.Length);
                        if (DataReceived != null)
                        {
                            DataReceived.BeginInvoke(client, recdata, null, null);//异步输出数据
                        }
                        ns.BeginRead(client.buffer, 0, client.buffer.Length, new AsyncCallback(TCPCallBack), client);
                    }
                    else
                    {
                        //client.DisConnect();
                        //lstConn.Invoke(new MethodInvoker(delegate {
                        //    lstConn.Items.Remove(client);
                        //    /*
                        //     * TODO : 可以不让列表消失，变色或者加一个已断开的后缀
                        //     * 
                        //     * */

                        //}));
                        //client = null;
                        //GC.Collect();// 通知托管堆强制回收垃圾
                        //在这里要关闭client.NetWork.GetStream()
                        ns.Close();
                        client.DisConnect();
                        lstConn.Invoke(new MethodInvoker(delegate
                        {
                            lstConn.Items.Remove(client);
                        }));
                        //lstClient.Remove(client);
                        //BindLstClient();
                        client = null;
                        GC.Collect();// 通知托管堆强制回收垃圾
                        if (tcpsever != null)
                        {
                            tcpsever.Stop();
                            Thread.Sleep(5000);
                            StartTCPServer();
                        }
                    }
                }
                else
                {
                    client.DisConnect();
                    lstConn.Invoke(new MethodInvoker(delegate {
                        lstConn.Items.Remove(client);
                    }));
                    //lstClient.Remove(client);
                    //BindLstClient();
                    client = null;
                    GC.Collect();// 通知托管堆强制回收垃圾
                }
            }
            catch { }
        }

        private void MS_Delete_Click(object sender, EventArgs e)
        {
            //在列表中右键显示：断开连接
            if (lstConn.SelectedItems.Count > 0)
            {
                if (lstConn.SelectedItems.Count > 0)
                {
                    List<CollectTCPClient> WaitRemove = new List<CollectTCPClient>();
                    for (int i = 0; i < lstConn.SelectedItems.Count; i++)
                    {
                        WaitRemove.Add((CollectTCPClient)lstConn.SelectedItems[i]);
                    }
                    foreach (CollectTCPClient client in WaitRemove)
                    {
                        client.DisConnect();
                       //lstClient.Remove(client);
                        lstConn.Invoke(new MethodInvoker(delegate {
                            lstConn.Items.Remove(client);
                        }));
                    }
                }
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void ClearSelf()
        {
            //清理列表中所有的显示内容
            //此函数在停止TCP监听(关闭服务器)的时候调用
            foreach (CollectTCPClient client in lstConn.Items)
            {
                client.DisConnect();
            }
            lstConn.Items.Clear();
            if (tcpsever != null)
            {
                tcpsever.Stop();
            }
        }

        private void cbxServerIP_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lstConn_SelectedIndexChanged(object sender, EventArgs e)
        {
            //选中某个客户端
            //立刻找客户端要数据，并打开一个timer1定时器，每8s发送一次check_beat要数据
            string[] str;
            if (lstConn.SelectedItems.Count > 0)
            {
                try
                {
                    byte[] check_beat = { 0xaa, 0xdd, 0x01, 0x32, 0x88, 0xEE };
                    CollectTCPClient selClient = (CollectTCPClient)lstConn.SelectedItem;
                    IPEndPoint iepR = (IPEndPoint)selClient.NetWork.Client.RemoteEndPoint;
                    str = selClient.Name.Split('-');
                    textBox_addp.Text = str[1];
                    //Advice：这里还是要加入查询指令，我感觉软件自动退出是没有安装，软件不稳定造成的
                    //安装完成后，咱在测试。
                    selClient.NetWork.GetStream().Write(check_beat, 0, check_beat.Length);
                    /*
                     * NOTE : 这个地方狂点会崩，为避免由于狂点引起的程序
                     *        崩溃，添加一个弹出框。
                     */
                    MessageBox.Show("成功查询 " + str[1] + " 的数据");
                    Period_Inquiry();
                }
                catch { }                       
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button_band_Click_1(object sender, EventArgs e)
        {
            //绑定地名按钮
            if (lstConn.SelectedItems.Count > 0)
            {  
                CollectTCPClient selClient = (CollectTCPClient)lstConn.SelectedItem;
                try
                {
                    selClient.SetName(textBox_addp.Text,1);
                    StreamWriter fileWriter = new StreamWriter("addr_ip.rng", true, Encoding.Default);
                    fileWriter.WriteLine(selClient.Name);
                    fileWriter.Flush();  //立即将数据写入addr_ip.rgn文件
                    fileWriter.Close();
                    MessageBox.Show(selClient.Name+"\r\n绑定成功！", "提示");
                    lstConn.Invoke(new MethodInvoker(delegate {
                        lstConn.Items.Remove(selClient);
                        lstConn.Items.Add(selClient);
                    }));
                    //if(textBox_addp.Text != null)
                    //{
                    //    paf.comboxArea.Items.Add(textBox_addp.Text);
                    //}
                    //lstClient.Remove(selClient);
                    //lstClient.Add(selClient);
                    //BindLstClient();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(selClient.Name + ": 绑定地名失败，请重试！" , "绑定地名失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return false;
                }

            }
            else
            {
                MessageBox.Show("无可用客户端", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // return false;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox_addp_TextChanged(object sender, EventArgs e)
        {

        }

 

    }
}
