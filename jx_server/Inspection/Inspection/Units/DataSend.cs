using System;
using System.Text;
using System.Windows.Forms;
using DataCollect.Lib;
using System.Threading;

namespace DataCollect.Units
{
    public partial class DataSend : UserControl
    {
       // BindingList<Model.CMD> lstCMD = new BindingList<Model.CMD>();

        public event CollectEvent.DataSendHandler EventDataSend;

        /// <summary>
        /// 是否在自动循环发送状态
        /// </summary>
        bool AutoSend = false;

        public DataSend()
        {
            InitializeComponent();

        }

 

        private void btnAutoSend_Click(object sender, EventArgs e)
        {
            if (AutoSend == false)
            {
                btnAutoSend.Text = "停止循环";
                textBox_send.Enabled = true;
                nmDelay.Enabled = false;
                AutoSend = true;
                Thread ThTestL = new Thread(new ParameterizedThreadStart(TAutoSend));
                ThTestL.IsBackground = true;
                ThTestL.Start(nmDelay.Value);
            }
            else
            {
                StopAutoSend();
            }
        }

        /// <summary>
        /// 自动发送命令线程
        /// </summary>
        private void TAutoSend(object Interval)
        {
            try
            {
                object sendlock = new object();
                int SendInterval = Convert.ToInt32(Interval);
                while (AutoSend)
                {
                   
                        if (AutoSend)
                        {
                            this.Invoke(new MethodInvoker(delegate
                            {
                               
                                    if (EventDataSend != null)
                                    {
                                        if ( EventDataSend(Encoding.Default.GetBytes(textBox_send.Text)) == false)
                                        {
                                            StopAutoSend();
                                        }
                                    }
                                
                            }));
                            Thread.Sleep(SendInterval);
                        }
                        else
                        {
                            break;
                        }
                    
                }
            }
            catch { };
        }

        /// <summary>
        /// 停止循环发送
        /// </summary>
        private void StopAutoSend()
        {
            AutoSend = false;
            btnAutoSend.Text = "循环发送";
            textBox_send.Enabled = true;
            nmDelay.Enabled = true;
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            if (EventDataSend != null)
                if (radioButton_sASCII.Checked == true)
                    EventDataSend(Encoding.Default.GetBytes(textBox_send.Text));
                else
                {
                    if (radioButton_shex.Checked == true)
                    {
                        string sd = textBox_send.Text;
                        String[] str =sd.Split(' ');
                        Byte[] byt = new byte[str.Length];
                        for (int i = 0; i < str.Length; i++)
                        {
                            int s = Convert.ToInt32(str[i], 16);

                            byt[i] = (byte)s;

                        }
                        EventDataSend(byt);
                    }

                }

        }

        private void textBox_send_TextChanged(object sender, EventArgs e)
        {

        }

        private void DataSend_Load(object sender, EventArgs e)
        {

        }

        private void nmDelay_ValueChanged(object sender, EventArgs e)
        {

        }

        private void radioButton_shex_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button_sclean_Click(object sender, EventArgs e)
        {
            textBox_send.Clear();
        }
    }
}
