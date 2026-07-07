namespace YoutubeVideosDownloaderPro
{
    partial class MainForm
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.downloadPlaylistButton = new System.Windows.Forms.Button();
            this.playlistUrlLabel = new System.Windows.Forms.Label();
            this.playlistUrlIconBox = new System.Windows.Forms.PictureBox();
            this.playlistUrlTextBox = new System.Windows.Forms.TextBox();
            this.playlistPictureBox = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.downloadVideoButton = new System.Windows.Forms.Button();
            this.videoUrlLabel = new System.Windows.Forms.Label();
            this.videoUrlIconBox = new System.Windows.Forms.PictureBox();
            this.videoUrlTextBox = new System.Windows.Forms.TextBox();
            this.videoPictureBox = new System.Windows.Forms.PictureBox();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.downloadPathLabel = new System.Windows.Forms.Label();
            this.downloadPathTextBox = new System.Windows.Forms.TextBox();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playlistUrlIconBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playlistPictureBox)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videoUrlIconBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabPage2.Controls.Add(this.downloadPlaylistButton);
            this.tabPage2.Controls.Add(this.playlistUrlLabel);
            this.tabPage2.Controls.Add(this.playlistUrlIconBox);
            this.tabPage2.Controls.Add(this.playlistUrlTextBox);
            this.tabPage2.Controls.Add(this.playlistPictureBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 35);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 411);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "قائمة تشغيل";
            // 
            // downloadPlaylistButton
            // 
            this.downloadPlaylistButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.downloadPlaylistButton.FlatAppearance.BorderSize = 0;
            this.downloadPlaylistButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadPlaylistButton.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadPlaylistButton.ForeColor = System.Drawing.Color.White;
            this.downloadPlaylistButton.Location = new System.Drawing.Point(285, 228);
            this.downloadPlaylistButton.Name = "downloadPlaylistButton";
            this.downloadPlaylistButton.Size = new System.Drawing.Size(226, 40);
            this.downloadPlaylistButton.TabIndex = 9;
            this.downloadPlaylistButton.Text = "تحميل";
            this.downloadPlaylistButton.UseVisualStyleBackColor = false;
            this.downloadPlaylistButton.Click += new System.EventHandler(this.downloadPlaylistButton_Click);
            // 
            // playlistUrlLabel
            // 
            this.playlistUrlLabel.AutoSize = true;
            this.playlistUrlLabel.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playlistUrlLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.playlistUrlLabel.Location = new System.Drawing.Point(628, 167);
            this.playlistUrlLabel.Name = "playlistUrlLabel";
            this.playlistUrlLabel.Size = new System.Drawing.Size(130, 25);
            this.playlistUrlLabel.TabIndex = 8;
            this.playlistUrlLabel.Text = "رابط المقطع";
            // 
            // playlistUrlIconBox
            // 
            this.playlistUrlIconBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.playlistUrlIconBox.Image = global::YoutubeVideosDownloaderPro.Properties.Resources.attachment;
            this.playlistUrlIconBox.Location = new System.Drawing.Point(172, 167);
            this.playlistUrlIconBox.Name = "playlistUrlIconBox";
            this.playlistUrlIconBox.Size = new System.Drawing.Size(28, 28);
            this.playlistUrlIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.playlistUrlIconBox.TabIndex = 7;
            this.playlistUrlIconBox.TabStop = false;
            // 
            // playlistUrlTextBox
            // 
            this.playlistUrlTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.playlistUrlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.playlistUrlTextBox.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playlistUrlTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.playlistUrlTextBox.Location = new System.Drawing.Point(206, 162);
            this.playlistUrlTextBox.Name = "playlistUrlTextBox";
            this.playlistUrlTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.playlistUrlTextBox.Size = new System.Drawing.Size(400, 33);
            this.playlistUrlTextBox.TabIndex = 6;
            // 
            // playlistPictureBox
            // 
            this.playlistPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.playlistPictureBox.Image = global::YoutubeVideosDownloaderPro.Properties.Resources.youtube;
            this.playlistPictureBox.Location = new System.Drawing.Point(368, 40);
            this.playlistPictureBox.Name = "playlistPictureBox";
            this.playlistPictureBox.Size = new System.Drawing.Size(64, 64);
            this.playlistPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.playlistPictureBox.TabIndex = 5;
            this.playlistPictureBox.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tabPage1.Controls.Add(this.downloadVideoButton);
            this.tabPage1.Controls.Add(this.videoUrlLabel);
            this.tabPage1.Controls.Add(this.videoUrlIconBox);
            this.tabPage1.Controls.Add(this.videoUrlTextBox);
            this.tabPage1.Controls.Add(this.videoPictureBox);
            this.tabPage1.Controls.Add(this.browseFolderButton);
            this.tabPage1.Controls.Add(this.downloadPathLabel);
            this.tabPage1.Controls.Add(this.downloadPathTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 35);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 411);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "مقطع";
            // 
            // downloadVideoButton
            // 
            this.downloadVideoButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.downloadVideoButton.FlatAppearance.BorderSize = 0;
            this.downloadVideoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadVideoButton.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadVideoButton.ForeColor = System.Drawing.Color.White;
            this.downloadVideoButton.Location = new System.Drawing.Point(285, 228);
            this.downloadVideoButton.Name = "downloadVideoButton";
            this.downloadVideoButton.Size = new System.Drawing.Size(226, 40);
            this.downloadVideoButton.TabIndex = 4;
            this.downloadVideoButton.Text = "تحميل";
            this.downloadVideoButton.UseVisualStyleBackColor = false;
            this.downloadVideoButton.Click += new System.EventHandler(this.downloadVideoButton_Click);
            // 
            // videoUrlLabel
            // 
            this.videoUrlLabel.AutoSize = true;
            this.videoUrlLabel.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.videoUrlLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.videoUrlLabel.Location = new System.Drawing.Point(628, 167);
            this.videoUrlLabel.Name = "videoUrlLabel";
            this.videoUrlLabel.Size = new System.Drawing.Size(130, 25);
            this.videoUrlLabel.TabIndex = 3;
            this.videoUrlLabel.Text = "رابط المقطع";
            // 
            // videoUrlIconBox
            // 
            this.videoUrlIconBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.videoUrlIconBox.Image = global::YoutubeVideosDownloaderPro.Properties.Resources.attachment;
            this.videoUrlIconBox.Location = new System.Drawing.Point(172, 167);
            this.videoUrlIconBox.Name = "videoUrlIconBox";
            this.videoUrlIconBox.Size = new System.Drawing.Size(28, 28);
            this.videoUrlIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.videoUrlIconBox.TabIndex = 2;
            this.videoUrlIconBox.TabStop = false;
            // 
            // videoUrlTextBox
            // 
            this.videoUrlTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.videoUrlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.videoUrlTextBox.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.videoUrlTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.videoUrlTextBox.Location = new System.Drawing.Point(206, 162);
            this.videoUrlTextBox.Name = "videoUrlTextBox";
            this.videoUrlTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.videoUrlTextBox.Size = new System.Drawing.Size(400, 33);
            this.videoUrlTextBox.TabIndex = 1;
            // 
            // videoPictureBox
            // 
            this.videoPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.videoPictureBox.Image = global::YoutubeVideosDownloaderPro.Properties.Resources.youtube;
            this.videoPictureBox.Location = new System.Drawing.Point(368, 40);
            this.videoPictureBox.Name = "videoPictureBox";
            this.videoPictureBox.Size = new System.Drawing.Size(64, 64);
            this.videoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.videoPictureBox.TabIndex = 0;
            this.videoPictureBox.TabStop = false;
            // 
            // browseFolderButton
            // 
            this.browseFolderButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.browseFolderButton.FlatAppearance.BorderSize = 0;
            this.browseFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseFolderButton.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseFolderButton.ForeColor = System.Drawing.Color.White;
            this.browseFolderButton.Image = global::YoutubeVideosDownloaderPro.Properties.Resources.folder;
            this.browseFolderButton.Location = new System.Drawing.Point(166, 347);
            this.browseFolderButton.Name = "browseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size(34, 34);
            this.browseFolderButton.TabIndex = 8;
            this.browseFolderButton.UseVisualStyleBackColor = false;
            this.browseFolderButton.Click += new System.EventHandler(this.browseFolderButton_Click);
            // 
            // downloadPathLabel
            // 
            this.downloadPathLabel.AutoSize = true;
            this.downloadPathLabel.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadPathLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.downloadPathLabel.Location = new System.Drawing.Point(628, 350);
            this.downloadPathLabel.Name = "downloadPathLabel";
            this.downloadPathLabel.Size = new System.Drawing.Size(148, 25);
            this.downloadPathLabel.TabIndex = 7;
            this.downloadPathLabel.Text = "مسار التحميل";
            // 
            // downloadPathTextBox
            // 
            this.downloadPathTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.downloadPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadPathTextBox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadPathTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.downloadPathTextBox.Location = new System.Drawing.Point(206, 348);
            this.downloadPathTextBox.Name = "downloadPathTextBox";
            this.downloadPathTextBox.ReadOnly = true;
            this.downloadPathTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.downloadPathTextBox.Size = new System.Drawing.Size(400, 27);
            this.downloadPathTextBox.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playlistUrlIconBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playlistPictureBox)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.videoUrlIconBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label downloadPathLabel;
        private System.Windows.Forms.TextBox downloadPathTextBox;
        private System.Windows.Forms.Button downloadVideoButton;
        private System.Windows.Forms.Label videoUrlLabel;
        private System.Windows.Forms.PictureBox videoUrlIconBox;
        private System.Windows.Forms.PictureBox videoPictureBox;
        private System.Windows.Forms.Button browseFolderButton;
        private System.Windows.Forms.TextBox videoUrlTextBox;
        private System.Windows.Forms.Button downloadPlaylistButton;
        private System.Windows.Forms.Label playlistUrlLabel;
        private System.Windows.Forms.PictureBox playlistUrlIconBox;
        private System.Windows.Forms.TextBox playlistUrlTextBox;
        private System.Windows.Forms.PictureBox playlistPictureBox;
    }
}