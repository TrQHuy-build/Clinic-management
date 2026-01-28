using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Performance monitor to track query execution times and identify bottlenecks
    /// </summary>
    public class PerformanceMonitor
    {
        private static PerformanceMonitor instance;
        private static readonly object lockObject = new object();
        
        private readonly Dictionary<string, QueryPerformanceData> queryStats;
        private readonly List<QueryExecutionLog> executionLogs;
        private readonly int maxLogsToKeep = 1000;

        private PerformanceMonitor()
        {
            queryStats = new Dictionary<string, QueryPerformanceData>();
            executionLogs = new List<QueryExecutionLog>();
        }

        public static PerformanceMonitor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new PerformanceMonitor();
                        }
                    }
                }
                return instance;
            }
        }

        #region Tracking Methods

        /// <summary>
        /// Start tracking a query execution
        /// </summary>
        public IDisposable TrackQuery(string queryName, string description = "")
        {
            return new QueryTracker(this, queryName, description);
        }

        /// <summary>
        /// Record query execution
        /// </summary>
        internal void RecordExecution(string queryName, string description, long elapsedMs, bool fromCache)
        {
            lock (lockObject)
            {
                // Update stats
                if (!queryStats.ContainsKey(queryName))
                {
                    queryStats[queryName] = new QueryPerformanceData
                    {
                        QueryName = queryName,
                        Description = description
                    };
                }

                var stats = queryStats[queryName];
                stats.ExecutionCount++;
                stats.TotalExecutionTime += elapsedMs;
                stats.AverageExecutionTime = stats.TotalExecutionTime / stats.ExecutionCount;
                
                if (elapsedMs < stats.MinExecutionTime || stats.MinExecutionTime == 0)
                    stats.MinExecutionTime = elapsedMs;
                
                if (elapsedMs > stats.MaxExecutionTime)
                    stats.MaxExecutionTime = elapsedMs;

                if (fromCache)
                    stats.CacheHits++;
                else
                    stats.CacheMisses++;

                stats.LastExecutedAt = DateTime.Now;

                // Add to execution log
                var log = new QueryExecutionLog
                {
                    QueryName = queryName,
                    Description = description,
                    ExecutionTime = elapsedMs,
                    ExecutedAt = DateTime.Now,
                    FromCache = fromCache
                };

                executionLogs.Add(log);

                // Keep only recent logs
                if (executionLogs.Count > maxLogsToKeep)
                {
                    executionLogs.RemoveRange(0, executionLogs.Count - maxLogsToKeep);
                }

                // Log slow queries (> 1 second)
                if (elapsedMs > 1000 && !fromCache)
                {
                    Logger.Log($"⚠ SLOW QUERY: {queryName} took {elapsedMs}ms", LogLevel.Warning);
                }
            }
        }

        #endregion

        #region Reporting Methods

        /// <summary>
        /// Get all query statistics
        /// </summary>
        public List<QueryPerformanceData> GetStatistics()
        {
            lock (lockObject)
            {
                return queryStats.Values.OrderByDescending(s => s.TotalExecutionTime).ToList();
            }
        }

        /// <summary>
        /// Get slow queries (average > 500ms)
        /// </summary>
        public List<QueryPerformanceData> GetSlowQueries()
        {
            lock (lockObject)
            {
                return queryStats.Values
                    .Where(s => s.AverageExecutionTime > 500)
                    .OrderByDescending(s => s.AverageExecutionTime)
                    .ToList();
            }
        }

        /// <summary>
        /// Get most executed queries
        /// </summary>
        public List<QueryPerformanceData> GetMostExecutedQueries(int top = 10)
        {
            lock (lockObject)
            {
                return queryStats.Values
                    .OrderByDescending(s => s.ExecutionCount)
                    .Take(top)
                    .ToList();
            }
        }

        /// <summary>
        /// Get recent execution logs
        /// </summary>
        public List<QueryExecutionLog> GetRecentLogs(int count = 100)
        {
            lock (lockObject)
            {
                return executionLogs
                    .OrderByDescending(l => l.ExecutedAt)
                    .Take(count)
                    .ToList();
            }
        }

        /// <summary>
        /// Get cache effectiveness
        /// </summary>
        public CacheEffectiveness GetCacheEffectiveness()
        {
            lock (lockObject)
            {
                long totalHits = queryStats.Values.Sum(s => s.CacheHits);
                long totalMisses = queryStats.Values.Sum(s => s.CacheMisses);
                long total = totalHits + totalMisses;

                return new CacheEffectiveness
                {
                    TotalRequests = total,
                    CacheHits = totalHits,
                    CacheMisses = totalMisses,
                    HitRate = total > 0 ? (double)totalHits / total * 100 : 0
                };
            }
        }

        /// <summary>
        /// Log performance summary
        /// </summary>
        public void LogSummary()
        {
            var stats = GetStatistics();
            var slowQueries = GetSlowQueries();
            var cacheStats = GetCacheEffectiveness();

            Logger.Log("=== Performance Summary ===", LogLevel.Info);
            Logger.Log($"Total Queries Tracked: {stats.Count}", LogLevel.Info);
            Logger.Log($"Total Executions: {stats.Sum(s => s.ExecutionCount)}", LogLevel.Info);
            Logger.Log($"Cache Hit Rate: {cacheStats.HitRate:F1}%", LogLevel.Info);
            Logger.Log($"Slow Queries (>500ms): {slowQueries.Count}", LogLevel.Info);

            if (slowQueries.Any())
            {
                Logger.Log("\n=== Slow Queries ===", LogLevel.Warning);
                foreach (var query in slowQueries.Take(5))
                {
                    Logger.Log($"  {query.QueryName}: Avg={query.AverageExecutionTime}ms, Max={query.MaxExecutionTime}ms, Count={query.ExecutionCount}", LogLevel.Warning);
                }
            }

            var topQueries = GetMostExecutedQueries(5);
            if (topQueries.Any())
            {
                Logger.Log("\n=== Most Executed Queries ===", LogLevel.Info);
                foreach (var query in topQueries)
                {
                    Logger.Log($"  {query.QueryName}: {query.ExecutionCount} times, Avg={query.AverageExecutionTime}ms", LogLevel.Info);
                }
            }
        }

        /// <summary>
        /// Reset all statistics
        /// </summary>
        public void Reset()
        {
            lock (lockObject)
            {
                queryStats.Clear();
                executionLogs.Clear();
                Logger.Log("Performance statistics reset", LogLevel.Info);
            }
        }

        #endregion
    }

    #region Query Tracker

    /// <summary>
    /// Disposable query tracker for using() blocks
    /// </summary>
    internal class QueryTracker : IDisposable
    {
        private readonly PerformanceMonitor monitor;
        private readonly string queryName;
        private readonly string description;
        private readonly Stopwatch stopwatch;
        private bool fromCache;

        public QueryTracker(PerformanceMonitor monitor, string queryName, string description)
        {
            this.monitor = monitor;
            this.queryName = queryName;
            this.description = description;
            this.stopwatch = Stopwatch.StartNew();
            this.fromCache = false;
        }

        public void MarkAsCached()
        {
            fromCache = true;
        }

        public void Dispose()
        {
            stopwatch.Stop();
            monitor.RecordExecution(queryName, description, stopwatch.ElapsedMilliseconds, fromCache);
        }
    }

    #endregion

    #region Data Classes

    public class QueryPerformanceData
    {
        public string QueryName { get; set; }
        public string Description { get; set; }
        public long ExecutionCount { get; set; }
        public long TotalExecutionTime { get; set; }
        public long AverageExecutionTime { get; set; }
        public long MinExecutionTime { get; set; }
        public long MaxExecutionTime { get; set; }
        public long CacheHits { get; set; }
        public long CacheMisses { get; set; }
        public DateTime LastExecutedAt { get; set; }

        public double CacheHitRate => (CacheHits + CacheMisses) > 0 
            ? (double)CacheHits / (CacheHits + CacheMisses) * 100 
            : 0;
    }

    public class QueryExecutionLog
    {
        public string QueryName { get; set; }
        public string Description { get; set; }
        public long ExecutionTime { get; set; }
        public DateTime ExecutedAt { get; set; }
        public bool FromCache { get; set; }
    }

    public class CacheEffectiveness
    {
        public long TotalRequests { get; set; }
        public long CacheHits { get; set; }
        public long CacheMisses { get; set; }
        public double HitRate { get; set; }
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// Extension methods for easy performance tracking
    /// </summary>
    public static class PerformanceExtensions
    {
        /// <summary>
        /// Execute with performance tracking
        /// </summary>
        public static T ExecuteWithTracking<T>(this Func<T> func, string queryName, string description = "")
        {
            using (PerformanceMonitor.Instance.TrackQuery(queryName, description))
            {
                return func();
            }
        }

        /// <summary>
        /// Execute async with performance tracking
        /// </summary>
        public static async System.Threading.Tasks.Task<T> ExecuteWithTrackingAsync<T>(
            this System.Threading.Tasks.Task<T> task, 
            string queryName, 
            string description = "")
        {
            using (PerformanceMonitor.Instance.TrackQuery(queryName, description))
            {
                return await task;
            }
        }
    }

    #endregion
}
