namespace DataCollect.Units
{
    partial class NetTCPServer
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lstConn = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MS_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_band = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_addp = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstConn
            // 
            this.lstConn.BackColor = System.Drawing.SystemColors.Window;
            this.lstConn.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstConn.ContextMenuStrip = this.contextMenuStrip1;
            this.lstConn.DisplayMember = "Name";
            this.lstConn.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstConn.ItemHeight = 14;
            this.lstConn.Location = new System.Drawing.Point(5, 47);
            this.lstConn.Name = "lstConn";
            this.lstConn.Size = new System.Drawing.Size(197, 476);
            this.lstConn.TabIndex = 11;
            this.lstConn.SelectedIndexChanged += new System.EventHandler(this.lstConn_SelectedIndexChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MS_Delete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // MS_Delete
            // 
            this.MS_Delete.Name = "MS_Delete";
            this.MS_Delete.Size = new System.Drawing.Size(124, 22);
            this.MS_Delete.Text = "断开连接";
            this.MS_Delete.Click += new System.EventHandler(this.MS_Delete_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_band);
            this.groupBox2.Controls.Add(this.lstConn);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBox_addp);
            this.groupBox2.Location = new System.Drawing.Point(0, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 532);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "设备列表";
            // 
            // button_band
            // 
            this.button_band.Location = new System.Drawing.Point(132, 18);
            this.button_band.Name = "button_band";
            this.button_band.Size = new System.Drawing.Size(75, 23);
            this.button_band.TabIndex = 21;
            this.button_band.Text = "绑定地名";
            this.button_band.UseVisualStyleBackColor = true;
            this.button_band.Click += new System.EventHandler(this.button_band_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 273);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "服务器IP";
            this.label2.Visible = false;
            // 
            // textBox_addp
            // 
            this.textBox_addp.Location = new System.Drawing.Point(6, 20);
            this.textBox_addp.Name = "textBox_addp";
            this.textBox_addp.Size = new System.Drawing.Size(120, 21);
            this.textBox_addp.TabIndex = 19;
            this.textBox_addp.TextChanged += new System.EventHandler(this.textBox_addp_TextChanged);
            // 
            // NetTCPServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Location = new System.Drawing.Point(6, 10);
            this.Name = "NetTCPServer";
            this.Size = new System.Drawing.Size(217, 538);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MS_Delete;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ListBox lstConn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_band;
        private System.Windows.Forms.TextBox textBox_addp;
    }
}
