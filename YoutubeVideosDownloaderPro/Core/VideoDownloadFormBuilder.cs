using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static List<System.Windows.Forms.Button> downloadButtons = new();
        private static System.Threading.SemaphoreSlim videoInfoSemaphore = new(5);
        private static Dictionary<System.Windows.Forms.Button, TaskCompletionSource<bool>> downloadCompletions = new();
        private enum DownloadState
        {
            None,
            WaitingForDownload,
            Downloading,
            Downloaded
        }
        private static bool IsVideoDownloading()
        {
            return downloadButtons.Any(b => (DownloadState)b.Tag == DownloadState.WaitingForDownload || (DownloadState)b.Tag == DownloadState.Downloading);
        }
        
        public static async Task BuildVideoDownloadFormAsync(List<string> videoUrls, string folderPath)
        {
            VideoDownloadFormBuilder.folderPath = folderPath;
            var downloadForm = new System.Windows.Forms.Form()
            {
                Name = "DownloadForm",
                Text = "تحميل المقاطع من يوتيوب",
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle,
                Size = new System.Drawing.Size(700, 900),
                BackColor = System.Drawing.Color.FromArgb(22, 22, 22),
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
                    {
                        cancellationTokenSource.Cancel();
                    }
                    else
                        e.Cancel = true;
                }
            };

            downloadForm.FormClosed += (s, e) =>
            {
                cancellationTokenSource.Dispose();
                downloadButtons.Clear();
                downloadCompletions.Clear();
            };

            int yOffset;
            if (videoUrls.Count > 1)
            {
                var downloadAllButton = new System.Windows.Forms.Button()
                {
                    Text = "تحميل جميع المقاطع",
                    Location = new System.Drawing.Point(10, 10),
                    Size = new System.Drawing.Size(660, 40),
                    Font = LabelFont,
                    RightToLeft = System.Windows.Forms.RightToLeft.Yes,
                    BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(153))))),
                    FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                    ForeColor = System.Drawing.Color.White
                };
                downloadAllButton.FlatAppearance.BorderSize = 0;
                downloadAllButton.Click += async (s, e) =>
                {
                    downloadAllButton.Enabled = false;
                    for (int i = 0; i < downloadButtons.Count; i++)
                    {
                        if ((DownloadState)downloadButtons[i].Tag == DownloadState.None)
                            downloadButtons[i].Tag = DownloadState.WaitingForDownload;
                    }

                    for (int i = 0; i < downloadButtons.Count; i++)
                    {
                        if ((DownloadState)downloadButtons[i].Tag == DownloadState.WaitingForDownload && downloadButtons[i].Enabled)
                        {
                            var tcs = new TaskCompletionSource<bool>();
                            downloadCompletions[downloadButtons[i]] = tcs;
                            downloadButtons[i].PerformClick();
                            await tcs.Task;
                        }
                    }
                };
                downloadForm.Controls.Add(downloadAllButton);
                yOffset = 50;
            }
            else
                yOffset = 0;
            var fetchTasks = videoUrls.Select(async url =>
            {
                await videoInfoSemaphore.WaitAsync(cancellationTokenSource.Token);
                try
                {
                    var video = await VideoDownloadService.GetVideoAsync(url, cancellationTokenSource.Token);
                    if (video == null) throw new InvalidOperationException("Failed to fetch video information.");
                    var manifest = await VideoDownloadService.GetStreamManifestAsync(video.Id, cancellationTokenSource.Token);
                    if (manifest == null) throw new InvalidOperationException("Failed to fetch stream manifest.");
                    return (Url: url, Video: video, Manifest: manifest);
                }
                catch
                {
                    return (Url: url, Video: null, Manifest: null);
                }
                finally
                {
                    videoInfoSemaphore.Release();
                }
            }).ToList();

            var results = await Task.WhenAll(fetchTasks);
            
            // دلوقتي نبني الـ UI بالترتيب الأصلي بعد ما البيانات كلها جاهزة
            foreach (var result in results)
            {
                if (result.Video == null)
                    downloadForm.Controls.Add(CreateErrorPanel(result.Url, yOffset));
                else
                {
                    var panel = await CreateVideoPanelAsync(result.Video, result.Manifest, yOffset, cancellationTokenSource.Token);
                    downloadForm.Controls.Add(panel);
                }
                yOffset += 370;
            }

            downloadForm.ShowDialog();
        }

        private static async Task<System.Windows.Forms.Panel> CreateVideoPanelAsync(Video video, StreamManifest streamManifest, int yOffset, System.Threading.CancellationToken cancellationToken)
        {
            var panel = new System.Windows.Forms.Panel()
            {
                Location = new System.Drawing.Point(10, yOffset),
                Size = new System.Drawing.Size(660, 360),
                BackColor = System.Drawing.Color.FromArgb(40, 40, 40),
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };

            var pictureBox = new System.Windows.Forms.PictureBox()
            {
                Location = new System.Drawing.Point(330, 10),
                Size = new System.Drawing.Size(320, 180),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(55, 55, 55)
            };
            await LoadThumbnailIntoPictureBoxAsync(pictureBox, video, cancellationToken);
            panel.Controls.Add(pictureBox);
            
            var videoStreams = VideoDownloadService.GetBestVideoStreams(streamManifest);
            if (!videoStreams.Any()) return CreateErrorPanel(video.Url, yOffset);

            System.Windows.Forms.ComboBox QualityComboBox = new System.Windows.Forms.ComboBox()
            {
                Location = new System.Drawing.Point(350, 320),
                Size = new System.Drawing.Size(130, 30),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                Font = LabelFont,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 45),
                ForeColor = System.Drawing.Color.FromArgb(200, 200, 200),
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };
            for (int i = 0; i < videoStreams.Count; i++)
                QualityComboBox.Items.Add(videoStreams[i].VideoQuality);
            QualityComboBox.Items.Add("MP3");
            QualityComboBox.SelectedIndex = 0;
            panel.Controls.Add(QualityComboBox);

            System.Windows.Forms.ComboBox Mp3BitrateComboBox = new System.Windows.Forms.ComboBox()
            {
                Location = new System.Drawing.Point(240, 320),
                Size = new System.Drawing.Size(100, 30),
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                Font = LabelFont,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 45),
                ForeColor = System.Drawing.Color.FromArgb(200, 200, 200),
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

            System.Windows.Forms.Button DownloadButton = new System.Windows.Forms.Button()
            {
                Text = "تحميل المقطع",
                Tag = DownloadState.None,
                Location = new System.Drawing.Point(490, 320),
                Size = new System.Drawing.Size(160, 30),
                Font = LabelFont,
                RightToLeft = System.Windows.Forms.RightToLeft.Yes,
                BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(153))))),
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                ForeColor = System.Drawing.Color.White
            };
            DownloadButton.FlatAppearance.BorderSize = 0;

            QualityComboBox.SelectedIndexChanged += (s, e) =>
            {
                Mp3BitrateComboBox.Visible = QualityComboBox.SelectedIndex >= videoStreams.Count;
                DownloadButton.Enabled = true;
            };
            panel.Controls.Add(Mp3BitrateComboBox);

            panel.Controls.Add(AddLabel($"اسم المقطع: {video.Title}", 200));
            panel.Controls.Add(AddLabel($"صاحب المقطع: {video.Author.ChannelTitle}", 230));
            panel.Controls.Add(AddLabel($"مدة المقطع: {video.Duration?.ToString(@"hh\:mm\:ss") ?? "غير معروف"}", 260));
            var percentageLabel = AddLabel("0%", 290);
            downloadButtons.Add(DownloadButton);
            panel.Controls.Add(percentageLabel);

            DownloadButton.Click += async (s, e) =>
            {
                LabelUpdate(percentageLabel, "0%", System.Drawing.Color.FromArgb(255, 255, 128));
                DownloadButton.Tag = DownloadState.Downloading;
                DownloadButton.Enabled = false;
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
                        downloadButtons.ForEach(b =>
                        {
                            if ((DownloadState)b.Tag != DownloadState.Downloading)
                                b.Enabled = true;
                        });
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
                    Exception downloadException = await VideoDownloadService.DownloadAndProcessAsync(streamManifest, selectedStream, mp3Bitrate, fullOutputPath, progressHandler, cancellationToken);
                    if (downloadException != null) throw downloadException;
                    DownloadButton.Tag = DownloadState.Downloaded;
                    LabelUpdate(percentageLabel, "تم التحميل بنجاح", System.Drawing.Color.FromArgb(100, 255, 100));
                    if (!IsVideoDownloading()) Process.Start("explorer.exe", $"/select,\"{fullOutputPath}\"");
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    DownloadButton.Tag = DownloadState.None;
                    EventLog.WriteEntry(Helper.AppName, ex.Message, EventLogEntryType.Error);
                    DownloadButton.Enabled = true;
                    LabelUpdate(percentageLabel, "حدث خطأ أثناء تحميل المقطع", System.Drawing.Color.FromArgb(255, 100, 100));
                }
                finally
                {
                    DownloadButton.Text = "تحميل المقطع";
                    QualityComboBox.Enabled = true;
                    Mp3BitrateComboBox.Enabled = true;

                    if (downloadCompletions.TryGetValue(DownloadButton, out var tcs))
                    {
                        downloadCompletions.Remove(DownloadButton);
                        tcs.TrySetResult(true);
                    }
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
                BackColor = System.Drawing.Color.FromArgb(40, 40, 40),
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };

            var errorLabel = new System.Windows.Forms.Label()
            {
                Text = $"خطأ: لم يتمكن من تحميل المقطع - {url}",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(640, 80),
                Font = LabelFont,
                ForeColor = System.Drawing.Color.FromArgb(255, 100, 100),
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
            catch (Exception ex)
            {
                EventLog.WriteEntry(Helper.AppName, ex.Message, EventLogEntryType.Error);
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
                ForeColor = System.Drawing.Color.FromArgb(200, 200, 200),
                RightToLeft = System.Windows.Forms.RightToLeft.Yes
            };
            return label;
        }
        private static void LabelUpdate(System.Windows.Forms.Label label, string text, System.Drawing.Color color)
        {
            label.Text = text;
            label.ForeColor = color;
        }
    }
}