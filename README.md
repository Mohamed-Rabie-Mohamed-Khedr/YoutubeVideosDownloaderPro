# YouTube Videos Downloader Pro

A lightweight Windows desktop application built with **.NET Framework 4.8** that enables users to download YouTube videos easily with multiple quality options and format selection.

## Features

- 🎬 Download videos directly from YouTube links
- 🎨 Intuitive Windows Forms UI with RTL support
- 📊 Multiple quality options (4K, 2K, 1080p, 720p, 480p, etc.)
- 🎵 Extract audio from videos
- ⚙️ Built-in FFmpeg support
- 💾 Custom download folder selection
- 🛡️ URL validation before download
- ⏸️ Cancel downloads anytime

## Requirements

- **OS**: Windows 7 or later
- **.NET Framework**: 4.8 or higher
- **RAM**: 2GB minimum
- **Storage**: 500MB minimum
- **Internet**: Required for downloading and initial FFmpeg setup

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| YoutubeExplode | 6.6.0 | YouTube video data access |
| Xabe.FFmpeg | 6.0.2 | Video/audio processing |
| AngleSharp | 1.4.0 | HTML parsing |
| CliWrap | 3.10.1 | CLI command execution |
| System.Text.Json | 9.0.2 | JSON handling |

## Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/Mohamed-Rabie-Mohamed-Khedr/DownloadYouTubeVideosApp.git
cd DownloadYouTubeVideosApp
```

### 2. Build
```bash
# Using Visual Studio: Open YoutubeVideosDownloaderPro.slnx and Build
# Or use command line:
dotnet build
```

### 3. Run
```bash
# F5 in Visual Studio
# Or run the executable:
.\bin\Debug\Youtube Videos Downloader Pro.exe
```

## Usage

1. **Paste YouTube Link**: Enter a YouTube URL or video ID
2. **Select Folder**: Click the folder button to choose download location
3. **Click Download**: A new window appears with video details
4. **Choose Quality**: Select preferred video quality
5. **Select Format**: Choose video only
6. **Start**: Click download button to begin
7. **Monitor**: Watch the progress bar; cancel anytime

## Project Structure

```
DownloadYouTubeVideosApp/
├── Program.cs                          # Entry point
├── MainForm.cs                         # Main UI form
├── Core/
│   ├── VideoDownloadFormBuilder.cs    # Download logic
│   └── Helper.cs                       # Utility functions
├── Properties/                         # Assembly info & resources
├── Resources/                          # Images & icons
└── packages.config                     # NuGet dependencies
```

## Main Components

### MainForm.cs
- User input for YouTube URL
- Folder selection for downloads
- Download button trigger

### VideoDownloadFormBuilder.cs
Key functions:
- `BuildVideoDownloadFormAsync()` - Main download orchestration
- `IsValidYouTubeUrl()` - URL validation
- `CreateVideoPanelAsync()` - Video info panel creation
- `LoadThumbnailAsync()` - Thumbnail fetching
- `DownloadVideoAsync()` - Video download handling

### Helper.cs
- `GetDownloadsFolder()` - Get default downloads directory
- `DownloadsPathSave()` - Save folder selection to registry
- `EnsureFFmpegExistsAsync()` - FFmpeg initialization

## How It Works

1. **URL Validation** - Checks if link is valid YouTube URL or video ID
2. **Fetch Video Info** - Retrieves title, author, duration, and thumbnail from YouTube
3. **Get Stream Manifest** - Fetches all available quality options
4. **Group by Quality** - Organizes streams by quality level
5. **Present Options** - Shows user available quality selections
6. **Download Stream** - Downloads selected stream with progress tracking
7. **Convert/Save** - Uses FFmpeg to convert/save in requested format

## Configuration

### Registry Storage
Download path is saved at:
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\DownloadYoutubeVideosApp
```

### FFmpeg Setup
- App checks system `PATH` for FFmpeg
- Auto-downloads if not found
- Caches for future use

## Error Handling

The app handles these errors gracefully:
- Invalid YouTube URLs
- YouTube connection failures
- Unavailable quality options
- FFmpeg download issues
- File write failures
- Network interruptions

All error messages are user-friendly and actionable.

## Resource Management

- Uses `CancellationTokenSource` for cancellation control
- Supports `async/await` for non-blocking operations
- Proper resource disposal

## Localization

The app has built-in Arabic support:
- RTL (Right-to-Left) interface layout
- Full Arabic UI text
- Bilingual code comments

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-feature`)
3. Commit changes (`git commit -m 'Add new feature'`)
4. Push to branch (`git push origin feature/new-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Mohamed**

## Support

- 📧 Email: [mohamedrabie3473@gmail.com]
- 🐙 LinkedIn: [https://www.linkedin.com/in/mohamed-rabie-123582231/]

## Acknowledgments

Special thanks to:
- **YoutubeExplode** - YouTube data access
- **Xabe.FFmpeg** - Video/audio processing
- **AngleSharp** - HTML parsing

## Security

- Only download path is stored locally
- No account/password required
- No personal data collection
- All operations run locally

---

**Enjoy downloading! 🎉**