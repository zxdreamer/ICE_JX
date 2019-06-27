using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Inspection
{
    public partial class Load : Form
    {
        public Load()
        {
            InitializeComponent();
        }
        public bool ok = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text.Trim() == "admin") && (textBox2.Text == "123"))
            {
                ok = true;
                this.Hide();

            }
            else
            {
                ok = false;
                MessageBox.Show("用户名/密码错误！","提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ok = false;
            this.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender,e);
            }
        }
    }
}
