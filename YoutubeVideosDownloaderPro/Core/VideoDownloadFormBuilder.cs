using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeVideosDownloaderPro.Core
{
    internal class VideoDownloadFormBuilder
    {
        private static readonly System.Drawing.Size LabelSize = new System.Drawing.Size(640, 23);
        private static readonly System.Drawing.Font LabelFont = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private static readonly YoutubeClient youtube = new YoutubeClient();
        private static readonly HttpClient HttpClient = new HttpClient();
        private static string folderPath, ffmpegPath;
        private static List<System.Windows.Forms.Button> downloadButtons = new List<System.Windows.Forms.Button>();


        private static bool IsVideoDownloading()
        {
            return downloadButtons.Any(b => b.Tag?.ToString() == "Downloading");
        }
        private static bool IsValidYouTubeUrl(string url)
        {
            return Regex.IsMatch(url.Trim(), @"^([a-zA-Z0-9_-]{11}|(https?://)?(www\.)?(youtube\.com|youtu\.be)/.*)$");
        }

        /// <summary>
        /// الدالة الرئيسية لبناء نموذج تحميل الفيديوهات
        /// تأخذ مصفوفة من روابط يوتيوب وتعرض كل منها في نموذج منفصل
        /// </summary>
        public static async Task BuildVideoDownloadFormAsync(string[] videoUrls, string folderPath)
        {
            VideoDownloadFormBuilder.folderPath = folderPath;
            videoUrls = videoUrls?.Where(url => !string.IsNullOrWhiteSpace(url) && IsValidYouTubeUrl(url)).ToArray() ?? Array.Empty<string>();
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
                    {
                        cancellationTokenSource.Cancel();
                        // تنظيف الموارد عند إغلاق الشاشة
                    }
                    else e.Cancel = true;
                }
            };

            int yOffset = 20;
            for (var i = 0; i < videoUrls.Length; i++)
            {
                try
                {
                    // الحصول على بيانات الفيديو من يوتيوب (اسم، الناشر، الصورة المصغرة، إلخ)
                    var video = await youtube.Videos.GetAsync(videoUrls[i]);
                    var panel = await CreateVideoPanelAsync(video, yOffset, cancellationTokenSource);
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
        private static async Task<System.Windows.Forms.Panel> CreateVideoPanelAsync(YoutubeExplode.Videos.Video video, int yOffset, System.Threading.CancellationTokenSource cancellationTokenSource)
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
                // تمديد الصورة لملء الصندوق
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage,
                // إضافة إطار بحدود بارزة
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D,
                BackColor = System.Drawing.Color.Gray
            };

            await LoadThumbnailAsync(pictureBox, video);
            panel.Controls.Add(pictureBox);

            // 2. الحصول على جميع مسارات الفيديو المتاحة
            StreamManifest streamManifest;
            try
            {
                streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            }
            catch
            {
                return CreateErrorPanel(video.Url, yOffset);
            }
            // تجميع المسارات بناءً على اسم الجودة
            var videoStreams = streamManifest.GetVideoOnlyStreams()
                .GroupBy(s => s.VideoQuality.Label)
                .Select(g => g.OrderByDescending(s => s.Bitrate).First())
                .ToList();

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
            // تعيين الجودة الافتراضية إلى أعلى جودة متاحة
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
            Mp3BitrateComboBox.SelectedIndex = 5;

            QualityComboBox.SelectedIndexChanged += (s, e) =>
            {
                Mp3BitrateComboBox.Visible = QualityComboBox.SelectedIndex >= videoStreams.Count;
            };

            panel.Controls.Add(Mp3BitrateComboBox);

            // 3. تحميل المقطع
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

                // 1. تنفيذ تحميل المقطع باستخدام selectedStream المختار من القائمة
                VideoOnlyStreamInfo selectedStream = null;
                if (QualityComboBox.SelectedIndex < videoStreams.Count)
                    selectedStream = videoStreams[QualityComboBox.SelectedIndex];

                // 2. تنظيف اسم الفيديو من الحروف غير الصالحة لتسمية الملفات في نظام التشغيل
                string safeTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
                string fileExtension = selectedStream != null ? ".mp4" : ".mp3";
                int counter = 1;
                while (File.Exists(Path.Combine(folderPath, $"{safeTitle}{fileExtension}")))
                {
                    safeTitle = $"{safeTitle}_{counter}";
                    counter++;
                }

                // 3. دمج المسار بالكامل مع امتداد الملف المناسب
                string fullOutputPath = Path.Combine(folderPath, $"{safeTitle}{fileExtension}");

                // 4. جلب أعلى مسار صوت متاح لدمجه مع الفيديو المختار
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                DownloadButton.Text = "جاري التحميل...";
                string tempVideoPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_video.tmp");
                string tempAudioPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_audio.tmp");
                try
                {
                    // 6. التأكد من وجود ملف ffmpeg.exe في مجلد برنامجك، وإذا لم يكن موجوداً، قم بتحميله تلقائياً من الإنترنت
                    if (string.IsNullOrEmpty(ffmpegPath))
                    {
                        downloadButtons.ForEach(b => b.Enabled = false);
                        ffmpegPath = await Helper.EnsureFFmpegExistsAsync(cancellationTokenSource);
                        if (string.IsNullOrEmpty(ffmpegPath))
                        {
                            System.Windows.Forms.MessageBox.Show("تعذّر تحميل ffmpeg.exe، تأكد من اتصال الإنترنت وحاول مرة أخرى", "خطأ", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            throw new Exception("ffmpeg.exe download failed");
                        }
                    }

                    var progressHandler = new Progress<double>(value =>
                    {
                        percentageLabel.Text = $"{(int)(value * 100)}%";
                    });

                    // 1. تحميل الفيديو والصوت كملفات مؤقتة منفصلة (بدون أي pipe streaming)
                    if (selectedStream != null)
                    {
                        await youtube.Videos.Streams.DownloadAsync(selectedStream, tempVideoPath, progressHandler, cancellationTokenSource.Token);
                        await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, tempAudioPath, null, cancellationTokenSource.Token);
                    }
                    else
                    {
                        await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, tempAudioPath, progressHandler, cancellationTokenSource.Token);
                    }
                    var ffmpegArgs = selectedStream != null ? $"-y -i \"{tempVideoPath}\" -i \"{tempAudioPath}\" -c copy \"{fullOutputPath}\""
                    : $"-y -i \"{tempAudioPath}\" -vn -acodec libmp3lame -b:a {Mp3BitrateComboBox.SelectedItem} \"{fullOutputPath}\"";

                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = ffmpegArgs,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var process = System.Diagnostics.Process.Start(psi))
                    {
                        // تسجيل دالة تقتل العملية لو المستخدم عمل cancel
                        using (cancellationTokenSource.Token.Register(() =>
                        {
                            try { if (!process.HasExited) process.Kill(); } catch { }
                        }))
                        {
                            // تشغيل الانتظار المتزامن (WaitForExit) جوه Task.Run عشان مايعملش block للواجهة
                            await Task.Run(() => process.WaitForExit());
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    
                    // 7. إظهار رسالة النجاح للمستخدم
                    System.Windows.Forms.MessageBox.Show(
                        $"تم تحميل المقطع بنجاح إلى:\n{fullOutputPath}",
                        "نجاح",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1,
                        System.Windows.Forms.MessageBoxOptions.RightAlign
                        );
                    // فتح المجلد الذي تم حفظ الفيديو فيه
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullOutputPath}\"");
                }
                catch (OperationCanceledException) {}
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
                    downloadButtons.ForEach(delegate (System.Windows.Forms.Button b)
                    {
                        if (b.Tag?.ToString() != "Downloading") b.Enabled = true;
                    });
                    downloadButtons.Remove(DownloadButton);

                    QualityComboBox.Enabled = true;
                    Mp3BitrateComboBox.Enabled = true;

                    if (File.Exists(tempVideoPath)) File.Delete(tempVideoPath);
                    if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
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

        /// <summary>
        /// تحميل الصورة المصغرة للفيديو من الإنترنت بشكل غير متزامن
        /// </summary>
        private static async Task LoadThumbnailAsync(System.Windows.Forms.PictureBox pictureBox, YoutubeExplode.Videos.Video video)
        {
            try
            {
                // الحصول على أول صورة مصغرة متاحة للفيديو
                var thumbnailUrl = video.Thumbnails.FirstOrDefault()?.Url;
                if (string.IsNullOrWhiteSpace(thumbnailUrl))
                    return;

                // إزالة معاملات الاستعلام من الرابط (المعاملات بعد علامة الاستفهام)
                string cleanUrl = thumbnailUrl.Split('?')[0];
                // إرسال طلب HTTP للحصول على الصورة من الإنترنت
                var response = await HttpClient.GetAsync(cleanUrl);
                // التحقق من نجاح الطلب (رمز الحالة 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // قراءة بيانات الصورة من الاستجابة كمصفوفة بايتات
                    var imageData = await response.Content.ReadAsByteArrayAsync();
                    if (imageData?.Length > 0)
                    {
                        using (var ms = new MemoryStream(imageData))
                        using (var bmp = new System.Drawing.Bitmap(ms))
                        {
                            pictureBox.Image = (System.Drawing.Bitmap)bmp.Clone();
                        }
                    }
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