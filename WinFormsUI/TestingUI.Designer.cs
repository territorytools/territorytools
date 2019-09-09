namespace AlbaClient.WinFormsUI
{
    partial class TestingUI
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
            this.logonButton = new System.Windows.Forms.Button();
            this.testResults = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.loadTerritoriesButton = new System.Windows.Forms.Button();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.userBox = new System.Windows.Forms.TextBox();
            this.accountBox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.openKmlButton = new System.Windows.Forms.Button();
            this.openKmlFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logonButton
            // 
            this.logonButton.Location = new System.Drawing.Point(428, 84);
            this.logonButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.logonButton.Name = "logonButton";
            this.logonButton.Size = new System.Drawing.Size(150, 44);
            this.logonButton.TabIndex = 3;
            this.logonButton.Text = "Log On";
            this.logonButton.UseVisualStyleBackColor = true;
            this.logonButton.Click += new System.EventHandler(this.logonButton_Click);
            // 
            // testResults
            // 
            this.testResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testResults.Location = new System.Drawing.Point(6, 140);
            this.testResults.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.testResults.MinimumSize = new System.Drawing.Size(500, 500);
            this.testResults.Multiline = true;
            this.testResults.Name = "testResults";
            this.testResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.testResults.Size = new System.Drawing.Size(1348, 500);
            this.testResults.TabIndex = 7;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(590, 84);
            this.clearButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(150, 44);
            this.clearButton.TabIndex = 4;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // loadTerritoriesButton
            // 
            this.loadTerritoriesButton.Enabled = false;
            this.loadTerritoriesButton.Location = new System.Drawing.Point(752, 84);
            this.loadTerritoriesButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.loadTerritoriesButton.Name = "loadTerritoriesButton";
            this.loadTerritoriesButton.Size = new System.Drawing.Size(266, 44);
            this.loadTerritoriesButton.TabIndex = 5;
            this.loadTerritoriesButton.Text = "Download Territories";
            this.loadTerritoriesButton.UseVisualStyleBackColor = true;
            this.loadTerritoriesButton.Click += new System.EventHandler(this.loadTerritoriesButton_Click);
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(118, 82);
            this.passwordBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(300, 31);
            this.passwordBox.TabIndex = 2;
            this.passwordBox.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "User";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Account";
            // 
            // userBox
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.userBox, true);
            this.userBox.Location = new System.Drawing.Point(69, 43);
            this.userBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.userBox.Name = "userBox";
            this.userBox.Size = new System.Drawing.Size(300, 31);
            this.userBox.TabIndex = 1;
            // 
            // accountBox
            // 
            this.flowLayoutPanel1.SetFlowBreak(this.accountBox, true);
            this.accountBox.Location = new System.Drawing.Point(102, 4);
            this.accountBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.accountBox.Name = "accountBox";
            this.accountBox.Size = new System.Drawing.Size(300, 31);
            this.accountBox.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.accountBox);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.userBox);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.passwordBox);
            this.flowLayoutPanel1.Controls.Add(this.logonButton);
            this.flowLayoutPanel1.Controls.Add(this.clearButton);
            this.flowLayoutPanel1.Controls.Add(this.loadTerritoriesButton);
            this.flowLayoutPanel1.Controls.Add(this.openKmlButton);
            this.flowLayoutPanel1.Controls.Add(this.testResults);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1406, 700);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // openKmlButton
            // 
            this.openKmlButton.Enabled = false;
            this.openKmlButton.Location = new System.Drawing.Point(1030, 84);
            this.openKmlButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.openKmlButton.Name = "openKmlButton";
            this.openKmlButton.Size = new System.Drawing.Size(212, 44);
            this.openKmlButton.TabIndex = 10;
            this.openKmlButton.Text = "Upload KML File";
            this.openKmlButton.UseVisualStyleBackColor = true;
            this.openKmlButton.Click += new System.EventHandler(this.openKmlButton_Click);
            // 
            // openKmlFileDialog
            // 
            this.openKmlFileDialog.Filter = "Google Maps KML Files|*.kml";
            // 
            // TestingUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1406, 700);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "TestingUI";
            this.Text = "Alba Client Tester";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button logonButton;
        private System.Windows.Forms.TextBox testResults;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button loadTerritoriesButton;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox userBox;
        private System.Windows.Forms.TextBox accountBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button openKmlButton;
        private System.Windows.Forms.OpenFileDialog openKmlFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

