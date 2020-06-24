using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TELEMETRY.lib
{
    public class BandwidthMeasure
    {
        public long ProgressBytes { get; set; }
        public long TotalBytes { get; set; }
        public long ElapsedMs { get; set; }
    }

    public class Bandwidth
    {
        public string FileUrl { get; set; }

        public long Mean1Second { get; set; }
        public long Mean5Seconds { get; set; }
        public long Mean30Seconds { get; set; }
        public long Mean1Minute { get; set; }
        public long? Remaining { get; set; }

        public List<BandwidthMeasure> Measures { get; set; }
    }
}
