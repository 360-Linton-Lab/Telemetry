namespace TELEMETRY.lib
{
    public class DownloadFileInfo
    {
        public string FileUrl { get; set; }

        public bool IsOperationSuccess { get; set; }

        public bool Exists { get; set; }

        public long Length { get; set; }

        public bool IsSupportedHead { get; set; }

        public bool IsSupportedRange { get; set; }
    }
}
