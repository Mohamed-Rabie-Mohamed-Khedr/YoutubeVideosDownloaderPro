using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeVideosDownloaderPro.Core
{
    internal class VideoDownloadFormBuilder
    {
        private static readonly System.Drawing.Size LabelSize = new System.Drawing.Size(640, 23);
        private static readonly System.Drawing.Font LabelFont = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private static string folderPath;
        private static List<System.Windows.Forms.Button> downloadButtons = new List<System.Windows.Forms.Button>();

        private static bool IsVideoDownloading()
        {
            return downloadButtons.Any(b => b.Tag?.ToString() == "Downloading");
        }
        
        public static async Task BuildVideoDownloadFormAsync(string[] videoUrls, string folderPath)
        {
            VideoDownloadFormBuilder.folderPath = folderPath;
            videoUrls = videoUrls?.Where(url => !string.IsNullOrWhiteSpace(url) && VideoDownloadService.IsValidYouTubeUrl(url)).ToArray() ?? Array.Empty<string>();
            if (videoUrls.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("الرجاء إدخال رابط المقطع أو معرّف يوتيوب صحيح", "خطأ في الإدخال", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.RightAlign);
                return;
            }

            var downloadForm = new System.Windows.Forms.Form()
            {
                Name = "DownloadForm",
                Text = "تحميل المقاطع من يوتيوب",
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle,
                Size = new System.Drawing.Size(700, 900),
                BackColor = System.Drawing.Color.White,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes,
                RightToLeftLayout = true,
                AutoScroll = true,
            };

            System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();
            downloadForm.FormClosing += (s, e) =>
            {
                if (IsVideoDownloading())
                {
                    var result = System.Windows.Forms.MessageBox.Show("هناك عملية تحميل جارية. هل أنت متأكد أنك تريد إغلاق النافذة؟", "تأكيد الإغلاق", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning, System.Windows.Forms.MessageBoxDefaultButton.Button2, System.Windows.Forms.MessageBoxOptions.RightAlign);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                        cancellationTokenSource.Cancel();
                    else
                        e.Cancel = true;
                }
            };

            int yOffset = 20;
            for (var i = 0; i < videoUrls.Length; i++)
            {
                try
                {
                    var video = await VideoDownloadService.GetVideoAsync(videoUrls[i], cancellationTokenSource.Token);
                    var panel = await CreateVideoPanelAsync(video, yOffset, cancellationTokenSource.Token);
                    downloadForm.Controls.Add(panel);
                }
                catch (Exception)
                {
                    var errorPanel = CreateErrorPanel(videoUrls[i], yOffset);
                    downloadForm.Controls.Add(errorPanel);
                }
                yOffset += 340;
            }

            downloadForm.ShowDialog();
        }

        private static async Task<System.Windows.Forms.Panel> CreateVideoPanelAsync(Video video, int yOffset, System.Threading.CancellationToken cancellationToken)
        {
            var panel = new System.Windows.Forms.Panel()
            {
                Location = new System.Drawing.Point(10, yOffset),
                Size = new System.Drawing.Size(660, 360),
                BackColor = System.Drawing.Color.LightGray,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };

            var pictureBox = new System.Windows.Forms.PictureBox()
            {
                Location = new System.Drawing.Point(330, 10),
                Size = new System.Drawing.Size(320, 180),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage,
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D,
                BackColor = System.Drawing.Color.Gray
            };
            await LoadThumbnailIntoPictureBoxAsync(pictureBox, video, cancellationToken);
            panel.Controls.Add(pictureBox);

            StreamManifest streamManifest;
            try
            {
                streamManifest = await VideoDownloadService.GetStreamManifestAsync(video.Id, cancellationToken);
            }
            catch
            {
                return CreateErrorPanel(video.Url, yOffset);
            }

            var videoStreams = VideoDownloadService.GetBestVideoStreams(streamManifest);
            if (!videoStreams.Any()) return CreateErrorPanel(video.Url, yOffset);

            System.Windows.Forms.ComboBox QualityComboBox = new System.Windows.Forms.ComboBox()
            {
                Location = new System.Drawing.Point(370, 320),
                Size = new System.Drawing.Size(100, 30),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                Font = LabelFont,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };
            for (int i = 0; i < videoStreams.Count; i++)
                QualityComboBox.Items.Add(videoStreams[i].VideoQuality);
            QualityComboBox.Items.Add("MP3");
            QualityComboBox.SelectedIndex = 0;
            panel.Controls.Add(QualityComboBox);

            System.Windows.Forms.ComboBox Mp3BitrateComboBox = new System.Windows.Forms.ComboBox()
            {
                Location = new System.Drawing.Point(260, 320),
                Size = new System.Drawing.Size(100, 30),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                Font = LabelFont,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes,
                Visible = false
            };
            Mp3BitrateComboBox.Items.Add("320k");
            Mp3BitrateComboBox.Items.Add("256k");
            Mp3BitrateComboBox.Items.Add("192k");
            Mp3BitrateComboBox.Items.Add("160k");
            Mp3BitrateComboBox.Items.Add("128k");
            Mp3BitrateComboBox.Items.Add("96k");
            Mp3BitrateComboBox.Items.Add("64k");
            Mp3BitrateComboBox.Items.Add("32k");
            Mp3BitrateComboBox.SelectedIndex = 3;

            QualityComboBox.SelectedIndexChanged += (s, e) =>
            {
                Mp3BitrateComboBox.Visible = QualityComboBox.SelectedIndex >= videoStreams.Count;
            };
            panel.Controls.Add(Mp3BitrateComboBox);

            System.Windows.Forms.Button DownloadButton = new System.Windows.Forms.Button()
            {
                Text = "تحميل المقطع",
                Location = new System.Drawing.Point(490, 320),
                Size = new System.Drawing.Size(160, 30),
                Font = LabelFont,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes,
                BackColor = System.Drawing.Color.Green,
                ForeColor = System.Drawing.Color.White
            };

            panel.Controls.Add(AddLabel($"اسم المقطع: {video.Title}", 200));
            panel.Controls.Add(AddLabel($"صاحب المقطع: {video.Author.ChannelTitle}", 230));
            panel.Controls.Add(AddLabel($"مدة المقطع: {video.Duration?.ToString(@"hh\:mm\:ss") ?? "غير معروف"}", 260));
            var percentageLabel = AddLabel("0%", 290);
            panel.Controls.Add(percentageLabel);

            DownloadButton.Click += async (s, e) =>
            {
                DownloadButton.Enabled = false;
                DownloadButton.Tag = "Downloading";
                downloadButtons.Add(DownloadButton);
                QualityComboBox.Enabled = false;
                Mp3BitrateComboBox.Enabled = false;

                VideoOnlyStreamInfo selectedStream = null;
                if (QualityComboBox.SelectedIndex < videoStreams.Count)
                    selectedStream = videoStreams[QualityComboBox.SelectedIndex];

                string fileExtension = selectedStream != null ? ".mp4" : ".mp3";
                string fullOutputPath = VideoDownloadService.BuildUniqueOutputPath(video.Title, folderPath, fileExtension);

                DownloadButton.Text = "جاري التحميل...";
                try
                {
                    if (!VideoDownloadService.IsFFmpegReady)
                    {
                        downloadButtons.ForEach(b => b.Enabled = false);
                        bool ready = await VideoDownloadService.EnsureFFmpegAsync(cancellationToken);
                        if (!ready)
                        {
                            System.Windows.Forms.MessageBox.Show("تعذّر تحميل ffmpeg.exe، تأكد من اتصال الإنترنت وحاول مرة أخرى", "خطأ", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            throw new Exception("ffmpeg.exe download failed");
                        }
                    }

                    var progressHandler = new Progress<double>(value =>
                    {
                        percentageLabel.Text = $"{(int)(value * 100)}%";
                    });

                    string mp3Bitrate = Mp3BitrateComboBox.SelectedItem.ToString();
                    await VideoDownloadService.DownloadAndProcessAsync(streamManifest, selectedStream, mp3Bitrate, fullOutputPath, progressHandler, cancellationToken);

                    System.Windows.Forms.MessageBox.Show(
                        $"تم تحميل المقطع بنجاح إلى:\n{fullOutputPath}",
                        "نجاح",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1,
                        System.Windows.Forms.MessageBoxOptions.RightAlign);
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullOutputPath}\"");
                }
                catch (OperationCanceledException) { }
                catch
                {
                    System.Windows.Forms.MessageBox.Show(
                        "حدث خطأ أثناء تحميل المقطع",
                        "خطأ في التحميل",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1,
                        System.Windows.Forms.MessageBoxOptions.RightAlign);
                }
                finally
                {
                    DownloadButton.Text = "تحميل المقطع";
                    DownloadButton.Tag = null;
                    downloadButtons.ForEach(b => { if (b.Tag?.ToString() != "Downloading") b.Enabled = true; });
                    downloadButtons.Remove(DownloadButton);

                    QualityComboBox.Enabled = true;
                    Mp3BitrateComboBox.Enabled = true;
                }
            };
            panel.Controls.Add(DownloadButton);
            return panel;
        }

        private static System.Windows.Forms.Panel CreateErrorPanel(string url, int yOffset)
        {
            var panel = new System.Windows.Forms.Panel()
            {
                Location = new System.Drawing.Point(10, yOffset),
                Size = new System.Drawing.Size(660, 360),
                BackColor = System.Drawing.Color.LightGray,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };

            var errorLabel = new System.Windows.Forms.Label()
            {
                Text = $"خطأ: لم يتمكن من تحميل المقطع - {url}",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(640, 80),
                Font = LabelFont,
                ForeColor = System.Drawing.Color.Red,
                AutoSize = false,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };
            panel.Controls.Add(errorLabel);
            return panel;
        }

        private static async Task LoadThumbnailIntoPictureBoxAsync(System.Windows.Forms.PictureBox pictureBox, Video video, System.Threading.CancellationToken cancellationToken)
        {
            var imageData = await VideoDownloadService.DownloadThumbnailBytesAsync(video, cancellationToken);
            if (imageData == null || imageData.Length == 0) return;

            try
            {
                using (var ms = new MemoryStream(imageData))
                using (var bmp = new System.Drawing.Bitmap(ms))
                {
                    pictureBox.Image = (System.Drawing.Bitmap)bmp.Clone();
                }
            }
            catch
            {
            }
        }

        private static System.Windows.Forms.Label AddLabel(string text, int yPosition)
        {
            var label = new System.Windows.Forms.Label()
            {
                Text = text,
                Location = new System.Drawing.Point(10, yPosition),
                Size = LabelSize,
                Font = LabelFont,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };
            return label;
        }
    }
}