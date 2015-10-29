namespace DominoTable13
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.labelx = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.inputN = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.inputK = new System.Windows.Forms.NumericUpDown();
            this.chkShow = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.InputThreadNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAbort = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.inputN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputK)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InputThreadNum)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(395, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Show the dominoes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(4, 78);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(583, 312);
            this.textBox1.TabIndex = 1;
            this.textBox1.WordWrap = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(4, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Solve";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // labelx
            // 
            this.labelx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelx.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelx.Location = new System.Drawing.Point(4, 393);
            this.labelx.Name = "labelx";
            this.labelx.Size = new System.Drawing.Size(583, 20);
            this.labelx.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Sum";
            // 
            // inputN
            // 
            this.inputN.Location = new System.Drawing.Point(38, 3);
            this.inputN.Maximum = new decimal(new int[] {
            49,
            0,
            0,
            0});
            this.inputN.Name = "inputN";
            this.inputN.Size = new System.Drawing.Size(54, 20);
            this.inputN.TabIndex = 9;
            this.inputN.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Height mesured in dice";
            // 
            // inputK
            // 
            this.inputK.Location = new System.Drawing.Point(268, 3);
            this.inputK.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.inputK.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputK.Name = "inputK";
            this.inputK.Size = new System.Drawing.Size(74, 20);
            this.inputK.TabIndex = 12;
            this.inputK.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // chkShow
            // 
            this.chkShow.AutoSize = true;
            this.chkShow.Location = new System.Drawing.Point(490, 6);
            this.chkShow.Name = "chkShow";
            this.chkShow.Size = new System.Drawing.Size(86, 17);
            this.chkShow.TabIndex = 13;
            this.chkShow.Text = "Show debug";
            this.chkShow.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.InputThreadNum);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.inputN);
            this.panel1.Controls.Add(this.chkShow);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.inputK);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(4, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(583, 31);
            this.panel1.TabIndex = 15;
            // 
            // InputThreadNum
            // 
            this.InputThreadNum.Location = new System.Drawing.Point(446, 3);
            this.InputThreadNum.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.InputThreadNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.InputThreadNum.Name = "InputThreadNum";
            this.InputThreadNum.Size = new System.Drawing.Size(38, 20);
            this.InputThreadNum.TabIndex = 17;
            this.InputThreadNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(357, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Num. of threads";
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(86, 13);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 16;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 420);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelx);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Magic square from a dominoe";
            ((System.ComponentModel.ISupportInitialize)(this.inputN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputK)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InputThreadNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown inputN;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown inputK;
        private System.Windows.Forms.CheckBox chkShow;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown InputThreadNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAbort;
    }
}

