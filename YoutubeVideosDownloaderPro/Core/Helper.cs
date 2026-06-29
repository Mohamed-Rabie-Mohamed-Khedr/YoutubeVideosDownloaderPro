using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeVideosDownloaderPro.Core
{
    internal class Helper
    {
        // تعريف الـ GUID الخاص بمجلد التحميلات في نظام ويندوز
        private static readonly Guid DownloadsGuid = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        // استيراد الدالة الخاصة بجلب المجلدات النظامية من ويندوز
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string ppszPath);
        public static string GetDownloadsFolder()
        {
            // تحقق من وجود مسار التحميلات في الريجستري أولاً
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\DownloadYoutubeVideosApp"))
            {
                if (rk != null)
                {
                    object value = rk.GetValue("DownloadsPath");
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
            }

            // استخدام SHGetKnownFolderPath لجلب مسار التحميلات
            int result = SHGetKnownFolderPath(DownloadsGuid, 0, IntPtr.Zero, out string path);
            if (result == 0) // 0 تعني أن العملية تمت بنجاح (S_OK)
            {
                return path;
            }
            else
            {
                // حل بديل في حال فشل الدالة لأي سبب
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }
        }

        public static void DownloadsPathSave(string path)
        {
            using (RegistryKey rk = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\DownloadYoutubeVideosApp", true))
            {
                rk.SetValue("DownloadsPath", path);
            }
        }



        public static async Task<string> EnsureFFmpegExistsAsync(CancellationToken cancellationToken)
        {
            // 1. تحديد مسار ملف ffmpeg النهائي في مجلد برنامجك
            string targetFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");

            // إذا كان الملف موجوداً بالفعل، اخرج من الدالة مباشرة
            if (File.Exists(targetFilePath)) return targetFilePath;

            // تحقق من وجود ffmpeg في متغير البيئة PATH
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (pathEnv != null)
            {
                // تقسيم متغير البيئة PATH إلى مسارات منفصلة والتحقق من وجود ffmpeg.exe في أي منها
                var paths = pathEnv.Split(Path.PathSeparator);
                foreach (var path in paths)
                {
                    string fullPath = Path.Combine(path, "ffmpeg.exe");
                    if (File.Exists(fullPath)) return fullPath;
                }
            }

            MessageBox.Show("يتم الآن تحميل ملفات البرنامج الأساسية (FFmpeg) تلقائياً، قد يستغرق هذا دقيقة بناءً على سرعة الإنترنت لديك.", "تهيئة البرنامج", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

            string tempZipPath = Path.Combine(Path.GetTempPath(), "ffmpeg_temp.zip");
            string tempExtractPath = Path.Combine(Path.GetTempPath(), "ffmpeg_extracted");
            string downloadUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";

            try
            {
                // المسؤولة عن إرسال طلبات HTTP (زي تحميل ملفات من الإنترنت).
                using (var httpClient = new HttpClient())
                {
                    // ResponseHeadersRead حمل محتوى على دفعات، بحيث نقدر نبدأ في الكتابة على القرص قبل ما يكتمل التحميل بالكامل.
                    var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    // 2. حفظ ملف ZIP المؤقت على القرص
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    using (var fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                    {
                        // مصفوفة للقراءة والكتابة من وإلى محتوى الاستجابة، بحجم 80 كيلوبايت لكل عملية.
                        byte[] buffer = new byte[81920];
                        // متغير هيحمل "عدد البايتات الفعلية" اللي اتقرت في كل دورة من الحلقة
                        int bytesRead;
                        // حلقة لقراءة البيانات من الاستجابة وكتابتها في الملف المؤقت
                        while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fs.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        }
                    }
                }

                // 3. تنظيف أي مجلد استخراج قديم إذا كان موجوداً
                if (Directory.Exists(tempExtractPath))
                    Directory.Delete(tempExtractPath, true);

                // 4. فك ضغط ملف الـ ZIP بأكمله في المجلد المؤقت
                System.IO.Compression.ZipFile.ExtractToDirectory(tempZipPath, tempExtractPath);

                // 5. البحث عن ملف ffmpeg.exe داخل المجلد المستخرج
                string[] files = Directory.GetFiles(tempExtractPath, "ffmpeg.exe", SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    if (File.Exists(targetFilePath)) File.Delete(targetFilePath);
                    File.Move(files[0], targetFilePath);
                    MessageBox.Show("تم تحميل وتثبيت أداة الدمج بنجاح!", "اكتملت التهيئة", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                }
                else
                {
                    throw new Exception("ffmpeg.exe not found inside the zip archive.");
                }
            }
            catch (OperationCanceledException)
            {
                targetFilePath = null;
                throw; // أعد رميها عشان الـ caller يتعرف عليها كـ cancellation حقيقي
            }
            catch { targetFilePath = null; }
            finally
            {
                // 6. تنظيف الملفات والمجلدات المؤقتة
                if (File.Exists(tempZipPath)) File.Delete(tempZipPath);
                if (Directory.Exists(tempExtractPath)) Directory.Delete(tempExtractPath, true);
            }

            return targetFilePath;
        }
    }
}