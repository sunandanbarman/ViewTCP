namespace ViewTCP
{
    partial class PropertiesForm
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
            this.imgApplication = new System.Windows.Forms.PictureBox();
            this.lblProcessDesc = new System.Windows.Forms.Label();
            this.lblPublisher = new System.Windows.Forms.Label();
            this.lblVersionValue = new System.Windows.Forms.Label();
            this.lblVersionText = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.edtFullPath = new System.Windows.Forms.TextBox();
            this.btnEndProcess = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgApplication)).BeginInit();
            this.SuspendLayout();
            // 
            // imgApplication
            // 
            this.imgApplication.Location = new System.Drawing.Point(22, 20);
            this.imgApplication.Name = "imgApplication";
            this.imgApplication.Size = new System.Drawing.Size(69, 60);
            this.imgApplication.TabIndex = 0;
            this.imgApplication.TabStop = false;
            // 
            // lblProcessDesc
            // 
            this.lblProcessDesc.AutoSize = true;
            this.lblProcessDesc.Location = new System.Drawing.Point(112, 20);
            this.lblProcessDesc.Name = "lblProcessDesc";
            this.lblProcessDesc.Size = new System.Drawing.Size(60, 13);
            this.lblProcessDesc.TabIndex = 1;
            this.lblProcessDesc.Text = "Description";
            // 
            // lblPublisher
            // 
            this.lblPublisher.AutoSize = true;
            this.lblPublisher.Location = new System.Drawing.Point(112, 52);
            this.lblPublisher.Name = "lblPublisher";
            this.lblPublisher.Size = new System.Drawing.Size(50, 13);
            this.lblPublisher.TabIndex = 2;
            this.lblPublisher.Text = "Publisher";
            // 
            // lblVersionValue
            // 
            this.lblVersionValue.AutoSize = true;
            this.lblVersionValue.Location = new System.Drawing.Point(112, 85);
            this.lblVersionValue.Name = "lblVersionValue";
            this.lblVersionValue.Size = new System.Drawing.Size(42, 13);
            this.lblVersionValue.TabIndex = 3;
            this.lblVersionValue.Text = "Version";
            // 
            // lblVersionText
            // 
            this.lblVersionText.AutoSize = true;
            this.lblVersionText.Location = new System.Drawing.Point(19, 85);
            this.lblVersionText.Name = "lblVersionText";
            this.lblVersionText.Size = new System.Drawing.Size(45, 13);
            this.lblVersionText.TabIndex = 4;
            this.lblVersionText.Text = "Version:";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(19, 115);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(29, 13);
            this.lblPath.TabIndex = 5;
            this.lblPath.Text = "Path";
            // 
            // edtFullPath
            // 
            this.edtFullPath.Location = new System.Drawing.Point(22, 140);
            this.edtFullPath.Name = "edtFullPath";
            this.edtFullPath.Size = new System.Drawing.Size(369, 20);
            this.edtFullPath.TabIndex = 6;
            // 
            // btnEndProcess
            // 
            this.btnEndProcess.Location = new System.Drawing.Point(315, 112);
            this.btnEndProcess.Name = "btnEndProcess";
            this.btnEndProcess.Size = new System.Drawing.Size(76, 22);
            this.btnEndProcess.TabIndex = 7;
            this.btnEndProcess.Text = "End process";
            this.btnEndProcess.UseVisualStyleBackColor = true;
            this.btnEndProcess.Click += new System.EventHandler(this.btnEndProcess_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(315, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 22);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 208);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnEndProcess);
            this.Controls.Add(this.edtFullPath);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.lblVersionText);
            this.Controls.Add(this.lblVersionValue);
            this.Controls.Add(this.lblPublisher);
            this.Controls.Add(this.lblProcessDesc);
            this.Controls.Add(this.imgApplication);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesForm";
            this.Text = "Properties";
            this.Load += new System.EventHandler(this.PropertiesForm_Load);
            this.Shown += new System.EventHandler(this.PropertiesForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PropertiesForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.imgApplication)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgApplication;
        private System.Windows.Forms.Label lblProcessDesc;
        private System.Windows.Forms.Label lblPublisher;
        private System.Windows.Forms.Label lblVersionValue;
        private System.Windows.Forms.Label lblVersionText;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.TextBox edtFullPath;
        private System.Windows.Forms.Button btnEndProcess;
        private System.Windows.Forms.Button btnOK;
    }
}