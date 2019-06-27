namespace DataCollect.Units
{
    partial class DataSend
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
            this.btnAutoSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nmDelay = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_send = new System.Windows.Forms.TextBox();
            this.button_send = new System.Windows.Forms.Button();
            this.button_sclean = new System.Windows.Forms.Button();
            this.radioButton_shex = new System.Windows.Forms.RadioButton();
            this.radioButton_sASCII = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nmDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAutoSend
            // 
            this.btnAutoSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAutoSend.Location = new System.Drawing.Point(3, 155);
            this.btnAutoSend.Name = "btnAutoSend";
            this.btnAutoSend.Size = new System.Drawing.Size(77, 33);
            this.btnAutoSend.TabIndex = 1;
            this.btnAutoSend.Text = "循环发送";
            this.btnAutoSend.UseVisualStyleBackColor = true;
            this.btnAutoSend.Click += new System.EventHandler(this.btnAutoSend_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 162);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "发送间隔：";
            // 
            // nmDelay
            // 
            this.nmDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nmDelay.Location = new System.Drawing.Point(154, 160);
            this.nmDelay.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nmDelay.Name = "nmDelay";
            this.nmDelay.Size = new System.Drawing.Size(70, 21);
            this.nmDelay.TabIndex = 3;
            this.nmDelay.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nmDelay.ValueChanged += new System.EventHandler(this.nmDelay_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(230, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "ms";
            // 
            // textBox_send
            // 
            this.textBox_send.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_send.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_send.Location = new System.Drawing.Point(3, 27);
            this.textBox_send.Multiline = true;
            this.textBox_send.Name = "textBox_send";
            this.textBox_send.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_send.Size = new System.Drawing.Size(406, 122);
            this.textBox_send.TabIndex = 5;
            this.textBox_send.TextChanged += new System.EventHandler(this.textBox_send_TextChanged);
            // 
            // button_send
            // 
            this.button_send.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_send.Location = new System.Drawing.Point(253, 157);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(75, 31);
            this.button_send.TabIndex = 6;
            this.button_send.Text = "发送";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_sclean
            // 
            this.button_sclean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_sclean.Location = new System.Drawing.Point(334, 157);
            this.button_sclean.Name = "button_sclean";
            this.button_sclean.Size = new System.Drawing.Size(75, 31);
            this.button_sclean.TabIndex = 7;
            this.button_sclean.Text = "清空发送";
            this.button_sclean.UseVisualStyleBackColor = true;
            this.button_sclean.Click += new System.EventHandler(this.button_sclean_Click);
            // 
            // radioButton_shex
            // 
            this.radioButton_shex.AutoSize = true;
            this.radioButton_shex.Checked = true;
            this.radioButton_shex.Location = new System.Drawing.Point(93, 6);
            this.radioButton_shex.Name = "radioButton_shex";
            this.radioButton_shex.Size = new System.Drawing.Size(41, 16);
            this.radioButton_shex.TabIndex = 8;
            this.radioButton_shex.TabStop = true;
            this.radioButton_shex.Text = "Hex";
            this.radioButton_shex.UseVisualStyleBackColor = true;
            this.radioButton_shex.CheckedChanged += new System.EventHandler(this.radioButton_shex_CheckedChanged);
            // 
            // radioButton_sASCII
            // 
            this.radioButton_sASCII.AutoSize = true;
            this.radioButton_sASCII.Location = new System.Drawing.Point(140, 6);
            this.radioButton_sASCII.Name = "radioButton_sASCII";
            this.radioButton_sASCII.Size = new System.Drawing.Size(53, 16);
            this.radioButton_sASCII.TabIndex = 9;
            this.radioButton_sASCII.Text = "ASCII";
            this.radioButton_sASCII.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "发送数据类型";
            // 
            // DataSend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.radioButton_sASCII);
            this.Controls.Add(this.radioButton_shex);
            this.Controls.Add(this.textBox_send);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nmDelay);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAutoSend);
            this.Controls.Add(this.button_sclean);
            this.Name = "DataSend";
            this.Size = new System.Drawing.Size(412, 188);
            this.Load += new System.EventHandler(this.DataSend_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nmDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAutoSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nmDelay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_send;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Button button_sclean;
        private System.Windows.Forms.RadioButton radioButton_shex;
        private System.Windows.Forms.RadioButton radioButton_sASCII;
        private System.Windows.Forms.Label label3;
    }
}
