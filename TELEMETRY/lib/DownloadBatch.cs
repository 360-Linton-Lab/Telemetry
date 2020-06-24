using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TELEMETRY.lib
{
    public class DownloadBatch
    {
        public int BatchId { get; set; }

        public Dictionary<string, DownloadFileInfo> Downloads { get; set; }
    }
}
