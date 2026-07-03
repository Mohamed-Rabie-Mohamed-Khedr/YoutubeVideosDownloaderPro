using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeVideosDownloaderPro.Core;
namespace YoutubeVideosDownloaderPro
{
    public partial class MainForm : Form
    {
        System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            downloadPathTextBox.Text = Helper.GetDownloadsFolder();
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "اختر مجلد التحميل";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.SelectedPath = downloadPathTextBox.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                downloadPathTextBox.Text = folderBrowserDialog.SelectedPath;
                Helper.DownloadsPathSave(downloadPathTextBox.Text);
            }
        }

        private async void downloadVideoButton_Click(object sender, EventArgs e)
        {
            downloadVideoButton.Enabled = downloadPlaylistButton.Enabled = false;
            downloadVideoButton.Text = "جاري التحميل...";
            try
            {
                await VideoDownloadFormBuilder.BuildVideoDownloadFormAsync(new List<string> { videoUrlTextBox.Text }, downloadPathTextBox.Text);
            }
            finally
            {
                downloadVideoButton.Enabled = downloadPlaylistButton.Enabled = true;
                downloadVideoButton.Text = "تحميل";
            }
        }

        private async void downloadPlaylistButton_Click(object sender, EventArgs e)
        {
            downloadVideoButton.Enabled = downloadPlaylistButton.Enabled = false;
            downloadPlaylistButton.Text = "جاري التحميل...";
            try
            {
                if (Helper.IsPlaylistUrl(playlistUrlTextBox.Text))
                {
                    using (var youtube = new YoutubeClient())
                    {
                        var urls = new List<string>();
                        await foreach (var video in youtube.Playlists.GetVideosAsync(playlistUrlTextBox.Text).WithCancellation(cancellationTokenSource.Token))
                        {
                            urls.Add(video.Url);
                            downloadPlaylistButton.Text = $"جاري التحميل... ({urls.Count})";
                        }
                        await VideoDownloadFormBuilder.BuildVideoDownloadFormAsync(urls, downloadPathTextBox.Text);
                    }
                }
                else
                {
                    MessageBox.Show("الرابط الذي أدخلته ليس رابط قائمة تشغيل.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception)
            {
                MessageBox.Show("تعذّر جلب قائمة التشغيل، تأكد من الرابط واتصال الإنترنت", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                downloadVideoButton.Enabled = downloadPlaylistButton.Enabled = true;
                downloadPlaylistButton.Text = "تحميل";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}