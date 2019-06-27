using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DataCollect.Lib;
namespace DataCollect.Units
{

    public partial class TabDataReceive : UserControl
    {
      
        List<Units.DataReceive> lstDataReceive = new List<DataReceive>();
       // NetTCPServer tcpserver = new NetTCPServer();
        
        public TabDataReceive()
        {
            InitializeComponent();
            tabData.ContextMenuStrip = CMenu;
        }

        private Units.DataReceive AddNewDataReceive(string SourceName)
        {
            TabPage tpage = new TabPage();
            tpage.Disposed += new EventHandler(tpage_Disposed);
            tpage.Text = SourceName;
            Units.DataReceive UDataReceive = new DataReceive();
            UDataReceive.Name = SourceName;
            tpage.Controls.Add(UDataReceive);
            UDataReceive.Dock = DockStyle.Fill;
            lstDataReceive.Add(UDataReceive);
            this.Invoke(new MethodInvoker(delegate
            {
                tabData.TabPages.Add(tpage);
            }));
            return UDataReceive;
        }

        void tpage_Disposed(object sender, EventArgs e)
        {
            TabPage tpage = (TabPage)sender;
            lstDataReceive.Remove(lstDataReceive.Find(p => p.Name == tpage.Text));
        }

        #region 公有方法
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="SourceName">来源名称</param>
        /// <param name="data">单个字节数据</param>
        public void AddData(string SourceName, byte data)
        {
            Units.DataReceive UDataReceive = lstDataReceive.Find(p => p.Name == SourceName);
            if (UDataReceive == null)
            {
               UDataReceive = AddNewDataReceive(SourceName);
            }
            UDataReceive.AddData(data);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="SourceName">来源名称</param>
        /// <param name="data">字节数组</param>
        public void AddData(string SourceName, byte[] data)
        {
            Units.DataReceive UDataReceive = lstDataReceive.Find(p => p.Name == SourceName);
            if (UDataReceive == null)
            {
                UDataReceive = AddNewDataReceive(SourceName);
            }
            UDataReceive.AddData(data);
    
        }
        #endregion

        #region Tab页标题快捷菜单
        private void tabData_DoubleClick(object sender, EventArgs e)
        {
            this.tabData.SelectedTab.Dispose();
        }

        private void MS_Close_Click(object sender, EventArgs e)
        {
            this.tabData.SelectedTab.Dispose();
        }

        private void MS_CloseALL_Click(object sender, EventArgs e)
        {
            foreach(TabPage tpage in this.tabData.TabPages)
            {
                tpage.Dispose();
            }
        }

        private void MS_CloseOthers_Click(object sender, EventArgs e)
        {
            foreach (TabPage tpage in this.tabData.TabPages)
            {
                if (tpage != this.tabData.SelectedTab)
                {
                    tpage.Dispose();
                }
            }
        }
        #endregion

        private void tabData_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CMenu_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
