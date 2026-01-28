using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// In-memory cache manager using MemoryCache
    /// Prevents repeated database queries for frequently accessed data
    /// </summary>
    public class CacheManager
    {
        private static CacheManager instance;
        private static readonly object lockObject = new object();
        private readonly MemoryCache cache;
        private readonly Dictionary<string, DateTime> cacheTimestamps;

        // Default expiration times for different data types
        public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan ShortExpiration = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan MediumExpiration = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan LongExpiration = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan VeryLongExpiration = TimeSpan.FromHours(1);

        private CacheManager()
        {
            cache = MemoryCache.Default;
            cacheTimestamps = new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new CacheManager();
                        }
                    }
                }
                return instance;
            }
        }

        #region Get/Set Methods

        /// <summary>
        /// Get cached item
        /// </summary>
        public T Get<T>(string key) where T : class
        {
            lock (lockObject)
            {
                var item = cache.Get(key);
                if (item != null)
                {
                    Logger.Log($"Cache HIT: {key}", LogLevel.Debug);
                    return item as T;
                }
                
                Logger.Log($"Cache MISS: {key}", LogLevel.Debug);
                return null;
            }
        }

        /// <summary>
        /// Set cached item with expiration
        /// </summary>
        public void Set<T>(string key, T value, TimeSpan expiration) where T : class
        {
            lock (lockObject)
            {
                if (value == null)
                {
                    Logger.Log($"Cache SET skipped (null value): {key}", LogLevel.Warning);
                    return;
                }

                var policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.Add(expiration),
                    RemovedCallback = OnCacheRemoved
                };

                cache.Set(key, value, policy);
                cacheTimestamps[key] = DateTime.Now;
                
                Logger.Log($"Cache SET: {key} (expires in {expiration.TotalMinutes:F1} minutes)", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Set cached item with default expiration
        /// </summary>
        public void Set<T>(string key, T value) where T : class
        {
            Set(key, value, DefaultExpiration);
        }

        #endregion

        #region Remove Methods

        /// <summary>
        /// Remove single item from cache
        /// </summary>
        public void Remove(string key)
        {
            lock (lockObject)
            {
                cache.Remove(key);
                cacheTimestamps.Remove(key);
                Logger.Log($"Cache REMOVE: {key}", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Remove items matching pattern (e.g., "invoices_*")
        /// </summary>
        public void RemoveByPattern(string pattern)
        {
            lock (lockObject)
            {
                var keysToRemove = cache
                    .Where(kvp => MatchesPattern(kvp.Key, pattern))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    cache.Remove(key);
                    cacheTimestamps.Remove(key);
                }

                Logger.Log($"Cache REMOVE by pattern: {pattern} ({keysToRemove.Count} items)", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Clear all cache
        /// </summary>
        public void Clear()
        {
            lock (lockObject)
            {
                var keys = cache.Select(kvp => kvp.Key).ToList();
                foreach (var key in keys)
                {
                    cache.Remove(key);
                }
                
                cacheTimestamps.Clear();
                Logger.Log($"Cache CLEAR: All items removed ({keys.Count} items)", LogLevel.Info);
            }
        }

        #endregion

        #region Specialized Cache Methods

        /// <summary>
        /// Cache services list (rarely changes)
        /// </summary>
        public List<T> GetOrSetServices<T>(Func<List<T>> loadFunc) where T : class
        {
            const string key = "services_all";
            var cached = Get<List<T>>(key);
            
            if (cached != null)
                return cached;

            var data = loadFunc();
            Set(key, data, LongExpiration); // 30 minutes
            return data;
        }

        /// <summary>
        /// Cache medicines list (rarely changes)
        /// </summary>
        public List<T> GetOrSetMedicines<T>(Func<List<T>> loadFunc) where T : class
        {
            const string key = "medicines_all";
            var cached = Get<List<T>>(key);
            
            if (cached != null)
                return cached;

            var data = loadFunc();
            Set(key, data, LongExpiration); // 30 minutes
            return data;
        }

        /// <summary>
        /// Cache staff list (rarely changes)
        /// </summary>
        public List<T> GetOrSetStaff<T>(Func<List<T>> loadFunc) where T : class
        {
            const string key = "staff_all";
            var cached = Get<List<T>>(key);
            
            if (cached != null)
                return cached;

            var data = loadFunc();
            Set(key, data, MediumExpiration); // 10 minutes
            return data;
        }

        /// <summary>
        /// Cache patients list (changes frequently)
        /// </summary>
        public List<T> GetOrSetPatients<T>(Func<List<T>> loadFunc) where T : class
        {
            const string key = "patients_all";
            var cached = Get<List<T>>(key);
            
            if (cached != null)
                return cached;

            var data = loadFunc();
            Set(key, data, ShortExpiration); // 1 minute
            return data;
        }

        /// <summary>
        /// Invalidate related caches when data changes
        /// </summary>
        public void InvalidateRelated(string entityType)
        {
            switch (entityType.ToLower())
            {
                case "invoice":
                    RemoveByPattern("invoices_*");
                    RemoveByPattern("dashboard_stats_*");
                    break;

                case "appointment":
                    RemoveByPattern("appointments_*");
                    RemoveByPattern("dashboard_stats_*");
                    break;

                case "patient":
                    RemoveByPattern("patients_*");
                    RemoveByPattern("invoices_*");
                    RemoveByPattern("appointments_*");
                    RemoveByPattern("medical_records_*");
                    RemoveByPattern("dashboard_stats_*");
                    break;

                case "service":
                    Remove("services_all");
                    RemoveByPattern("appointments_*");
                    break;

                case "medicine":
                    Remove("medicines_all");
                    RemoveByPattern("prescriptions_*");
                    break;

                case "staff":
                    Remove("staff_all");
                    RemoveByPattern("appointments_*");
                    RemoveByPattern("medical_records_*");
                    break;

                case "medicalrecord":
                    RemoveByPattern("medical_records_*");
                    break;

                default:
                    Logger.Log($"Unknown entity type for cache invalidation: {entityType}", LogLevel.Warning);
                    break;
            }

            Logger.Log($"Cache invalidated for entity: {entityType}", LogLevel.Info);
        }

        #endregion

        #region Cache Statistics

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public CacheStatistics GetStatistics()
        {
            lock (lockObject)
            {
                var stats = new CacheStatistics
                {
                    TotalItems = cache.Count(),
                    TotalMemorySize = cache.GetCacheMemoryLimit(),
                    Items = new List<CacheItemInfo>()
                };

                foreach (var kvp in cache)
                {
                    var key = kvp.Key;
                    var timestamp = cacheTimestamps.ContainsKey(key) ? cacheTimestamps[key] : DateTime.MinValue;
                    
                    stats.Items.Add(new CacheItemInfo
                    {
                        Key = key,
                        CachedAt = timestamp,
                        Age = DateTime.Now - timestamp
                    });
                }

                return stats;
            }
        }

        /// <summary>
        /// Log cache statistics
        /// </summary>
        public void LogStatistics()
        {
            var stats = GetStatistics();
            Logger.Log($"=== Cache Statistics ===", LogLevel.Info);
            Logger.Log($"Total Items: {stats.TotalItems}", LogLevel.Info);
            Logger.Log($"Memory Limit: {stats.TotalMemorySize / 1024 / 1024} MB", LogLevel.Info);
            
            foreach (var item in stats.Items.OrderByDescending(i => i.Age))
            {
                Logger.Log($"  - {item.Key}: {item.Age.TotalMinutes:F1} min old", LogLevel.Debug);
            }
        }

        #endregion

        #region Private Methods

        private bool MatchesPattern(string key, string pattern)
        {
            if (pattern.EndsWith("*"))
            {
                return key.StartsWith(pattern.Substring(0, pattern.Length - 1));
            }
            
            if (pattern.StartsWith("*"))
            {
                return key.EndsWith(pattern.Substring(1));
            }
            
            if (pattern.Contains("*"))
            {
                var parts = pattern.Split('*');
                return key.StartsWith(parts[0]) && key.EndsWith(parts[1]);
            }
            
            return key == pattern;
        }

        private void OnCacheRemoved(CacheEntryRemovedArguments args)
        {
            lock (lockObject)
            {
                cacheTimestamps.Remove(args.CacheItem.Key);
            }
            
            Logger.Log($"Cache EXPIRED: {args.CacheItem.Key} (Reason: {args.RemovedReason})", LogLevel.Debug);
        }

        #endregion
    }

    #region Cache Statistics Classes

    public class CacheStatistics
    {
        public long TotalItems { get; set; }
        public long TotalMemorySize { get; set; }
        public List<CacheItemInfo> Items { get; set; }
    }

    public class CacheItemInfo
    {
        public string Key { get; set; }
        public DateTime CachedAt { get; set; }
        public TimeSpan Age { get; set; }
    }

    #endregion
}
