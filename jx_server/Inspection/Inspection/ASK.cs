using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Inspection
{
    public partial class ASK : Form
    {
        public bool ok = false ;
        public ASK(string str)
        {
            InitializeComponent();
            label1.Text = str;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ok = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ok = false;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ok = true;
            this.Close();
        }
    }
}
