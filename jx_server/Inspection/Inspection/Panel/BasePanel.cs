using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using DataCollect.Lib;
using System.IO;
using System.Threading;
using System.Data.OleDb;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;

namespace Inspection.Panel
{
    public partial class BasePanel : UserControl
    {
        private CollectTCPClient connect_save = null;
        byte[] heart_beat = { 0xaa, 0xdd, 0x01, 0x37, 0x88, 0xEE };
        byte[] check_beat = { 0xaa, 0xdd, 0x01, 0x32, 0x88, 0xEE };
        byte[] radio_beat_y = { 0xaa, 0xdd, 0x01, 0x81,0x01,0x88, 0xEE };
        byte[] radio_beat_n = { 0xaa, 0xdd, 0x01, 0x81,0x02,0x88, 0xEE };

        byte[] pic_Data = new byte[40 * 1024];
        byte[] pic_buff = new byte[40 * 1024];
        string[] ip_id = new string[20];
        static string mdb = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=db_col.mdb"; //Jet连接，根据自己的设置
        //static string mdb = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=accdb_col.accdb";
        static OleDbConnection conn = new OleDbConnection(mdb);  //定义数据库连接
        //int time1_relay_count = 0;
        bool relay1_state = false;
        bool relay2_state = false;
        bool relay3_state = false;
        bool relay4_state = false;
        
        public Bitmap mybitmap;

        // 从textBox_ipaddr中获取旧的IP
        String Old_ip = " ";
        
        public BasePanel()
        {
            conn.Open();
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today;
            label_number.Text = "";
            //从textBox_ipaddr中获取旧的IP
            Old_ip = textBox_ipaddr.Text.ToString();

            Thread ThTestL = new Thread(new ParameterizedThreadStart(heart_deal));
            ThTestL.IsBackground = true;
            ThTestL.Start(1000);
            cbxServerIP.SelectedIndex = 0;
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in ipHostEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {//筛选IPV4
                    cbxServerIP.Items.Add(ip.ToString());
                }
            }
            // insert.Hide();
            //insert.Inquiry_InsertInfo += Inquiry_InsertInfo1;
            // insert.Delete_InsertInfo += Delete_InsertInfo1;
            textBox_reporttime.Text = DateTime.Now.ToString();
            netTCPServer1.DataReceived += NetTCPServer1_DataReceived1;
        }
        
