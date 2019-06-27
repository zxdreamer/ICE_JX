using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Inspection
{
    public partial class Inspect : Form
    {
        Form flog = null;
        public Inspect()
        {
            //flog = login;
            InitializeComponent();
            //if (name == "wdzy")
            //{
            //   // basePanel1.groupBox4.Enabled = true;
            //   // basePanel1.groupBox5.Enabled = true;
            ////    basePanel1.groupBox6.Enabled = true;
            //    label_usename.Text = "管理员：";
            //}
            //else
            //{
            //  //  basePanel1.groupBox4.Enabled = false;
            //    //basePanel1.groupBox5.Enabled = false;
            //  //  basePanel1.groupBox6.Enabled = false;
            //    label_usename.Text = "用户：";
            //}
            //linkLabel1.Text = name;
            this.skinEngine1.SkinFile = "EmeraldColor1.ssk";
        }
        private void Inspect_FormClosed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Point screenPoint = Control.MousePosition;
            //contextMenuStrip1.Show(screenPoint);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void basePanel1_Load(object sender, EventArgs e)
        {

        }

        //private void 注销ToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void basePanel1_Load_1(object sender, EventArgs e)
        {

        }

    }
}
