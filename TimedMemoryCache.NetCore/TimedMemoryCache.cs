﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TimedMemoryCache.NetCore
{
    public class TimedMemoryCache : MemoryCache.NetCore.MemoryCache
    {
        private readonly ConcurrentDictionary<string, CacheTimingEntry> _timing;
        public event TimeoutCallback OnTimeout;
        private readonly Timer _timeout;
        private readonly long _default;
        private bool _overload;
        private long _period = 1;

        public TimedMemoryCache(long seconds)
        {
            _timing = new ConcurrentDictionary<string, CacheTimingEntry>();
            _timeout = new Timer(Timeout_Elapsed, null, 0, 1000);
            _default = seconds;
        }

        public new dynamic this[string key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                if (_timing.ContainsKey(key))
                    _timing.TryUpdate(key, new CacheTimingEntry()
                    {
                        Timestamp = DateTime.UtcNow.ToBinary(),
                        Timeout = _default
                    }, _timing[key]);
                else
                    _timing.TryAdd(key, new CacheTimingEntry()
                    {
                        Timestamp = DateTime.UtcNow.ToBinary(),
                        Timeout = _default
                    });
            }
        }

        public new void Write(string key, dynamic value)
        {
            // Cast fix for IL not knowing which base to use.
            ((MemoryCache.NetCore.MemoryCache)this).Write(key, value);
            if (_timing.ContainsKey(key))
                _timing.TryUpdate(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = _default
                }, _timing[key]);
            else
                _timing.TryAdd(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = _default
                });
        }

        public new T Write<T>(string key, T value)
        {
            // Cast fix for IL not knowing which base to use.
            ((MemoryCache.NetCore.MemoryCache)this).Write(key, value);
            if (_timing.ContainsKey(key))
                _timing.TryUpdate(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = _default
                }, _timing[key]);
            else
                _timing.TryAdd(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = _default
                });

            return base[key];
        }

        public void Write(string key, dynamic value, long timeout)
        {
            // Cast fix for IL not knowing which base to use.
            ((MemoryCache.NetCore.MemoryCache)this).Write(key, value);
            if (timeout == 0)
                return;

            if (_timing.ContainsKey(key))
                _timing.TryUpdate(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = timeout
                }, _timing[key]);
            else
                _timing.TryAdd(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = timeout
                });
        }

        public T Write<T>(string key, T value, long timeout)
        {
            // Cast fix for IL not knowing which base to use.
            ((MemoryCache.NetCore.MemoryCache)this).Write(key, value);
            if (timeout == 0)
                return base[key];

            if (_timing.ContainsKey(key))
                _timing.TryUpdate(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = timeout
                }, _timing[key]);
            else
                _timing.TryAdd(key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = timeout
                });

            return base[key];
        }

        public new void Delete(string key)
        {
            base.Delete(key);
            _timing.TryRemove(key, out _);
        }

        public new T Delete<T>(string key)
        {
            var entry = base.Delete<T>(key);
            _timing.TryRemove(key, out _);

            return entry;
        }

        public new void Load<T>(T data, bool clear = true)
        {
            if (clear)
                _timing.Clear();

            base.Load(data, clear);
            foreach (var entry in this)
                _timing.TryAdd(entry.Key, new CacheTimingEntry()
                {
                    Timestamp = DateTime.UtcNow.ToBinary(),
                    Timeout = _default
                });
        }

        private void Timeout_Elapsed(object state)
        {
            // This is for very large caches, ideally we want to process all data in the cache in a single tick
            // if we are unable too, slowly start raising the interval until we can.
            if (_overload)
            {
                _period++;
                _timeout.Change(0, _period * 1000);
                return;
            }

            _overload = true;
            foreach (var (key, value) in _timing)
            {
                var time = DateTime.FromBinary(value.Timestamp);
                if ((DateTime.UtcNow - time).TotalSeconds < value.Timeout)
                    continue;

                var entry = base.Delete<dynamic>(key);
                _timing.TryRemove(key, out _);

                OnTimeout?.Invoke(this, key, entry, value.Timeout);
            }

            _overload = false;
        }
    }
}
