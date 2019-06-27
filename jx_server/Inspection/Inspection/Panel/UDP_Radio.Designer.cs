namespace Inspection.Panel
{
    partial class UDP_Radio
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.offline_warning_tb = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // offline_warning_tb
            // 
            this.offline_warning_tb.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.offline_warning_tb.Location = new System.Drawing.Point(3, 4);
            this.offline_warning_tb.Multiline = true;
            this.offline_warning_tb.Name = "offline_warning_tb";
            this.offline_warning_tb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.offline_warning_tb.Size = new System.Drawing.Size(180, 106);
            this.offline_warning_tb.TabIndex = 0;
            // 
            // UDP_Radio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.offline_warning_tb);
            this.Name = "UDP_Radio";
            this.Size = new System.Drawing.Size(186, 128);
            this.Load += new System.EventHandler(this.UDP_Radio_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox offline_warning_tb;

    }
}
