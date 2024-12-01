namespace PBL4_DotNet
{
    partial class Tools_Ping
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxPing = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.richTextBoxPing = new System.Windows.Forms.RichTextBox();
            this.checkBoxContinuousPing = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxPing
            // 
            this.textBoxPing.Location = new System.Drawing.Point(196, 39);
            this.textBoxPing.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPing.Name = "textBoxPing";
            this.textBoxPing.Size = new System.Drawing.Size(144, 20);
            this.textBoxPing.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Domain Name / IP Address";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(366, 38);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(56, 21);
            this.button1.TabIndex = 2;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(454, 38);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 21);
            this.button2.TabIndex = 4;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBoxPing
            // 
            this.richTextBoxPing.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxPing.Location = new System.Drawing.Point(59, 89);
            this.richTextBoxPing.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBoxPing.Name = "richTextBoxPing";
            this.richTextBoxPing.Size = new System.Drawing.Size(452, 257);
            this.richTextBoxPing.TabIndex = 5;
            this.richTextBoxPing.Text = "";
            // 
            // checkBoxContinuousPing
            // 
            this.checkBoxContinuousPing.AutoSize = true;
            this.checkBoxContinuousPing.Location = new System.Drawing.Point(196, 62);
            this.checkBoxContinuousPing.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxContinuousPing.Name = "checkBoxContinuousPing";
            this.checkBoxContinuousPing.Size = new System.Drawing.Size(100, 17);
            this.checkBoxContinuousPing.TabIndex = 6;
            this.checkBoxContinuousPing.Text = "ContinuousPing";
            this.checkBoxContinuousPing.UseVisualStyleBackColor = true;
            // 
            // Tools_Ping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.checkBoxContinuousPing);
            this.Controls.Add(this.richTextBoxPing);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPing);
            this.Name = "Tools_Ping";
            this.Size = new System.Drawing.Size(802, 396);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox richTextBoxPing;
        private System.Windows.Forms.CheckBox checkBoxContinuousPing;
    }
}
