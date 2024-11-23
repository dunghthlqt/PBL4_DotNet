namespace PBL4_DotNet
{
    partial class Tools_Route
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRoute = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonStart = new System.Windows.Forms.Button();
            this.richTextBoxRoute = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(92, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Domain Name / IP Address";
            // 
            // textBoxRoute
            // 
            this.textBoxRoute.Location = new System.Drawing.Point(234, 44);
            this.textBoxRoute.Name = "textBoxRoute";
            this.textBoxRoute.Size = new System.Drawing.Size(166, 20);
            this.textBoxRoute.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(422, 44);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(59, 20);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // richTextBoxRoute
            // 
            this.richTextBoxRoute.Location = new System.Drawing.Point(95, 85);
            this.richTextBoxRoute.Name = "richTextBoxRoute";
            this.richTextBoxRoute.Size = new System.Drawing.Size(474, 237);
            this.richTextBoxRoute.TabIndex = 4;
            this.richTextBoxRoute.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(502, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 21);
            this.button1.TabIndex = 5;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Tools_Route
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBoxRoute);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.textBoxRoute);
            this.Controls.Add(this.label1);
            this.Name = "Tools_Route";
            this.Size = new System.Drawing.Size(802, 396);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRoute;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RichTextBox richTextBoxRoute;
        private System.Windows.Forms.Button button1;
    }
}
