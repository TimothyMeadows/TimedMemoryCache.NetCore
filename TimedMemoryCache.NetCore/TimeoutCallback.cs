using System;
using System.Collections.Generic;
using System.Text;
using MemoryCache.NetCore;

namespace TimedMemoryCache.NetCore
{
    public delegate void TimeoutCallback(TimedMemoryCache source, string key, dynamic value);
}
