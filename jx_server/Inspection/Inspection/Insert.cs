using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DataCollect.Lib;
namespace Inspection
{
    public partial class Insert : Form
    {
        //public event DataCollect.Lib.CollectEvent.Inquiry_InsertInfoHandler Inquiry_InsertInfo;
        //public event DataCollect.Lib.CollectEvent.Delete_InsertInfoHandler Delete_InsertInfo;
        
        public Insert(string str)
        {
            InitializeComponent();
            this.Text = str;
            ok = false;
            textBox_worknum.Text = "";
            textBox_password.Text = "";
            textBox_name.Text = "";
            textBox_email.Text = "";
            textBox_phone.Text = "";
            textBox_worknum.Focus();
        }
        public bool ok = false;
        

        private void button_yes_Click_1(object sender, EventArgs e)
        {
            ok = true;
            this.Close();
        }

        private void button_no_Click(object sender, EventArgs e)
        {
            ok = false;
            this.Close();
        }
        static string mdb = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=db_col.mdb"; //根据自己的设置
        static OleDbConnection conn = new OleDbConnection(mdb);

        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open();
           // Inquiry_InsertInfo();
            listView1.Items.Clear();
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Insert_Info_Map";
            // conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            int num = 0;
            while (dr.Read())
            {
                string[] str = new string[10];
                str[0] = dr[0].ToString();

                for (int i = 1; i < dr.FieldCount - 1; i++)
                {
                    // textBox_redis.AppendText(dr[i].ToString() + "  ");
                    str[i] = dr[i + 1].ToString();
                }

                ListViewItem item = new ListViewItem((++num).ToString());
                item.SubItems.AddRange(str);
                listView1.Items.Add(item);
            }
            cmd.Dispose();

            conn.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
           // Delete_InsertInfo();
            ASK ask = new ASK("将删除所有录入信息，确定删除？");
            ask.ShowDialog();
            if (ask.ok == true)
            {
                OleDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = "delete from Insert_Info_Map";
                cmd.ExecuteNonQuery();
                listView1.Items.Clear();
            }
            
            conn.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void Insert_Load(object sender, EventArgs e)
        {

        }
    }
}
