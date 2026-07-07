# YouTube Videos Downloader Pro

A lightweight Windows desktop application built with **.NET Framework 4.8** for downloading YouTube videos with multiple quality options and format selection.

## Features

- 🎬 Download videos from YouTube links
- 🎨 Windows Forms UI with RTL support (Arabic)
- 📊 Multiple quality options (4K, 2K, 1080p, 720p, 480p, etc.)
- 🎬 Extract audio from videos
- ⚙️ Built-in FFmpeg support
- 💾 Custom download folder selection
- 🛡️ URL validation and error handling

## Requirements

- **OS**: Windows 7 or later
- **.NET Framework**: 4.8 or higher
- **RAM**: 2GB minimum
- **Internet**: Required for downloading

## Usage

1. Enter a YouTube URL or video ID
2. Select download folder
3. View video details and choose quality
4. Select format (Video or **Audio/MP3**)
5. Click download and monitor progress

### MP3 Download

Extract audio from YouTube videos and save as MP3 files. Perfect for downloading podcasts and audio content directly from YouTube.

## Architecture

| Component | Purpose |
|-----------|---------|
| `MainForm.cs` | User interface and input |
| `VideoDownloadFormBuilder.cs` | Download orchestration |
| `VideoDownloadService.cs` | Core download service logic |
| `Helper.cs` | Utility functions |

## Dependencies

- **YoutubeExplode** - YouTube data access
- **Xabe.FFmpeg** - Video/audio processing
- **AngleSharp** - HTML parsing

## License

Licensed under the MIT License - see [LICENSE](LICENSE) file for details.

## Contact

- 📧 Email: mohamedrabie3473@gmail.com
- 🐙 LinkedIn: [https://www.linkedin.com/in/mohamed-rabie-123582231/]

**Enjoy downloading! 🎉**