        private void NetTCPServer1_DataReceived1(object sender, byte[] data)
        {
            CollectTCPClient client = (CollectTCPClient)sender;
            string[] str = new string[2];
            str = client.Name.Split('-');
            //    }
            /**
             * irisqp
             * 2018-06-13
             */
            if ((client.buffer[0] == 0xAA) && (client.buffer[1] == 0xBB)) {
                //Debug.Print("收到的信息：" + client.buffer.Length);
                //for (int info = 0; info < 6; info++)
                //{
                //    Debug.Print(client.buffer[info] + " ");
                //}
                //Debug.Print("\n");
                //switch (data[2]) {

                //    case 0x00:
                //        if (data[3] == 0x01)
                //        {
                //            label_test_con.Text = "正常";
                //        }
                //        else{
                //            label_test_con.Text = "异常";
                //        }
                //        break;
                //    case 0x01:
                //        if (data[3] == 0x01)
                //        {
                //            label_test_camera.Text = "正常";
                //        }
                //        else if (data[3] == 0x00)
                //        {
                //            label_test_camera.Text = "异常";
                //        }
                //        break;
                //    case 0x02:
                //        if (data[3] == 0x00)
                //        {
                //            label_test_radio.Text = "正常";
                //        }
                //        else if (data[3] == 0x01)
                //        {
                //            label_test_radio.Text = "麦克风异常";
                //        }
                //        else if (data[3] == 0x02)
                //        {
                //            label_test_radio.Text = "扬声器异常";
                //        }
                //        else if (data[3] == 0x03)
                //        {
                //            label_test_radio.Text = "异常";
                //        }
                //        break;
                //    case 0x03:
                //        if (data[3] == 0x01)
                //        {
                //            label_test_wifi.Text = "正常";
                //        }
                //        else if (data[3] == 0x00)
                //        {
                //            label_test_wifi.Text = "异常";
                //        }
                //        break;
                //    case 0x04:
                //        if (data[3] == 0x01)
                //        {
                //            label_test_tempture.Text = "正常";
                //        }
                //        else if (data[3] == 0x00)
                //        {
                //            label_test_tempture.Text = "异常";
                //        }
                //        break;
                //    case 0x05:
                //        if (data[3] == 0x01)
                //        {
                //            label_test_humid.Text = "正常";
                //        }
                //        else if (data[3] == 0x00)
                //        {
                //            label_test_humid.Text = "异常";
                //        }
                //        break;
                //    case 0x06:
                //        if (data[3] == 0x01)
                //        {
                //            label_test_balance.Text = "正常";
                //        }
                //        else if (data[3] == 0x00)
                //        {
                //            label_test_balance.Text = "异常";
                //        }
                //        break;
                //}
            }

            //接受数据
            if ((client.buffer[0] == 0xAA) && (client.buffer[1] == 0xDD))
            {
                switch (data[3])
                {
                    case 0x21:
                        //拍照
                        client.picture_flg = true;
                        break;
                    case 0x22:
                        /*
                         * 请求语音通话
                         */
                        if (radio_flg == false)
                        //上位机语音空闲，可以接入语音
                        {
                            udP_Radio1.point = new IPEndPoint(IPAddress.Parse(str[0]), 7000);
                            connect_save = client;
                            Debug.Print("当前空闲，请求语音连接->当前IP为"+IPAddress.Parse(str[0]).ToString());
                            client.NetWork.GetStream().Write(radio_beat_y, 0, radio_beat_y.Length);
                            this.Invoke(new MethodInvoker(delegate
                            {
                                textBox_raddr.Text = str[1];
                            }));
                            radio_flg = true;
                        }
                        else
                        {
                           //上位忙机，拒绝接入
                           client.NetWork.GetStream().Write(radio_beat_n, 0, radio_beat_n.Length);
                           Debug.Print("当前忙，请求语音连接->当前IP为" + IPAddress.Parse(str[0]).ToString());
                        }
                        break;
                    case 0x23:

                        /*
                         * kabuto 
                         * 20180605
                         * 语音链接请求断开
                         */
                        string[] str1 = new string[2];
                        str1 = connect_save.Name.Split('-');

                        if(str[0] == str1[0])
                        {
                            radio_flg = false;
                            Debug.Print("》》》》》》》》当前连接的终端请求断开");
                            udP_Radio1.point = null;
                            client.NetWork.GetStream().Write(radio_beat_n, 0, radio_beat_n.Length);
                            this.Invoke(new MethodInvoker(delegate
                            {
                                textBox_raddr.Clear();
                            }));
                            Debug.Print("请求语音断开，服务器空闲->当前IP为" + IPAddress.Parse(str[0]).ToString());
                        }

                        break;
                  /**
                   * irisqp
                   * 2018-06-26
                   * 机箱远程复位
                   */
                    case 0x31:
                        //第二套下位机机箱复位反馈信息
                        switch (data[4]) {
                            case 0x01:
                                if (data[5] == 0x00)
                                {
                                    label_reset.Text = "复位失败";
                                }
                                else if (data[5] == 0x01) {
                                    label_reset.Text = "复位成功!";
                                }
                                break;
                        }
                        break;
                    case 0x37:
                        //回复心跳包
                        this.Invoke(new MethodInvoker(delegate
                        {
                            client.heart_counter = 0;
                            client.NetWork.GetStream().Write(heart_beat, 0, heart_beat.Length);
                        }));
                        Debug.Print("接收心跳包->当前IP为" + IPAddress.Parse(str[0]).ToString());
                        break;
                    /**
                     * irisqp
                     * 2018-06-26
                     * 交流接触器复位
                     */
                    case 0x38:
                        label_reset_relay1.Text = "启动复位";
                        relay1_state = false;
                        //time_relay_count = 0;
                        timer_relay.Enabled = false;
                        if (data[6] == 0x01)
                        {
                            //label_reset_relay1.Text = "复位成功!";
                            MessageBox.Show("复位成功！！！");
                        }
                        else if (data[6] == 0x00)
                        {
                            //label_reset_relay1.Text = "复位失败";
                            MessageBox.Show("复位失败！！！，请重新复位", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case 0x39:
                        label_reset_relay2.Text = "启动复位";
                        relay2_state = false;
                        //time_relay_count = 0;
                        timer_relay.Enabled = false;
                        if (data[6] == 0x01)
                        {
                            //label_reset_relay2.Text = "复位成功!";
                            MessageBox.Show("复位成功！！！");
                        }
                        else if (data[6] == 0x00)
                        {
                            //label_reset_relay2.Text = "复位失败";
                            MessageBox.Show("复位失败！！！，请重新复位", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case 0x48:
                        label_reset_relay3.Text = "启动复位";
                        relay3_state = false;
                        //time_relay_count = 0;
                        timer_relay.Enabled = false;
                        if (data[6] == 0x01)
                        {
                            //label_reset_relay3.Text = "复位成功!";
                            MessageBox.Show("复位成功！！！");
                        }
                        else if (data[6] == 0x00)
                        {
                            //label_reset_relay3.Text = "复位失败";
                            MessageBox.Show("复位失败！！！，请重新复位", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case 0x49:
                        label_reset_relay4.Text = "启动复位";
                        relay4_state = false;
                        //time_relay_count = 0;
                        timer_relay.Enabled = false;
                        if (data[6] == 0x01)
                        {
                            //label_reset_relay4.Text = "复位成功!";
                            MessageBox.Show("复位成功！！！");
                        }
                        else if (data[6] == 0x00)
                        {
                            //label_reset_relay4.Text = "复位失败";
                            MessageBox.Show("复位失败！！！，请重新复位", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                      /**
                      * irisqp
                      * 2018-09-18
                      * wifi打卡与RFID打卡
                      */
                    case 0x41:
                        //打卡
                        //地点-》ip
                        String ip = IPAddress.Parse(str[0]).ToString();
                        String addr = null;  //设备地址
                        try
                        {
                            StreamReader fileReader = new StreamReader("addr_ip.rng", Encoding.Default);
                            string line;
                            while ((line = fileReader.ReadLine()) != null)
                            {
                                string[] s = line.Split('-');
                                if ((s[0] == ip) && s[1] != null)
                                {
                                    addr = s[1];
                                }
                            }
                            fileReader.Close();
                        }
                        catch { }
                        Debug.Print("addr = "+addr);

                        //时间-》系统
                        DateTime dt = new DateTime();
                        dt = System.DateTime.Now;
                        string time_Id = dt.ToString("yyyy-MM-dd HH:mm:ss");

                        //工号
                        String Worker_Id = "";
                        for (int i = 0; i < 8; i++)
                        {
                            int num = data[4 + i] - 48;
                            Worker_Id = Worker_Id + num;
                        }

                       
                        //从数据库Insert_Info_Map中读取员工姓名
                        OleDbCommand cmd = conn.CreateCommand();
                        //Sql语句的命令文本
                        cmd.CommandText = "select 姓名 from Insert_Info_Map where 工号 =" + "\'" + Worker_Id + "\'";
                        //conn.Open();
                        String Worker_name = (String)cmd.ExecuteScalar();
                        Debug.Print(Worker_name);
                        cmd.Dispose();

                        //把数据写入Insert_Info_Map2中
                        string sql = "insert into Insert_Info_Map2(工号,密码,姓名,地点,时间)" +
                        "values('" +
                        Worker_Id + "','" +
                        " " + "','" +
                        Worker_name + "','" +
                        addr + "','" +
                        time_Id +
                        "')";
                        try
                        {
                            OleDbCommand comm = new OleDbCommand(sql, conn);
                            comm.ExecuteNonQuery();
                            comm.Dispose();
                        }
                        catch { }

                        break;
                    default: client.msg_flg = true; break;

                }
            }

        }
        /**
         * irisqp 2018-07-19
         * 增加保存图片的功能，每次拍照将照片保存到当前上位机运行目录下的“照片记录\地段名\***.jpg”
         */
        void dis_picture(byte[] data,string name)
        {
            int i, k, jpgstart = 0, headok = 0, jpglen = 0;
            for (i = 0; i < data.Length; i++)
            {
                if (data[i] == 0xFF && data[i + 1] == 0xD8)
                {
                    jpgstart = i;
                    headok = 1;
                }
                if ((data[i] == 0XFF) && (data[i + 1] == 0XD9) && (headok == 1))
                {
                    jpglen = i - jpgstart + 2;
                    break;
                }
            }
            for (k = 0; k < jpglen; k++)
                pic_Data[k] = data[jpgstart + k];
            if (jpgstart != jpglen)
                try
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Image image;
                        MemoryStream ms = new MemoryStream(pic_Data);
                        image = Image.FromStream(ms);
                        Bitmap bm = new Bitmap(image);
                        String fileName = DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                        String tempTime = DateTime.Now.ToString();
                        String filePath = "照片记录\\" + fileName + ".jpg";
                        if (!Directory.Exists("照片记录\\" + name))
                        {
                            Directory.CreateDirectory("照片记录\\" + name);
                        }
                        
                        String pictureUrl = "照片记录\\" + name + "\\" + fileName + ".jpg";
                        //将此图的url存入数据库的picture_table中
                        String pictureSql = "insert into picture_table([address],[time],picture_url)" +
                        "values(" + "\'" + 
                        name + "\'" + "," + "\'" + 
                        tempTime + "\'" + "," + "\'" +
                        pictureUrl + "\'" + ")";
                        //+ " and 时间 = " + "\'" + tempTime + "\'
                        try
                        {
                            OleDbCommand comm = new OleDbCommand(pictureSql, conn);
                            comm.ExecuteNonQuery();
                            comm.Dispose();
                        }
                        catch{ }
                        //bm.RotateFlip(RotateFlipType.Rotate180FlipX);
                        bm.Save(pictureUrl, ImageFormat.Jpeg);
                        pictureBox1.Image = bm;
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        //label_picAddr.Text = name + "  拍摄时间: " + DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                        label_area.Text = name + "  拍摄时间: " + DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                    }));
                }
                catch { }
        }

        private void heart_deal(object Interval)
        {
            while (true)
            {
                if (netTCPServer1.lstConn.Items.Count > 0)
                {
                    for (int i = 0; i < netTCPServer1.lstConn.Items.Count; i++)
                    {
                        CollectTCPClient client = (CollectTCPClient)netTCPServer1.lstConn.Items[i];
                        string[] str = new string[2];
                        str = client.Name.Split('-');
                        if (str[1] == null)
                            str[1] = "****";

                        //login登录成功
                        if (Load_Success_flg)
                        {
                            Load_Success_flg = false;
                            byte[] load_success = { 0xaa, 0xdd, 0x07, 0x38, 0x01, 0x88, 0xee };
                            this.Invoke(new MethodInvoker(delegate
                            {
                                client.NetWork.GetStream().Write(load_success, 0, load_success.Length);
                            }));
                        }
                        // login登录失败
                        else if (Load_Error_flg)
                        {
                            Load_Error_flg = false;
                            byte[] load_error = { 0xaa, 0xdd, 0x07, 0x38, 0x00, 0x88, 0xee };
                            this.Invoke(new MethodInvoker(delegate
                            {
                                client.NetWork.GetStream().Write(load_error, 0, load_error.Length);
                            }));
                        }
                        else if (client.Core_Load_flg)
                        {
                            Check_Load(client.buffer);
                            client.Core_Load_flg = false;
                            client.heart_counter = 0;
                        }
                        else if (client.msg_flg == true)
                        {
                            if ((client.buffer[0] == 0xAA) && (client.buffer[1] == 0xDD))
                            {
                                deal_data(client.buffer, str[1]);
                            }
                            client.heart_counter = 0;
                            client.msg_flg = false;
                        }
                        else if (client.picture_flg)
                        {
                            for (int j = 0; j < client.buffer.Length; j++)
                                pic_buff[j] = client.buffer[j];
                            dis_picture(pic_buff, str[1]);
                            client.picture_flg = false;
                            client.heart_counter = 0;
                        }
                        else
                        {
                            client.heart_counter++;
                            if (client.heart_counter == 30)
                            {
                                Debug.Print("xxxxxxxxxxxxxxxx This is the counter = 30 xxxxxxxxxxxxx");
                                String[] addr_ip = new String[2];
                                addr_ip = client.Name.Split('-');
                                udP_Radio1.offline_warning_tb.AppendText(client.Name + "\r\n" + 
                                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  掉线" + "\r\n"+"========================="+"\r\n");
                                /**
                                 * 将异常数据存入数据库
                                 */

                                //把数据写入offline_table中
                                string sql = "insert into offline_table([ip],[addr],[time])" +
                                "values('" +
                                addr_ip[0] + "','" +
                                addr_ip[1] + "','" +
                                DateTime.Now.ToString() + 
                                "')";
                                try
                                {
                                    OleDbCommand comm = new OleDbCommand(sql, conn);
                                    comm.ExecuteNonQuery();
                                    comm.Dispose();
                                }
                                catch { }
                                
                                
                                client.heart_counter = 0;

                                client.DisConnect();
                                netTCPServer1.lstConn.Invoke(new MethodInvoker(delegate
                                {
                                    netTCPServer1.lstConn.Items.Remove(client);
                                }));
                                client = null;
                                GC.Collect();// 通知托管堆强制回收垃圾
                                
                            }
                        }
                    }
                }
                else
                { 
                    timer1.Enabled = false;
                }
                Thread.Sleep(1000);
            }
        }
        public bool Load_Success_flg = false;
        public bool Load_Error_flg = false;
        public void Check_Load(byte[] data)
        {
            string Str_num = "";
            string Str_password = "";
            for (int i = 4; i <= 11; i++)
            {
                Str_num += (data[i]-48).ToString();
            }
            for (int i = 12; i <= 17; i++)
            {
                Str_password += (data[i]-48).ToString();
            }
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Insert_Info_Map where 工号='" + Str_num + "'";
            OleDbDataReader dr = cmd.ExecuteReader();
            string str_pass = "";

            while (dr.Read())
            {
                str_pass = dr[1].ToString();
            }
            if (str_pass != "")
            {
                if (str_pass == Str_password)
                {
                    Load_Success_flg = true;
                    Load_Error_flg = false;
                }
                else
                {
                    Load_Error_flg = true;
                    Load_Success_flg = false;
                }
            }
        }



        bool radio_flg = false;

        private void NetTCPServer1_DataReceived(object sender, byte[] data)
        {
           
        }
        private void button_setth_Click(object sender, EventArgs e)
        {
            //byte[] setth = { 0xaa, 0xdd, 0x02, 0x34, 0x00, 0x00,0x00, 0x88, 0xEE };
            //setth[4] = (byte)numericUpDown_tmp.Value;
            //setth[5] = (byte)numericUpDown_tmp2.Value;
            //setth[6] = (byte)numericUpDown_hum.Value;
            //this.Invoke(new MethodInvoker(delegate
            //{
            //    bool send;
            //    send= netTCPServer1.SendData(setth);
            //    //if (send == true)
            //       // textBox_redis.AppendText("温湿度阈值设置成功     " + DateTime.Now.ToString() + "\r\n");
            //}));
        }
        private void button_setcycle_Click(object sender, EventArgs e)
        {
            decimal cycle = numericUpDown_cycle.Value;
            if (cycle > 99 || cycle < 10)
            {
                MessageBox.Show("请输入10到99的数");
                return;
            }
            byte[] setcycle = { 0xaa, 0xdd, 0x02, 0x35, 0x00, 0x00, 0x00, 0x88, 0xEE };
            setcycle[4] = (byte)(numericUpDown_cycle.Value / 100);
            setcycle[5] = (byte)(numericUpDown_cycle.Value % 100 / 10);
            setcycle[6] = (byte)(numericUpDown_cycle.Value % 10);
            this.Invoke(new MethodInvoker(delegate
            {
                bool send;
                 send = netTCPServer1.SendData(setcycle);
                 if (send == true)
                     MessageBox.Show("采集间隔设置成功");
                 else
                     MessageBox.Show("采集间隔设置失败");
                //textBox_redis.AppendText("查询周期设置成功      " + DateTime.Now.ToString() + "\r\n");
            }));
        }
        private void button_setip_Click(object sender, EventArgs e)
        {
            int i,port;
            byte[] lan = new byte[60];
            lan[0] = 0xaa;
            lan[1] = 0xdd;
            lan[2] = 52;
            lan[3] = 0x36;
            try
            {
                string[] ip =textBox_ipaddr.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[i + 4] = (byte)(Convert.ToInt32(ip[i]));
                }
                string[] mask = textBox_mask.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[i + 8] = (byte)(Convert.ToInt32(mask[i]));
                }
                string[] def = textBox_gateway.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[i + 12] = (byte)(Convert.ToInt32(def[i]));
                }
                string[] remote = textBox_severip.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[i + 16] = (byte)(Convert.ToInt32(remote[i]));
                }
                port = Convert.ToInt32(textBox_severport.Text);
                lan[20] = (byte)(port / 256);
                lan[21] = (byte)(port % 256);
                lan[22] = 0x88;
                lan[23] = 0xee;
                //lan[54] = (byte)(port % 1000 %100 /10);
                //lan[55] = (byte)(port % 10);
            }
            catch { }
            this.Invoke(new MethodInvoker(delegate
            {
               bool send;
               send = netTCPServer1.SendData(lan);
               if (send == true)
                   MessageBox.Show("IP信息设置成功");
               else
                   MessageBox.Show("IP信息设置失败");
            }));
        }
        private void button_rest_Click(object sender, EventArgs e)
        {
            //远程机箱复位
            label_reset.Text = "复位中...";
            byte[] rest = { 0xaa, 0xdd, 0x02, 0x31, 0x00, 0x00, 0x00, 0x88, 0xEE }; //新老版本都是这条
            this.Invoke(new MethodInvoker(delegate
            {
                bool send;
                send= netTCPServer1.SendData(rest);
                /*
                 * DATE：2018.08.31
                 * AUTHOR：Kabuto
                 * NOTE：由于老版本没有反馈，这里发送成功之后就立即显示复位成功
                 */
                 if(send == true)
                {
                    label_reset.Text = "复位成功";
                }
                //if (send == true)
                   // textBox_redis.AppendText("远程复位机箱成功      " + DateTime.Now.ToString() + "\r\n");
            }));
        }
        private bool remote_relay_deal(byte[] relaydate)
        {
 //           Button relay_bytton = (Button)sender;
            bool send = false;
            if (relay1_state == true)
            {
                MessageBox.Show("交流接触器1复位中！！！","警告", MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
                return false;
            }
            else if (relay2_state == true)
            {
                MessageBox.Show("交流接触器2复位中！！！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return false;
            }
            else if (relay3_state == true)
            {
                MessageBox.Show("交流接触器3复位中！！！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return false;
            }
            else if (relay4_state == true)
            {
                MessageBox.Show("交流接触器4复位中！！！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    //bool send;
                    send = netTCPServer1.SendData(relaydate);
                }));
                return send;
            }
        }
        private void button_relay1_Click(object sender, EventArgs e)
        {
            //远程复位交流接触器1
            
            byte[] relay1 = { 0xaa, 0xdd, 0x02, 0x38, 0x00, 0x00, 0x00, 0x88, 0xEE };    //为了统一老版本，新版本机箱更改了复位协议
            if(netTCPServer1.lstConn.SelectedItems.Count == 1)
            {
                timer_relay.Enabled = true;
                timer_relay.Interval = 40000;//定时40s
                bool Bool_relay1 = remote_relay_deal(relay1);
                if (Bool_relay1 == true)
                {
                    label_reset_relay1.Text = "复位中...";
                    relay1_state = true;
                }
            }
            else
            {
                MessageBox.Show("请选中一台设备", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Thread.Sleep(1000);
        }

        private void button_relay2_Click(object sender, EventArgs e)
        {
            //远程复位交流接触器2
            byte[] relay2 = { 0xaa, 0xdd, 0x02, 0x39, 0x00, 0x00, 0x00, 0x88, 0xEE };  //同button_relay1_Click
            if (netTCPServer1.lstConn.SelectedItems.Count == 1)
            {
                timer_relay.Enabled = true;
                timer_relay.Interval = 40000;//定时40s
                bool Bool_relay2 = remote_relay_deal(relay2);
                if (Bool_relay2 == true)
                {
                    label_reset_relay2.Text = "复位中...";
                    relay2_state = true;
                }
            }
            else
            {
                MessageBox.Show("请选中一台设备", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Thread.Sleep(1000);
        }

        string temp1, temp2, gate,humid;
       
        int textcount = 0;

        public void deal_data(byte[] data,string addrname)
        {
            string site1, site2;

            this.Invoke(new MethodInvoker(delegate
            {
                switch (data[3])
                {
                    //查询全部信息,主动查询所传上来的数据，主要有环境数据和网络信息数据
                    case 0x32:
                        textBox_addrname.Text = addrname;
                        if (data[4] >= 128)                          //温度信息
                          temp1 = (data[4] - 256 - 5).ToString();
                        else
                          temp1 = (data[4] - 5).ToString();
                        temp1 += "." + data[5].ToString();
                        textBox_temp1.Text = temp1+ " ℃";

                        if(data[6] == 0 && data[7] == 0)  //温度2位00 00时，代表新版上位机上传的温度
                        {
                            temp2 = temp1;
                        }
                        else
                        {
                            if (data[6] >= 128)
                                temp2 = (data[6] - 256 - 5).ToString();
                            else
                                temp2 = (data[6] - 5).ToString();
                            temp2 += "."+data[7].ToString();
                        }                       
                        textBox_temp2.Text = temp2+ " ℃";
                        textBox_humid.Text = data[8].ToString() + " %";  //湿度信息
                        if (data[9] >128)                                //机箱倾斜信息
                            site1 = "右倾" + ((256-data[9])*5).ToString() + "°";
                        else if (data[9] >0)
                            site1 = "左倾" + (data[9] * 5).ToString() + "°";
                        else
                            site1 = "平衡";
                        textBox_site.Text = site1;
                        if (data[10] > 128)
                            site2 = "后倾" + ((256 - data[10])*5).ToString() + "°";
                        else if (data[10] > 0)
                            site2 = "前倾" + (data[10] * 5).ToString() + "°";
                        else
                            site2 = "平衡";
                        textBox_site2.Text = site2;
                           //12 11预留
                        //data[13]箱门状态
                        if (data[13] == 0)
                        {
                            textBox_gate.Text = "打开";
                        }
                        else
                        textBox_gate.Text = "关闭";
                        textBox_reporttime.Text = DateTime.Now.ToString();

                        /*
                         *  NOTE : 添加一个判断，判断IP是否变化，如果没变就不要更新IP参数界面，以免影响修改IP
                         *  Modified by : Kabuto
                         *  DATE ：2018-09-03 16:23:30
                         */
                        // 从数据包中获取新IP
                        String New_ip = data[14].ToString() + '.' + data[15].ToString() + '.' + data[16].ToString() + '.' + data[17].ToString();
                       
                        //两者不一样，更改ip显示界面
                        if (Old_ip.CompareTo(New_ip) != 0)
                        {
                            textBox_ipaddr.Text = data[14].ToString() + '.' + data[15].ToString() + '.' + data[16].ToString() + '.' + data[17].ToString();
                            textBox_mask.Text = data[18].ToString() + '.' + data[19].ToString() + '.' + data[20].ToString() + '.' + data[21].ToString();
                            textBox_gateway.Text = data[22].ToString() + '.' + data[23].ToString() + '.' + data[24].ToString() + '.' + data[25].ToString();
                            textBox_severip.Text = data[26].ToString() + '.' + data[27].ToString() + '.' + data[28].ToString() + '.' + data[29].ToString();
                            textBox_severport.Text = (data[30] * 256 + data[31]).ToString();

                            //更新old_ip
                            Old_ip = textBox_ipaddr.Text.ToString();
                        }
   
                       // numericUpDown_tmp.Value = data[32] ;
                        //numericUpDown_tmp2.Value = data[33];
                        //numericUpDown_hum.Value = data[34];
                        numericUpDown_cycle.Value = data[35]*256 + data[36];
                        label_number.Text = System.Text.Encoding.ASCII.GetString(data,37,20);
                    break;
                    //上报全部信息，由下位机定时上传的数据，只有环境信息，在这里存储定时上报的数据
                    case 0x33:
                        int t1 = 0, t2 = 0;
                        if(data[6] == 0 && data[7] == 0)
                        {
                            data[6] = data[4];
                            data[7] = data[5];
                        }
                        /*
                         * NOTE : 解决地名一直变化的问题
                         * TIME ：2018-09-10 17:31:55
                         * AIHTOR : Kabuto
                         */
                        //   textBox_addrname.Text = addrname;            //显示地点
                        if (data[4] >= 128)                          //温度信息
                        {
                            t1 = data[4] - 256 - 5;
                            temp1 = t1.ToString();
                        }
                        else
                        {
                            t1 = data[4] - 5;
                            temp1 = t1.ToString();
                        }
                        temp1 += "." + data[5].ToString();
                        //double temps1 = double.Parse(temp1);
                        //textBox_temp1.Text = temp1 + " ℃";
                        if (t1 > numericUpDown_tmp.Value)
                        {
                            textBox1.AppendText(addrname + "    温度1超阈值  " + DateTime.Now.ToString() + "\r\n");
                            textcount++;
                        }
                        if (data[6] >= 128)
                        {
                            t2 = data[6] - 256 - 5;
                            temp2 = (t2).ToString();
                        }
                        else
                        {
                            t2 = data[6] - 5;
                            temp2 = t2.ToString();
                        }
                        temp2 += "." + data[7].ToString();
                        //double temps2 = double.Parse(temp2);
                        // textBox_temp2.Text = temp2 + " ℃";
                        //humid = textBox_humid.Text = data[8].ToString() + " %";  //湿度信息
                        if (t2 > numericUpDown_tmp2.Value)
                        {
                            textBox1.AppendText(addrname + "     温度2超阈值  " + DateTime.Now.ToString() + "\r\n");
                            textcount++;
                        }
                        humid =data[8].ToString();  //湿度信息
                        if (data[8] > numericUpDown_hum.Value)
                        {
                            textBox1.AppendText(addrname + "     湿度超阈值   " + DateTime.Now.ToString() + "\r\n");
                            textcount++;
                        }
                        if (data[9] > 128)                                //机箱倾斜信息
                            site1 = "右倾" + ((256 - data[9]) * 5).ToString() + "°";
                        else if (data[9] > 0)
                            site1 = "左倾" + (data[9] * 5).ToString() + "°";
                        else
                            site1 = "平衡";
                       // textBox_site.Text = site1;
                        if (data[10] > 128)
                            site2 = "后倾" + ((256 - data[10]) * 5).ToString() + "°";
                        else if (data[10] > 0)
                            site2 = "前倾" + (data[10] * 5).ToString() + "°";
                        else
                            site2 = "平衡";
                        //data[13]箱门状态
                        if (data[13] == 0)
                        {
                            textBox1.AppendText(addrname + "     箱门被打开   " + DateTime.Now.ToString() + "\r\n");
                            textcount++;
                            gate =  "打开";
                        }
                        else
                            gate = "关闭";
                        string time = DateTime.Now.ToString();
                        string sql = "insert into map(name,temp1,temp2,humid,gate,site1,site2,时间)" +
                        "values('"+
                        addrname + "','" +
                        temp1 + "','" +
                        temp2 + "','" +
                        humid + "','" +
                        gate + "','" +
                        site1 + "','" +
                        site2 + "','" +
                        time + 
                        "')";
                        try
                        {
                            OleDbCommand comm = new OleDbCommand(sql, conn);
                            comm.ExecuteNonQuery();
                            comm.Dispose();
                        }
                        catch { }
                        if (textcount == 16)
                        {
                            textcount = 0;
                            textBox1.Clear();
                        }
                        break;
                    case 0x34:
                    break;
                    
                    //case 0x39:   //WiFi打卡是0x41，RFID打卡是0x40，0x39是远程复位交流接收器2
                    //   //指纹打卡存入数据库
                    //   /*
                    //    * NOTE: 没有上传0x39的数据，暂时先放着
                    //    */
                    //   string Str_num = "";
                    //   string time1 = DateTime.Now.ToString();
                    //   for (int i = 4; i <= 11; i++)
                    //   {
                    //        Str_num += (data[i]-48).ToString();
                    //   }
                    //   string Str_name = "";
                    //   OleDbCommand cmd1 = conn.CreateCommand();
                    //   cmd1.CommandText = "select * from Insert_Info_Map where 工号='" + Str_num + "'";
                    //   OleDbDataReader dr1 = cmd1.ExecuteReader();
                    //   while (dr1.Read())
                    //   {
                    //       Str_name = dr1[2].ToString();
                    //   }
                    //   if (Str_name != "")
                    //   {
                    //       string Str_site = "";
                    //       Str_site = addrname;

                    //       string sql1 = "insert into Insert_Info_Map2(工号,密码,姓名,地点,时间)" +
                    //       "values('" +
                    //       Str_num + "','" +
                    //       "" + "','" +
                    //       Str_name + "','" +
                    //       Str_site + "','" +
                    //       time1 +
                    //       "')";
                    //       try
                    //       {
                    //           OleDbCommand comm = new OleDbCommand(sql1, conn);
                    //           comm.ExecuteNonQuery();
                    //           // conn.Close();
                    //       }
                    //       catch { }
                    //   }
                 
                    //break;
                        
                    case 0x40:
                        //RFID打卡记录---存入新版本的数据库Insert_Info_Map2表单中
                        string name_str = "RFID打卡";   //名字
                        string rfid_str = "";           //rfid号
                        rfid_str = data[4].ToString("X2") + data[5].ToString("X2") + data[6].ToString("X2") + data[7].ToString("X2");
                        
                        string ktime = DateTime.Now.ToString();  //打卡时间
                        string ksql = "insert into Insert_Info_Map2(工号,密码,姓名,地点,时间)" +
                             "values('" +
                             rfid_str + "','" +
                             "" + "','" +
                             name_str + "','" +
                             addrname + "','" +
                             ktime +
                             "')";
                        try
                        {
                            OleDbCommand comk = new OleDbCommand(ksql, conn);
                            comk.ExecuteNonQuery();
                            comk.Dispose();
                        }
                        catch { }
                        break;
                    default: break;
                }
            }));

         }
        private void button_deldata_Click(object sender, EventArgs e)
        {
            ASK ask = new ASK("将删除所有历史记录，确定删除？");
            ask.ShowDialog();
            if (ask.ok == true)
            {
                //OleDbCommand cmd = conn.CreateCommand();
                //cmd.CommandText = "delete from map";
                //cmd.ExecuteNonQuery();

                string sql_del_map = "delete * from map";
                OleDbCommand cmd = new OleDbCommand(sql_del_map, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void button_delkdata_Click(object sender, EventArgs e)
        {
            ASK ask = new ASK("将删除所有考勤记录，确定删除？");
            ask.ShowDialog();
            if (ask.ok == true)
            {
                //OleDbCommand cmd = conn.CreateCommand();
                //cmd.CommandText = "delete from Insert_Info_Map2";
                //cmd.ExecuteNonQuery();
                //listView2.Items.Clear();

                //按条件删除，字段不需要加引号，值要加入引号
                //string sql_del_sign = "delete * from Insert_Info_Map2 where 姓名='test' OR 地点='555'";
                string sql_del_sign = "delete * from Insert_Info_Map2";
                OleDbCommand cmd = new OleDbCommand(sql_del_sign, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button_deljpg_Click(object sender, EventArgs e)
        {
            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage_network_Click(object sender, EventArgs e)
        {

        }

        //private void button_status_Click(object sender, EventArgs e)
        //{
        //    //原查询按钮，无用
        //    byte[] stat = { 0xaa, 0xdd, 0x02, 0x32, 0x08, 0x4d, 0x88, 0xEE };
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(stat);
        //        //if (send == true)
        //            //textBox_redis.AppendText("查询命令发送成功      "+DateTime.Now.ToString() + "\r\n");
        //    }));
        //}

        private void netTCPServer1_Load(object sender, EventArgs e)
        {

        }

        private void button_readhis_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            //if(netTCPServer1.lstConn.Items.Count < 0)
            //{
            //    MessageBox.Show("设备列表中没有数据");
            //    return;
            //}
            //string[] Listconn_Select = new string[2];
            //CollectTCPClient selClient = (CollectTCPClient)netTCPServer1.lstConn.SelectedItem;
            //if (selClient != null)
            //{
            //    //MessageBox.Show("警告:\n       \n没有打开服务器或主机网线断开", "警告");
            //    //return;
            //    Listconn_Select = selClient.Name.Split('-');
            //    textBoxArea.Text = Listconn_Select[1];               
            //}

            /****************************************************************************************/
            string YQ_JXArea = comboxArea1.Text;
            if (YQ_JXArea == "")
            {
                MessageBox.Show("请先输入查询地点 或 在设备列表中选中设备", "警告");
                return;
            }
            /*********************************************/

            //Listconn_Select = netTCPServer1.lstConn.SelectedItem.ToString().Split('-'); //获得listconn中选中的设备名
            //if (Listconn_Select[1] == "" || Listconn_Select[1] == "***")  //如果没绑定设备，提示先绑定
            //{
            //    MessageBox.Show("请先绑定" + Listconn_Select[0] + "的地点", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //定义链接
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=" + Application.StartupPath + "//db_col.mdb";//将数据库放到debug目录下。
            connection.Open();
            DataSet dc = new DataSet();
            OleDbDataAdapter oleDapAdaptor;
            oleDapAdaptor = new OleDbDataAdapter("select * from map", connection);
            oleDapAdaptor.Fill(dc);                                           //dc中存储数据库中的数据
            string[] time1 = new string[2];
            string[] time2 = new string[2];
            int sei = 0;
            int timeSameFlag = 0;
            int areaSameFlag = 0;
            //List<string> Ballslist = new List<string>();
            for (int i = 0; i < dc.Tables[0].Rows.Count; i++)                 //按行读取数据库的数据
            {
                time1 = dc.Tables[0].Rows[i]["时间"].ToString().Split(' ');   //获得数据库中“时间”字段
                time2 = dateTimePicker1.Value.ToString().Split(' ');
                if (time1[0] == time2[0])                                      //筛选1.指定时间
                {
                    timeSameFlag = 1;
                    if (dc.Tables[0].Rows[i]["name"].ToString() == YQ_JXArea)  //筛选2.指定地点
                    {
                        areaSameFlag = 1;
                        ListViewItem item = new ListViewItem();
                        //item.SubItems.Add((++sei).ToString());
                        item.Text = (++sei).ToString();                             //指定listview1的序号
                        item.SubItems.Add(dc.Tables[0].Rows[i]["name"].ToString()); //以下内容一次指定listview1的歌列
                        item.SubItems.Add(dc.Tables[0].Rows[i]["temp1"].ToString());
                        item.SubItems.Add(dc.Tables[0].Rows[i]["temp2"].ToString());
                        item.SubItems.Add(dc.Tables[0].Rows[i]["humid"].ToString());
                        item.SubItems.Add(dc.Tables[0].Rows[i]["gate"].ToString());
                        //item.SubItems.Add(dc.Tables[0].Rows[i]["power"].ToString());
                        //item.SubItems.Add(dc.Tables[0].Rows[i]["site_l"].ToString());
                        //item.SubItems.Add(dc.Tables[0].Rows[i]["site_f"].ToString());
                        item.SubItems.Add(dc.Tables[0].Rows[i]["site1"].ToString());
                        item.SubItems.Add(dc.Tables[0].Rows[i]["site2"].ToString());
                        item.SubItems.Add(dc.Tables[0].Rows[i]["时间"].ToString());
                        listView1.Items.Add(item);                                   //启动显示
                    }
                }

            }
            if(timeSameFlag == 0 && areaSameFlag == 0)
            {
                MessageBox.Show("查询地点与查询时间不正确，请查证后重新查询", "错误");
            }
            else if(timeSameFlag == 0)
            {
                MessageBox.Show("查询时间不正确，请查证后重新查询", "错误");
            }
            else if(areaSameFlag == 0)
            {
                MessageBox.Show("查询地点不正确，请查证后重新查询", "错误");
            }
            dc.Reset();
            timeSameFlag = 0;
            areaSameFlag = 0;
        }

        private void button12_Click_1(object sender, EventArgs e)
        {

        }

        private void button_satrt_Click(object sender, EventArgs e)
        {
            //textBox_temp2.Text = "test";
            //textBox_temp1.Text = "test";
            if (netTCPServer1.isListen == false)
            {//监听已停止
                if (cbxServerIP.SelectedIndex == 0)
                {
                    netTCPServer1.tcpsever = new TcpListener(IPAddress.Any, (int)nmServerPort.Value);
                }
                else
                {
                    netTCPServer1.tcpsever = new TcpListener(IPAddress.Parse(cbxServerIP.SelectedItem.ToString()), (int)nmServerPort.Value);
                }
                netTCPServer1.StartTCPServer();
                udP_Radio1.Init();
            }
            else
            {//监听已开启
                netTCPServer1.StopTCPServer();
                udP_Radio1.Deint();
                radio_flg = false;
            }
            cbxServerIP.Enabled = !netTCPServer1.isListen;
            nmServerPort.Enabled = !netTCPServer1.isListen;
            if (netTCPServer1.isListen)
            {
                button_start.Text = "关闭服务器";
            }
            else
            {
                button_start.Text = "开启服务器";
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button_clearlist_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }
        //按照指定的地点进行查询
        private void button_readattn_Click(object sender, EventArgs e)
        {
            int sei = 0;
            bool areaSameFlag = false;
            if (CMBoxAreaSeach.Text == "")
            {
                MessageBox.Show("查询工号为空，请选择工号", "错误");
                return;
            }
            listView2.Items.Clear();
            DataSet Dc_sign_area = new DataSet();
            //OleDbCommand ole_sign_id = new OleDbCommand()
            OleDbDataAdapter Ole_sign_area = new OleDbDataAdapter("select * from Insert_Info_Map2", conn);
            Ole_sign_area.Fill(Dc_sign_area);
            for (int i = 0; i < Dc_sign_area.Tables[0].Rows.Count; i++)
            {
                if (Dc_sign_area.Tables[0].Rows[i]["地点"].ToString() == CMBoxAreaSeach.Text)
                {
                    areaSameFlag = true;
                    ListViewItem item = new ListViewItem();
                    item.Text = (++sei).ToString();
                    item.SubItems.Add(Dc_sign_area.Tables[0].Rows[i]["工号"].ToString());
                    item.SubItems.Add(Dc_sign_area.Tables[0].Rows[i]["姓名"].ToString());
                    item.SubItems.Add(Dc_sign_area.Tables[0].Rows[i]["地点"].ToString());
                    item.SubItems.Add(Dc_sign_area.Tables[0].Rows[i]["时间"].ToString());
                    listView2.Items.Add(item);
                }
            }
            if (areaSameFlag == false)
            {
                MessageBox.Show("没有查询到该地点下的打卡记录");
            }
            Dc_sign_area.Reset();
            areaSameFlag = false;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
        }

        //private void button_checkswitch_Click(object sender, EventArgs e)
        //{
        //    //原复位代码、此处无用
        //    byte[] resj = { 0xaa, 0xdd, 0x02, 0x38, 0x08, 0x4d, 0x88, 0xEE };
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        netTCPServer1.SendData(resj);
        //    }));
        //}


        Load load = new Load();
        private void BtonEntry_Click(object sender, EventArgs e)
        {
             
            if (load.ok == false)
            {
                
                load.ShowDialog();
            }
            if(load.ok==true)
            {
                Insert insert = new Insert("录入信息");
                insert.ShowDialog();
            
            /*
            insert.textBox_worknum.Text = "";
            insert.textBox_name.Text = "";
            insert.textBox_phone.Text = "";
            insert.textBox_password.Text = "";
            insert.textBox_email.Text = "";
            */
            
                if (insert.ok == true)
                {
                    string work_num = insert.textBox_worknum.Text.Trim();
                    string password = insert.textBox_password.Text.Trim();
                    string name = insert.textBox_name.Text.Trim();
                    string phone = insert.textBox_phone.Text.Trim();
                    string email = insert.textBox_email.Text.Trim();
                    string time = DateTime.Now.ToString();

                    if (work_num == "")
                    {
                        MessageBox.Show("工号不能位空!", "提示");
                   
                    }
                    else if (password == "")
                    {
                        MessageBox.Show("密码不能为空！", "提示");
                    }
                    else
                    {
                         string sql = "insert into Insert_Info_Map(工号,密码,姓名,电话,邮箱,时间)" +//字符串类型在执行数据库的插入
                             "values('" +                                                       //与删除操作时需要加入单引号''
                              work_num + "','" +        
                              password + "','" +
                              name + "','" +
                              phone + "','" +
                              email + "','" +
                              time +
                              "')";
                         try
                         {
                            OleDbCommand comm = new OleDbCommand(sql, conn);
                            comm.ExecuteNonQuery();
                            comm.Dispose();
                            // conn.Close();
                         }
                         catch (System.Data.OleDb.OleDbException)
                         {
                                 MessageBox.Show("录入异常！", "提示");
                                 return;
                         }
                         MessageBox.Show("录入成功！", "提示");
                    }
                }
            }
        }
/*
        private void button4_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Insert_Info_Map";
            // conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            int num = 0;
            while (dr.Read())
            {
                string[] str = new string[10];
                str[0] = dr[0].ToString();

                for (int i = 1; i < dr.FieldCount-1; i++)
                {
                    // textBox_redis.AppendText(dr[i].ToString() + "  ");
                    str[i] = dr[i+1].ToString();
                }

                ListViewItem item = new ListViewItem((++num).ToString());
                item.SubItems.AddRange(str);
                listView2.Items.Add(item);
            }
            cmd.Dispose();
        }
        */
        /*
        private void button5_Click(object sender, EventArgs e)
        {
            ASK ask = new ASK("将删除所有录入信息，确定删除？");
            ask.ShowDialog();
            if (ask.ok == true)
            {
                OleDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = "delete from Insert_Info_Map";
                cmd.ExecuteNonQuery();
            }
        }*/

        private void tabPage_opdev_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox_addrname_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_DoubleClick_1(object sender, EventArgs e)
        {
          
        }

        private void udP_Radio1_Load(object sender, EventArgs e)
        {

        }

        private void netTCPServer1_Load_1(object sender, EventArgs e)
        {

        }

        private void cbxServerIP_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown_report_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown_tmp_ValueChanged(object sender, EventArgs e)
        {
            //if (numericUpDown_tmp.Value > 99 || numericUpDown_tmp.Value < -55)
            //{
            //    MessageBox.Show("请输入-55到99之间的数");
            //    return;
            //}

        }

        private void numericUpDown_hum_ValueChanged(object sender, EventArgs e)
        {
            //if (numericUpDown_hum.Value > 99 ||   numericUpDown_hum.Value < 20) {
            //    MessageBox.Show("请输入20到99之间的数");
            //    return;
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

       

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox_ipaddr_TextChanged(object sender, EventArgs e)
        {

        }

        //private void label_test_radio_Click(object sender, EventArgs e)
        //{

        //}
        //private void button_test_camera_Click(object sender, EventArgs e)
        //{
        //    byte[] camera_test = { 0xaa, 0xbb, 0x01, 0x01, 0x88, 0xff };
        //    label_test_camera.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(camera_test);
              
        //    }));
        //}

        //private void tab_Page_test_Click(object sender, EventArgs e)
        //{

        //}

        //private void button_test_tempture_Click(object sender, EventArgs e)
        //{
        //    byte[] tempture_test = { 0xaa, 0xbb, 0x04, 0x01, 0x88, 0xff };
        //    label_test_tempture.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(tempture_test);

        //    }));
        //}

        //private void button_test_balance_Click(object sender, EventArgs e)
        //{
            
        //    byte[] balance_test = { 0xaa, 0xbb, 0x06, 0x01, 0x88, 0xff };


        //    label_test_balance.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(balance_test);

        //    }));
            

        //}

        //private void button_test_con_Click(object sender, EventArgs e)
        //{
        //    byte[] con_test = { 0xaa, 0xbb, 0x00, 0x01, 0x88, 0xff };
        //    label_test_con.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(con_test);

        //    }));
        //}

        //private void button_test_radio_Click(object sender, EventArgs e)
        //{
        //    byte[] radio_test = { 0xaa, 0xbb, 0x02, 0x01, 0x88, 0xff };
        //    label_test_radio.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(radio_test);

        //    }));
        //}

        //private void button_test_wifi_Click(object sender, EventArgs e)
        //{
        //    byte[] wifi_test = { 0xaa, 0xbb, 0x03, 0x01, 0x88, 0xff };
        //    label_test_wifi.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(wifi_test);

        //    }));
        //}

        //private void button_test_humid_Click(object sender, EventArgs e)
        //{
        //    byte[] humid_test = { 0xaa, 0xbb, 0x05, 0x01, 0x88, 0xff };
        //    label_test_humid.Text = "检测中...";
        //    this.Invoke(new MethodInvoker(delegate
        //    {
        //        bool send;
        //        send = netTCPServer1.SendData(humid_test);

        //    }));
        //}

        //private void label_test_balance_Click(object sender, EventArgs e)
        //{

        //}

        //private void label_test_tempture_Click(object sender, EventArgs e)
        //{

        //}

        //private void label_test_humid_Click(object sender, EventArgs e)
        //{

        //}

        //private void tab_Page_test_Click_1(object sender, EventArgs e)
        //{

        //}

        //private void label_test_wifi_Click(object sender, EventArgs e)
        //{

        //}

        //private void label_test_camera_Click(object sender, EventArgs e)
        //{

        //}

        //private void label_test_con_Click(object sender, EventArgs e)
        //{

        //}

        //private void label_reset_relay1_Click(object sender, EventArgs e)
        //{

        //}

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button_export_equipment_Click(object sender, EventArgs e)
        {
            ExportToExecl(listView1);

        }
        // <summary>
        /// 执行导出数据
        /// </summary>
        public void ExportToExecl(ListView listview)
        {
            if (listview.Items.Count <= 0)
            {
                MessageBox.Show("请先点击 “历史记录查询” 按钮", "警告");
                return;
            }
            System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "xls";
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DoExport(listview, sfd.FileName);
            }
        }
        /// <summary>
        /// 具体导出的方法
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="strFileName">导出到的文件名</param>
        private void DoExport(ListView listView, string strFileName)
        {
            int rowNum = listView.Items.Count;
            int columnNum = listView.Items[0].SubItems.Count;
            int rowIndex = 1;
            int columnIndex = 0;
            if (rowNum == 0 || string.IsNullOrEmpty(strFileName))
            {
                return;
            }
            if (rowNum > 0)
            {

                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                if (xlApp == null)
                {
                    MessageBox.Show("无法创建excel对象，可能您的系统没有安装excel");
                    return;
                }
                xlApp.DefaultFilePath = "";
                xlApp.DisplayAlerts = true;
                xlApp.SheetsInNewWorkbook = 1;
                Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
                //将ListView的列名导入Excel表第一行
                foreach (ColumnHeader dc in listView.Columns)
                {
                    columnIndex++;
                    xlApp.Cells[rowIndex, columnIndex] = dc.Text;
                }
                //将ListView中的数据导入Excel中
                for (int i = 0; i < rowNum; i++)
                {
                    rowIndex++;
                    columnIndex = 0;
                    for (int j = 0; j < columnNum; j++)
                    {
                        columnIndex++;
                        //注意这个在导出的时候加了“\t” 的目的就是避免导出的数据显示为科学计数法。可以放在每行的首尾。
                        xlApp.Cells[rowIndex, columnIndex] = Convert.ToString(listView.Items[i].SubItems[j].Text) + "\t";
                    }
                }
                //例外需要说明的是用strFileName,Excel.XlFileFormat.xlExcel9795保存方式时 当你的Excel版本不是95、97 而是2003、2007 时导出的时候会报一个错误：异常来自 HRESULT:0x800A03EC。 解决办法就是换成strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal。
                xlBook.SaveAs(strFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                xlApp = null;
                xlBook = null;
                MessageBox.Show("导出完成！");
            }
        }

        private void button_export_check_Click(object sender, EventArgs e)
        {
            ExportToExecl(listView2);
        }

        private void groupBox12_Enter(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void textBox_temp2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void textBox_humid_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_gate_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_temp1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void tabPagePicture_Click(object sender, EventArgs e)
        {

        }
    /*
     * Irisqp-2018-7-20
     * 增加通过数据库查询图片功能
     * 包括根据地段名+时间两种查询模式
     * 下面这个函数是根据地段的所有查询
     */
    private void searchPictureButton_Click(object sender, EventArgs e)
        {
            listViewPicture.Items.Clear();
            imageList1.Images.Clear();
            String add = textBoxPictureLocation.Text.Trim();
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select time,picture_url from picture_table where address = " + "\'" + add + "\'";
            String pictureUrl = "";
            try
            { 
                OleDbDataReader dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    Debug.Print("当前查询到的时间为：" + dr[0].ToString());
                    String tempTime = dr[0].ToString();
                    Debug.Print("当前查询到的路径为：" + dr[1].ToString());
                    pictureUrl = dr[1].ToString();
                    imageList1.Images.Add(Image.FromFile(pictureUrl));
                    ListViewItem li = new ListViewItem();
                    li.Text = add + tempTime;
                    li.ImageIndex = i++;
                    listViewPicture.Items.Add(li);
                    //if (i >= 50) //限制图片查询数量为50，防止程序死机
                       // break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            cmd.Dispose();

        }
        /*
        * Irisqp-2018-7-20
        * 增加通过数据库查询图片功能
        * 包括根据地段名+时间两种查询模式
        * 下面这个函数是根据时间+地段的精确查询
        */
        private void searchPictureByTimebutton_Click(object sender, EventArgs e)
        {
            listViewPicture.Items.Clear();
            imageList1.Images.Clear();
            String add = textBoxPictureLocation.Text.Trim();
            String timePicker = dateTimePicker4Picture.Value.ToString();
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select time,picture_url from picture_table where address = " + "\'" + add + "\'" ;
            String pictureUrl = "";
            try
            {
                OleDbDataReader dr = cmd.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    string[] time1 = new string[2];
                    string[] time2 = new string[2];
                    time1 = dr[0].ToString().Split(' ');
                    Debug.Print("time1为：" + time1[0].ToString());
                    time2 = timePicker.Split(' ');
                    Debug.Print("time2为：" + time2[0].ToString());
                    if (time1[0] == time2[0]) {
                        Debug.Print("当前查询到的时间为：" + dr[0].ToString());
                        String tempTime = dr[0].ToString();
                        Debug.Print("当前查询到的路径为：" + dr[1].ToString());
                        pictureUrl = dr[1].ToString();
                        imageList1.Images.Add(Image.FromFile(pictureUrl));
                        ListViewItem li = new ListViewItem();
                        li.Text = add + tempTime;
                        li.ImageIndex = i++;
                        listViewPicture.Items.Add(li);
                    }
                    if (i >= 50) //限制图片查询数量为50，防止程序死机
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd.Dispose();
        }

        private void dateTimePicker4Picture_ValueChanged(object sender, EventArgs e)
        {

        }
        /*
         *Irisqp-2018-7-20
         * 增加双击图片导出图片功能 
         */
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Title = "图片保存";
            saveImageDialog.Filter = @"jpeg|*.jpg|bmp|*.bmp";
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveImageDialog.FileName.ToString();
                if (fileName != "" && fileName != null)
                {
                    string fileExtName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToString();
                    ImageFormat imgformat = null;


                    if (fileExtName != "")
                    {
                        switch (fileExtName)
                        {
                            case "jpg":
                                imgformat = ImageFormat.Jpeg;
                                break;
                            case "bmp":
                                imgformat = ImageFormat.Bmp;
                                break;
                            default:
                                imgformat = ImageFormat.Jpeg;
                                break;
                        }


                        try
                        {
                            Bitmap bit = new Bitmap(pictureBox1.Image);
                            pictureBox1.Image.Save(fileName, imgformat);
                            MessageBox.Show("已保存至：" + fileName);
                        }
                        catch (Exception exx)
                        {
                            MessageBox.Show(exx.Message);

                        }
                    }
                }
            }
        }
        /**
         * Irisqp-2018-7-20
         * 实现异常情况查询：异常情况包括：温度，湿度超阈值或者不平衡或者箱门被打开
         * 下面这个异常复杂的sql语句就是上面所有异常情况的并集查询。
         */
        private void button_searchUnusual_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            OleDbCommand cmd = conn.CreateCommand();
            Debug.Print(numericUpDown_hum.Value + "");
            cmd.CommandText = "select * from map where val(humid) > " + numericUpDown_hum.Value + " or  val(temp1) > " 
                + numericUpDown_tmp.Value + " or val(temp2) > " + numericUpDown_tmp2.Value + " or site1 like " + "\'" + "%倾%" + "\'" +
                " or site2 like " + "\'" + "%倾%" + "\'" + " or gate = " + "\'" + "打开" + "\'";
            // conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            int sei = 0;
            while (dr.Read())
            {
                string[] time1 = new string[2];
                string[] time2 = new string[2];
                time1 = dr[dr.FieldCount - 1].ToString().Split(' ');
                time2 = dateTimePicker1.Value.ToString().Split(' ');

                if (time1[0] == time2[0])
                {
                    string[] str = new string[10];
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // textBox_redis.AppendText(dr[i].ToString() + "  ");
                        str[i] = dr[i].ToString();
                    }
                    ListViewItem item = new ListViewItem((++sei).ToString());
                    item.SubItems.AddRange(str);
                    listView1.Items.Add(item);
                }
                if (sei >= 300) //设备异常记录显示限制为300条
                    break;
            }
            cmd.Dispose();
        }

        private void netTCPServer1_Period_Inquiry()
        {
            timer1.Enabled = true;   
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void numericUpDown_cycle_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void label_WifiPassWord_Click(object sender, EventArgs e)
        {

        }
        /**
         * Irisqp-2018-7-20
         * 增加修改wifi名称和密码的功能
         */
        private void button_SetWifiCommit_Click(object sender, EventArgs e)
        {
            String wifiName = textBox_wifiName.Text.Trim();
            
            String password = textBox_WifiPassword.Text.Trim();

            int passwordLength = password.Length;
            int nameLength = wifiName.Length;
            if (nameLength == 0) {
                MessageBox.Show("wifi名不能为空");
                return;
            }
            if (passwordLength == 0)
            {
                MessageBox.Show("密码不能为空");
                return;
            }

            if (nameLength > 100)
            {
                MessageBox.Show("wifi名长度不能超过60个字符");
                return;
            }
            if (passwordLength > 10 || passwordLength < 8)
            {
                MessageBox.Show("请输入8-10位WiFi密码");
                return;
            }

            byte[] sendNameConfig = new byte[6 + nameLength + 1 + passwordLength + 50];

            //初始化名称传输数组
            sendNameConfig[0] = 0xaa;
            sendNameConfig[1] = 0xdd;
            sendNameConfig[2] = (byte)nameLength;
            sendNameConfig[3] = 0x56;
            for (int i = 0; i < nameLength; i++)
            {
                sendNameConfig[4 + i] = (byte)wifiName[i];
                Debug.Print(sendNameConfig[4 + i] + "");
            }
            //传输密码
            sendNameConfig[4 + nameLength] = (byte)passwordLength;
            Debug.Print(sendNameConfig[4 + nameLength] + "");
            for (int i = 0; i < passwordLength; i++)
            {
                sendNameConfig[4 + nameLength + 1 + i] = (byte)password[i];
                Debug.Print(sendNameConfig[4 + nameLength + 1 + i] + "");
            }
            sendNameConfig[4 + nameLength + 1 + passwordLength] = 0x88;
            sendNameConfig[5 + nameLength + 1 + passwordLength] = 0xee;

            this.Invoke(new MethodInvoker(delegate
            {
                bool send;
                send = netTCPServer1.SendData(sendNameConfig);
                if (send == true)
                    MessageBox.Show("wifi设置成功");
                else
                    MessageBox.Show("wifi设置失败");
            }));
        }

        private void textBox_wifiName_TextChanged(object sender, EventArgs e)
        {
            
            
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }
        int time_check_count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] check_beat = { 0xaa, 0xdd, 0x01, 0x32, 0x88, 0xEE };
            time_check_count++;
            //time_relay_count++;
            if (time_check_count == 60)
            {
                time_check_count = 0;
                this.Invoke(new MethodInvoker(delegate
                {
                    netTCPServer1.SendData(check_beat);
                }));
            }
        }

        private void button_report_Click(object sender, EventArgs e)
        {
            decimal cycle = numericUpDown_report.Value;
            Debug.Print("cycle值为：" + cycle);
            if (cycle > 25200 || cycle < 10) {
                MessageBox.Show("请输入10到25200以内的数字");
                return;
            }
            byte[] set_reportcycle = { 0xaa, 0xdd, 0x34, 0x34, 0x00, 0x00, 0x00, 0x88, 0xEE };
            set_reportcycle[4] = (byte)(numericUpDown_report.Value / 100);
            set_reportcycle[5] = (byte)(numericUpDown_report.Value % 100 / 10);
            set_reportcycle[6] = (byte)(numericUpDown_report.Value % 10);
            this.Invoke(new MethodInvoker(delegate
            {
                bool send;
                send = netTCPServer1.SendData(set_reportcycle);
                if (send == true)
                    MessageBox.Show("定时上报设置成功");
                else
                    MessageBox.Show("定时上报设置失败");
                //textBox_redis.AppendText("查询周期设置成功      " + DateTime.Now.ToString() + "\r\n");
            }));
        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {
            
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown_tmp2_ValueChanged(object sender, EventArgs e)
        {
            //if (numericupdown_tmp2.value > 99 || numericupdown_tmp2.value < -55)
            //{
            //    messagebox.show("请输入-55到99之间的数");
            //    return;
            //}
        }

        private void btn_threshold_config_Click(object sender, EventArgs e)
        {
            if (numericUpDown_tmp.Value > 99 || numericUpDown_tmp.Value < -55)
            {
                MessageBox.Show("温度1设置错误：请输入-55到99之间的数");
                return;
            }else if (numericUpDown_tmp2.Value > 99 || numericUpDown_tmp2.Value < -55)
            {
                MessageBox.Show("温度2设置错误：请输入-55到99之间的数");
                return;
            }
            else if (numericUpDown_hum.Value > 99 || numericUpDown_hum.Value < 20)
            {
                MessageBox.Show("湿度设置错误：请输入20到99之间的数");
                return;
            }
            else {
                MessageBox.Show("设置成功");
            } 
        }

        private void tebcontrol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tebcontrol.SelectedTab.Name == "tabPage_opdev")
            {
                comboxArea1.Items.Clear();
                string[] Ip_Area = new string[2];
                if (netTCPServer1.lstConn.Items.Count <= 0) return;
                CollectTCPClient selClient_first = (CollectTCPClient)netTCPServer1.lstConn.Items[0];
                Ip_Area = selClient_first.Name.Split('-');
                comboxArea1.Text = Ip_Area[1];
                int count = netTCPServer1.lstConn.Items.Count;
                if (netTCPServer1.lstConn.Items.Count <= 0) return;

                for (int i = 0; i < count; i++)
                {
                    CollectTCPClient selClient_index = (CollectTCPClient)netTCPServer1.lstConn.Items[i];
                    Ip_Area = selClient_index.Name.Split('-');
                    comboxArea1.Items.Add(Ip_Area[1]);
                    Ip_Area[0] = null;
                    Ip_Area[1] = null;
                }
            }
            if(tebcontrol.SelectedTab.Name == "tabPage_attn")
            {
                List<string> AreaList = new List<string>();
                CMBoxJobNumSeach.Items.Clear();
                CMBoxAreaSeach.Items.Clear();
                OleDbCommand cmdJobNum = new OleDbCommand("select 工号 from Insert_Info_Map",conn);
                OleDbDataReader reader = cmdJobNum.ExecuteReader();
                while(reader.Read())
                {
                    string jobNum = Convert.ToString(reader[0]);
                    CMBoxJobNumSeach.Items.Add(jobNum);
                    jobNum = " ";
                }
                cmdJobNum.Dispose();
                reader.Close();
                try
                {
                    StreamReader fileReader = new StreamReader("addr_ip.rng", Encoding.Default);
                    string line;
                    while((line = fileReader.ReadLine()) != null)
                    {
                        string[] s = line.Split('-');
                        if (s[1] == "" || s[1] == "***") continue;
                        string resultstr = AreaList.Find(
                            delegate(string str)
                            {
                                return str == s[1];
                            }); 
                        if(resultstr == null)
                        {
                            CMBoxAreaSeach.Items.Add(s[1]);
                            AreaList.Add(s[1]);
                        }
                    }
                    fileReader.Close();
                }
                catch { }

            }
        }

        /*
         * NOTE : 为老机箱设置IP添加新的按钮
         * DATE ：2018-09-10 22:34:25
         * AUTHOR ：Kabuto
         */
        private void btn_setoldIP_Click(object sender, EventArgs e)
        {
            int i, port;
            byte[] lan = new byte[60];
            lan[0] = 0xaa;
            lan[1] = 0xdd;
            lan[2] = 52;
            lan[3] = 0x36;
            try
            {

                string[] ip = textBox_ipaddr.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[i * 3 + 4] = (byte)(Convert.ToInt32(ip[i]) / 100);
                    lan[i * 3 + 5] = (byte)(Convert.ToInt32(ip[i]) % 100 / 10);
                    lan[i * 3 + 6] = (byte)(Convert.ToInt32(ip[i]) % 10);
                }
                string[] mask = textBox_mask.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[i * 3 + 16] = (byte)(Convert.ToInt32(mask[i]) / 100);
                    lan[i * 3 + 17] = (byte)(Convert.ToInt32(mask[i]) % 100 / 10);
                    lan[i * 3 + 18] = (byte)(Convert.ToInt32(mask[i]) % 10);
                }
                string[] def = textBox_gateway.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[28 + i * 3] = (byte)(Convert.ToInt32(def[i]) / 100);
                    lan[29 + i * 3] = (byte)(Convert.ToInt32(def[i]) % 100 / 10);
                    lan[30 + i * 3] = (byte)(Convert.ToInt32(def[i]) % 10);
                }
                string[] remote = textBox_severip.Text.Split('.');
                for (i = 0; i < 4; i++)
                {
                    lan[40 + i * 3] = (byte)(Convert.ToInt32(remote[i]) / 100);
                    lan[41 + i * 3] = (byte)(Convert.ToInt32(remote[i]) % 100 / 10);
                    lan[42 + i * 3] = (byte)(Convert.ToInt32(remote[i]) % 10);
                }

                port = Convert.ToInt32(textBox_severport.Text);
                lan[52] = (byte)(port / 1000);
                lan[53] = (byte)(port % 1000 / 100);
                lan[54] = (byte)(port % 1000 % 100 / 10);
                lan[55] = (byte)(port % 10);
            }
            catch { }

            //设置MessageBox事件响应
            DialogResult dr = MessageBox.Show("即将重新设置旧版机箱的IP，请确认所选中的设备是否为旧版机箱？", "设置旧版机箱IP", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                //点确定的代码，发送IP信息
                this.Invoke(new MethodInvoker(delegate
                {
                    bool send;
                    send = netTCPServer1.SendData(lan);
                    if(send == true)
                    {
                        MessageBox.Show("IP设置成功");
                    }
                    else
                    {
                        MessageBox.Show("IP设置失败");
                    }
                }));
            }
            else
            { //点取消的代码 
            }
        }

        private void button_relay3_Click(object sender, EventArgs e)
        {
            //远程复位交流接触器3
            byte[] relay3 = { 0xaa, 0xdd, 0x02, 0x48, 0x00, 0x00, 0x00, 0x88, 0xEE };    //为了统一老版本，新版本机箱更改了复位协议
            if(netTCPServer1.lstConn.SelectedItems.Count == 1)
            {
                timer_relay.Enabled = true;
                timer_relay.Interval = 40000;//定时40s
                bool Bool_relay3 = remote_relay_deal(relay3);
                if (Bool_relay3 == true)
                {
                    label_reset_relay3.Text = "复位中...";
                    relay3_state = true;
                }
            }
            else
            {
                MessageBox.Show("请选中一台设备", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Thread.Sleep(1000);
        }

        private void button_relay4_Click(object sender, EventArgs e)
        {
            //远程复位交流接触器4
            byte[] relay4 = { 0xaa, 0xdd, 0x02, 0x49, 0x00, 0x00, 0x00, 0x88, 0xEE };    //为了统一老版本，新版本机箱更改了复位协议
            if (netTCPServer1.lstConn.SelectedItems.Count == 1)
            {
                timer_relay.Enabled = true;
                timer_relay.Interval = 40000;//定时40s
                bool Bool_relay4 = remote_relay_deal(relay4);
                if (Bool_relay4 == true)
                {
                    label_reset_relay4.Text = "复位中...";
                    relay4_state = true;
                }
            }
            else
            {
                MessageBox.Show("请选中一台设备", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Thread.Sleep(1000);
        }

        private void button_readdate_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            string[] time1 = new string[2];
            string[] time2 = new string[2];

            int sei = 0;
            time2 = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss").Split(' ');
            DataSet Dc_sign_data = new DataSet();
            OleDbDataAdapter Ole_sign_data = new OleDbDataAdapter("select * from Insert_Info_Map2", conn);
            Ole_sign_data.Fill(Dc_sign_data);
            int timeSameFlag = 0;
            for (int i = 0;i<Dc_sign_data.Tables[0].Rows.Count;i++)
            {
                time1 = Dc_sign_data.Tables[0].Rows[i]["时间"].ToString().Split(' ');
                //time2 = dateTimePicker2.Value.ToString().Split(' ');
                if(time1[0] == time2[0])
                {
                    timeSameFlag = 1;
                    
                    ListViewItem item = new ListViewItem();
                    item.Text = (++sei).ToString();
                    item.SubItems.Add(Dc_sign_data.Tables[0].Rows[i]["工号"].ToString());
                    item.SubItems.Add(Dc_sign_data.Tables[0].Rows[i]["姓名"].ToString());
                    item.SubItems.Add(Dc_sign_data.Tables[0].Rows[i]["地点"].ToString());
                    item.SubItems.Add(Dc_sign_data.Tables[0].Rows[i]["时间"].ToString());
                    listView2.Items.Add(item);
                }
            }
            if(timeSameFlag == 0)
            {
                MessageBox.Show("查询时间不正确，请重新选择", "错误");
            }
            timeSameFlag = 0;
            Dc_sign_data.Reset();
        }

        private void button_readid_Click(object sender, EventArgs e)
        {
            int sei = 0;
            bool idSameFlag = false;
            if (CMBoxJobNumSeach.Text == "")
            {
                MessageBox.Show("查询工号为空，请选择工号", "错误");
                return;
            }
            listView2.Items.Clear();
            DataSet Dc_sign_id = new DataSet();
            //OleDbCommand ole_sign_id = new OleDbCommand()
            OleDbDataAdapter Ole_sign_id = new OleDbDataAdapter("select * from Insert_Info_Map2", conn);
            Ole_sign_id.Fill(Dc_sign_id);
            for(int i = 0;i<Dc_sign_id.Tables[0].Rows.Count;i++)
            {
                if(Dc_sign_id.Tables[0].Rows[i]["工号"].ToString() == CMBoxJobNumSeach.Text)
                {
                    idSameFlag = true;
                    ListViewItem item = new ListViewItem();
                    item.Text = (++sei).ToString();
                    item.SubItems.Add(Dc_sign_id.Tables[0].Rows[i]["工号"].ToString());
                    item.SubItems.Add(Dc_sign_id.Tables[0].Rows[i]["姓名"].ToString());
                    item.SubItems.Add(Dc_sign_id.Tables[0].Rows[i]["地点"].ToString());
                    item.SubItems.Add(Dc_sign_id.Tables[0].Rows[i]["时间"].ToString());
                    listView2.Items.Add(item);
                }
            }
            if(idSameFlag == false)
            {
                MessageBox.Show("没有查询到该工号下的打卡记录");
            }   
            Dc_sign_id.Reset();
            idSameFlag = false;
        }

        private void BasePanel_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;
        }

        private void label_reset_relay4_Click(object sender, EventArgs e)
        {

        }

        private void timer_relay_Tick(object sender, EventArgs e)
        {
            label_reset_relay1.Text = "启动复位";
            label_reset_relay2.Text = "启动复位";
            label_reset_relay3.Text = "启动复位";
            label_reset_relay4.Text = "启动复位";

            if (relay1_state == true)    //如果40s没有收到数据，就认为复位成功
            {
                relay1_state = false;
                MessageBox.Show("交流接触器1复位成功!!!", "正确", MessageBoxButtons.OK);
            }
            else if (relay2_state == true)
            {
                relay2_state = false;
                //MessageBox.Show("交流接触器2复位成功!!!");
                MessageBox.Show("交流接触器2复位成功!!!", "正确", MessageBoxButtons.OK);
            }
            else if (relay3_state == true)
            {
                relay3_state = false;
                MessageBox.Show("交流接触器3复位成功!!!", "正确", MessageBoxButtons.OK);
            }
            else if (relay4_state == true)
            {
                relay4_state = false;
                MessageBox.Show("交流接触器4复位成功!!!", "正确", MessageBoxButtons.OK);
            }
            timer_relay.Enabled = false;
        }

        private void udP_Radio1_Load_1(object sender, EventArgs e)
        {

        }

        private void button_offline_search_Click(object sender, EventArgs e)
        {
            listView_offline.Items.Clear();

            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select ip,addr,[time] from offline_table";
            // conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            int sei = 0;
            while (dr.Read())
            {
                string[] time1 = new string[2];
                string[] time2 = new string[2];
                time1 = dr[dr.FieldCount - 1].ToString().Split(' ');
                time2 = dateTimePicker4offlineSearch.Value.ToString().Split(' ');
                Debug.Print("time1 = " + time1[0] + "time2 = " + time2[0]);
                Debug.Print("time1 = " + time1[0].Length + "  time2 = " + time2[0].Length);
                if (time1[0] == time2[0])
                {
                    
                    string[] str = new string[5];
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        // textBox_redis.AppendText(dr[i].ToString() + "  ");
                        str[i] = dr[i].ToString();
                    }
                    ListViewItem item = new ListViewItem((++sei).ToString());
                    item.SubItems.AddRange(str);
                    listView_offline.Items.Add(item);
                    if (sei >= 300) {
                        break;
                    }
                }

            }
            cmd.Dispose();
        }

        private void button_offline_export_Click(object sender, EventArgs e)
        {
            ExportToExecl(listView_offline);
        }
         
         

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "CREATE TABLE Insert_Info_Map2(" +

                "工号 TEXT(20)," +
                "密码 TEXT(20)," +
                "姓名 TEXT(20)," +
                "电话 TEXT(20)," +
                "邮箱 TEXT(20)," +
                "地点 TEXT(20)," +
                "时间 TEXT(20)" +
                ")";
            try
            {
                OleDbCommand comm = new OleDbCommand(sql, conn);
                comm.ExecuteNonQuery();
                // conn.Close();
            }
            catch { }
        }
        */
         
        
       
    }
}
