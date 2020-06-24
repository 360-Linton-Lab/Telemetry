namespace TELEMETRY.lib
{
    public class DownloadResult
    {
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
        public bool FileExists { get; set; }
        public long FileLength { get; set; }
        public long BytesDownloaded { get; set; }
        public long TimeTakenMs { get; set; }
        public int ParallelDownloads { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsOperationSuccess { get; set; }
    }
}