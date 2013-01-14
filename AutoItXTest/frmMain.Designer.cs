namespace AutoitxTest
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.button1 = new System.Windows.Forms.Button();
            this.lblKeys = new System.Windows.Forms.Label();
            this.lblHotkey = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 41);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test Script";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblKeys
            // 
            this.lblKeys.Location = new System.Drawing.Point(12, 238);
            this.lblKeys.Name = "lblKeys";
            this.lblKeys.Size = new System.Drawing.Size(264, 136);
            this.lblKeys.TabIndex = 1;
            this.lblKeys.Text = "Keys: ";
            // 
            // lblHotkey
            // 
            this.lblHotkey.AutoSize = true;
            this.lblHotkey.Location = new System.Drawing.Point(12, 214);
            this.lblHotkey.Name = "lblHotkey";
            this.lblHotkey.Size = new System.Drawing.Size(46, 13);
            this.lblHotkey.TabIndex = 2;
            this.lblHotkey.Text = "Hotkeys";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 59);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(374, 134);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 383);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblHotkey);
            this.Controls.Add(this.lblKeys);
            this.Controls.Add(this.button1);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblKeys;
        private System.Windows.Forms.Label lblHotkey;
        private System.Windows.Forms.TextBox textBox1;
    }
}

