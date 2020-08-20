using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TimedMemoryCache.NetCore
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct CacheTimingEntry
    {
        public long Timestamp { get; set; }
        public long Timeout { get; set; }
    }
}
