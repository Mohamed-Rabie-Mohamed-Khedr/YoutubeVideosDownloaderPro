using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (!EventLog.SourceExists(Helper.AppName))
            {
                EventLog.CreateEventSource(Helper.AppName, "Application");
            }
            EventLog.WriteEntry(Helper.AppName, "Application started in Time: " + DateTime.Now.ToString(), EventLogEntryType.Information);
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
                Helper.SaveDownloadsPath(downloadPathTextBox.Text);
            }
        }

        private async void downloadVideoButton_Click(object sender, EventArgs e)
        {
            if (!Helper.IsValidYouTubeVideoUrl(videoUrlTextBox.Text))
            {
                MessageBox.Show("الرابط الذي أدخلته ليس رابط فيديو صحيح.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            downloadVideoButton.Enabled = downloadPlaylistButton.Enabled = false;
            downloadVideoButton.Text = "جاري التحميل...";
            try
            {
                await VideoDownloadFormBuilder.BuildVideoDownloadFormAsync(new List<string> { videoUrlTextBox.Text }, downloadPathTextBox.Text);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                EventLog.WriteEntry(Helper.AppName, $"Failed to fetch video: {ex.Message}", EventLogEntryType.Error);
                MessageBox.Show("تعذّر جلب الفيديو، تأكد من الرابط واتصال الإنترنت", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        if (urls.Count == 0)
                        {
                            MessageBox.Show("قائمة التشغيل فارغة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
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
            catch (Exception ex)
            {
                EventLog.WriteEntry(Helper.AppName, $"Failed to fetch playlist: {ex.Message}", EventLogEntryType.Error);
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
            cancellationTokenSource.Dispose();
        }
    }
}