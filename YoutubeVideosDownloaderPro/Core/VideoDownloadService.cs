using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeVideosDownloaderPro.Core
{
    internal static class VideoDownloadService
    {
        private static readonly YoutubeClient youtube = new YoutubeClient();
        private static readonly HttpClient httpClient = new HttpClient();
        private static string ffmpegPath;
        
        public static bool IsValidYouTubeUrl(string url)
        {
            return Regex.IsMatch(url.Trim(), @"^([a-zA-Z0-9_-]{11}|(https?://)?(www\.)?(youtube\.com|youtu\.be)/.*)$");
        }

        public static async Task<Video> GetVideoAsync(string videoUrl, CancellationToken cancellationToken) =>
            await youtube.Videos.GetAsync(videoUrl, cancellationToken);

        public static async Task<StreamManifest> GetStreamManifestAsync(string videoId, CancellationToken cancellationToken) =>
            await youtube.Videos.Streams.GetManifestAsync(videoId, cancellationToken);
        public static List<VideoOnlyStreamInfo> GetBestVideoStreams(StreamManifest streamManifest)
        {
            return streamManifest.GetVideoOnlyStreams()
                .GroupBy(s => s.VideoQuality.Label)
                .Select(g => g.OrderByDescending(s => s.Bitrate).First())
                .ToList();
        }
        
        public static async Task<byte[]> DownloadThumbnailBytesAsync(Video video, CancellationToken cancellationToken)
        {
            try
            {
                var thumbnailUrl = video.Thumbnails.FirstOrDefault()?.Url;
                if (string.IsNullOrWhiteSpace(thumbnailUrl)) return null;

                string cleanUrl = thumbnailUrl.Split('?')[0];
                var response = await httpClient.GetAsync(cleanUrl, cancellationToken);
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                return null;
            }
        }

        public static bool IsFFmpegReady => !string.IsNullOrEmpty(ffmpegPath);
        
        public static async Task<bool> EnsureFFmpegAsync(CancellationToken cancellationToken)
        {
            if (!IsFFmpegReady)
                ffmpegPath = await Helper.EnsureFFmpegExistsAsync(cancellationToken);
            return IsFFmpegReady;
        }
        
        public static string BuildUniqueOutputPath(string title, string folderPath, string extension)
        {
            string safeTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            int counter = 1;
            while (File.Exists(Path.Combine(folderPath, $"{safeTitle}{extension}")))
            {
                safeTitle = $"{safeTitle}_{counter}";
                counter++;
            }
            return Path.Combine(folderPath, $"{safeTitle}{extension}");
        }
        
        public static async Task DownloadAndProcessAsync(
            StreamManifest streamManifest,
            VideoOnlyStreamInfo selectedVideoStream,
            string mp3Bitrate,
            string fullOutputPath,
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            string tempVideoPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_video.tmp");
            string tempAudioPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_audio.tmp");

            try
            {
                if (selectedVideoStream != null)
                {
                    await youtube.Videos.Streams.DownloadAsync(selectedVideoStream, tempVideoPath, progress, cancellationToken);
                    await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, tempAudioPath, null, cancellationToken);
                }
                else
                {
                    await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, tempAudioPath, progress, cancellationToken);
                }

                string ffmpegArgs = selectedVideoStream != null
                    ? $"-y -i \"{tempVideoPath}\" -i \"{tempAudioPath}\" -c copy \"{fullOutputPath}\""
                    : $"-y -i \"{tempAudioPath}\" -vn -acodec libmp3lame -b:a {mp3Bitrate} \"{fullOutputPath}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = ffmpegArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                using (cancellationToken.Register(() =>
                {
                    try { if (!process.HasExited) process.Kill(); } catch { }
                }))
                {
                    await Task.Run(() => process.WaitForExit());
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                if (File.Exists(tempVideoPath)) File.Delete(tempVideoPath);
                if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
            }
        }
    }
}