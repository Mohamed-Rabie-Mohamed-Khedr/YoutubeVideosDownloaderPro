using YoutubeVideosDownloaderPro.Core;
using System;
using System.Windows.Forms;

namespace YoutubeVideosDownloaderPro
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBox2.Text = Helper.GetDownloadsFolder();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "اختر مجلد التحميل";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.SelectedPath = textBox2.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog.SelectedPath;
                Helper.DownloadsPathSave(textBox2.Text);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "جاري التحميل...";
            await VideoDownloadFormBuilder.BuildVideoDownloadFormAsync(new[] { textBox1.Text}, textBox2.Text);
            button1.Enabled = true;
            button1.Text = "تحميل";
        }
    }
}