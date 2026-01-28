using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Query optimizer to prevent N+1 problems and improve performance
    /// Uses batch loading, JOIN queries, and query result caching
    /// </summary>
    public class QueryOptimizer
    {
        private readonly DatabaseHelper dbHelper;
        private readonly CacheManager cacheManager;
        private static readonly object lockObject = new object();

        public QueryOptimizer()
        {
            dbHelper = new DatabaseHelper();
            cacheManager = CacheManager.Instance;
        }

        #region Batch Loading (Prevent N+1)

        /// <summary>
        /// Load invoices with related data in single query (JOIN)
        /// BEFORE: 1 + N queries (1 for invoices, N for patients)
        /// AFTER: 1 query with JOIN
        /// </summary>
        public async Task<List<InvoiceWithDetails>> LoadInvoicesWithDetailsAsync(string whereClause = "", object parameters = null)
        {
            string cacheKey = $"invoices_details_{whereClause}_{parameters?.GetHashCode()}";
            
            // Check cache first
            var cached = cacheManager.Get<List<InvoiceWithDetails>>(cacheKey);
            if (cached != null)
            {
                Logger.Log($"Cache HIT: {cacheKey}");
                return cached;
            }

            Logger.Log($"Cache MISS: {cacheKey}");

            // Single JOIN query instead of N+1
            string sql = @"
                SELECT 
                    i.id AS InvoiceId,
                    i.invoice_date AS InvoiceDate,
                    i.total_amount AS TotalAmount,
                    i.amount_paid AS AmountPaid,
                    i.status AS Status,
                    i.notes AS Notes,
                    i.created_at AS CreatedAt,
                    p.id AS PatientId,
                    p.full_name AS PatientName,
                    p.phone AS PatientPhone,
                    p.email AS PatientEmail,
                    s.id AS StaffId,
                    s.full_name AS StaffName
                FROM Invoices i
                INNER JOIN Patients p ON i.patient_id = p.id
                LEFT JOIN Staff s ON i.staff_id = s.id
                " + (string.IsNullOrEmpty(whereClause) ? "" : $"WHERE {whereClause}") + @"
                ORDER BY i.created_at DESC";

            using (var conn = dbHelper.GetConnection())
            {
                var result = await conn.QueryAsync<InvoiceWithDetails>(sql, parameters);
                var list = result.ToList();

                // Cache for 5 minutes
                cacheManager.Set(cacheKey, list, TimeSpan.FromMinutes(5));

                return list;
            }
        }

        /// <summary>
        /// Load appointments with patient and doctor info in single query
        /// </summary>
        public async Task<List<AppointmentWithDetails>> LoadAppointmentsWithDetailsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            string cacheKey = $"appointments_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}";
            
            var cached = cacheManager.Get<List<AppointmentWithDetails>>(cacheKey);
            if (cached != null) return cached;

            string sql = @"
                SELECT 
                    a.id AS AppointmentId,
                    a.appointment_date AS AppointmentDate,
                    a.appointment_time AS AppointmentTime,
                    a.status AS Status,
                    a.notes AS Notes,
                    p.id AS PatientId,
                    p.full_name AS PatientName,
                    p.phone AS PatientPhone,
                    d.id AS DoctorId,
                    d.full_name AS DoctorName,
                    s.service_name AS ServiceName,
                    s.price AS ServicePrice
                FROM Appointments a
                INNER JOIN Patients p ON a.patient_id = p.id
                INNER JOIN Staff d ON a.doctor_id = d.id
                LEFT JOIN Services s ON a.service_id = s.id
                WHERE (@FromDate IS NULL OR a.appointment_date >= @FromDate)
                  AND (@ToDate IS NULL OR a.appointment_date <= @ToDate)
                ORDER BY a.appointment_date DESC, a.appointment_time DESC";

            using (var conn = dbHelper.GetConnection())
            {
                var result = await conn.QueryAsync<AppointmentWithDetails>(sql, new { FromDate = fromDate, ToDate = toDate });
                var list = result.ToList();

                cacheManager.Set(cacheKey, list, TimeSpan.FromMinutes(3));
                return list;
            }
        }

        /// <summary>
        /// Load medical records with prescription details in single query
        /// Uses multi-mapping to avoid N+1
        /// </summary>
        public async Task<List<MedicalRecordWithPrescriptions>> LoadMedicalRecordsWithPrescriptionsAsync(int patientId)
        {
            string cacheKey = $"medical_records_patient_{patientId}";
            
            var cached = cacheManager.Get<List<MedicalRecordWithPrescriptions>>(cacheKey);
            if (cached != null) return cached;

            string sql = @"
                SELECT 
                    mr.id AS RecordId,
                    mr.diagnosis AS Diagnosis,
                    mr.treatment AS Treatment,
                    mr.notes AS Notes,
                    mr.record_date AS RecordDate,
                    d.full_name AS DoctorName,
                    p.id AS PrescriptionId,
                    p.medicine_name AS MedicineName,
                    p.dosage AS Dosage,
                    p.quantity AS Quantity,
                    p.instructions AS Instructions
                FROM MedicalRecords mr
                INNER JOIN Staff d ON mr.doctor_id = d.id
                LEFT JOIN Prescriptions p ON p.medical_record_id = mr.id
                WHERE mr.patient_id = @PatientId
                ORDER BY mr.record_date DESC";

            using (var conn = dbHelper.GetConnection())
            {
                var recordDict = new Dictionary<int, MedicalRecordWithPrescriptions>();

                await conn.QueryAsync<MedicalRecordWithPrescriptions, PrescriptionInfo, MedicalRecordWithPrescriptions>(
                    sql,
                    (record, prescription) =>
                    {
                        if (!recordDict.TryGetValue(record.RecordId, out var recordEntry))
                        {
                            recordEntry = record;
                            recordEntry.Prescriptions = new List<PrescriptionInfo>();
                            recordDict.Add(record.RecordId, recordEntry);
                        }

                        if (prescription != null && prescription.PrescriptionId > 0)
                        {
                            recordEntry.Prescriptions.Add(prescription);
                        }

                        return recordEntry;
                    },
                    new { PatientId = patientId },
                    splitOn: "PrescriptionId"
                );

                var list = recordDict.Values.ToList();
                cacheManager.Set(cacheKey, list, TimeSpan.FromMinutes(10));
                return list;
            }
        }

        /// <summary>
        /// Batch load patients by IDs (prevent N+1)
        /// BEFORE: foreach(id) { SELECT * FROM Patients WHERE id = @id } → N queries
        /// AFTER: SELECT * FROM Patients WHERE id IN (@ids) → 1 query
        /// </summary>
        public async Task<Dictionary<int, PatientInfo>> BatchLoadPatientsAsync(IEnumerable<int> patientIds)
        {
            var ids = patientIds.Distinct().ToList();
            if (!ids.Any()) return new Dictionary<int, PatientInfo>();

            string cacheKey = $"patients_batch_{string.Join("_", ids.OrderBy(x => x))}";
            var cached = cacheManager.Get<Dictionary<int, PatientInfo>>(cacheKey);
            if (cached != null) return cached;

            string sql = @"
                SELECT 
                    id AS PatientId,
                    full_name AS FullName,
                    phone AS Phone,
                    email AS Email,
                    date_of_birth AS DateOfBirth,
                    gender AS Gender,
                    address AS Address
                FROM Patients
                WHERE id IN @Ids";

            using (var conn = dbHelper.GetConnection())
            {
                var result = await conn.QueryAsync<PatientInfo>(sql, new { Ids = ids });
                var dict = result.ToDictionary(p => p.PatientId);

                cacheManager.Set(cacheKey, dict, TimeSpan.FromMinutes(15));
                return dict;
            }
        }

        /// <summary>
        /// Batch load services by IDs
        /// </summary>
        public async Task<Dictionary<int, ServiceInfo>> BatchLoadServicesAsync(IEnumerable<int> serviceIds)
        {
            var ids = serviceIds.Distinct().ToList();
            if (!ids.Any()) return new Dictionary<int, ServiceInfo>();

            string cacheKey = $"services_batch_{string.Join("_", ids.OrderBy(x => x))}";
            var cached = cacheManager.Get<Dictionary<int, ServiceInfo>>(cacheKey);
            if (cached != null) return cached;

            string sql = @"
                SELECT 
                    id AS ServiceId,
                    service_name AS ServiceName,
                    description AS Description,
                    price AS Price,
                    duration AS Duration
                FROM Services
                WHERE id IN @Ids";

            using (var conn = dbHelper.GetConnection())
            {
                var result = await conn.QueryAsync<ServiceInfo>(sql, new { Ids = ids });
                var dict = result.ToDictionary(s => s.ServiceId);

                cacheManager.Set(cacheKey, dict, TimeSpan.FromMinutes(30));
                return dict;
            }
        }

        #endregion

        #region Query Result Caching

        /// <summary>
        /// Execute query with caching
        /// </summary>
        public async Task<T> ExecuteWithCacheAsync<T>(string cacheKey, Func<Task<T>> queryFunc, TimeSpan? expiration = null)
        {
            var cached = cacheManager.Get<T>(cacheKey);
            if (cached != null)
            {
                Logger.Log($"Cache HIT: {cacheKey}");
                return cached;
            }

            Logger.Log($"Cache MISS: {cacheKey}");
            var result = await queryFunc();

            cacheManager.Set(cacheKey, result, expiration ?? TimeSpan.FromMinutes(5));
            return result;
        }

        /// <summary>
        /// Invalidate cache when data changes
        /// </summary>
        public void InvalidateCache(string pattern)
        {
            cacheManager.RemoveByPattern(pattern);
            Logger.Log($"Cache invalidated: {pattern}");
        }

        #endregion

        #region Aggregation Queries

        /// <summary>
        /// Get dashboard statistics in single query
        /// BEFORE: 5 separate SELECT COUNT(*) → 5 queries
        /// AFTER: 1 query with multiple aggregations
        /// </summary>
        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            string cacheKey = $"dashboard_stats_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}";
            
            var cached = cacheManager.Get<DashboardStats>(cacheKey);
            if (cached != null) return cached;

            string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Patients WHERE created_at >= ISNULL(@FromDate, '1900-01-01') AND created_at <= ISNULL(@ToDate, '9999-12-31')) AS TotalPatients,
                    (SELECT COUNT(*) FROM Appointments WHERE appointment_date >= ISNULL(@FromDate, '1900-01-01') AND appointment_date <= ISNULL(@ToDate, '9999-12-31')) AS TotalAppointments,
                    (SELECT COUNT(*) FROM Appointments WHERE status = 'Completed' AND appointment_date >= ISNULL(@FromDate, '1900-01-01') AND appointment_date <= ISNULL(@ToDate, '9999-12-31')) AS CompletedAppointments,
                    (SELECT ISNULL(SUM(total_amount), 0) FROM Invoices WHERE invoice_date >= ISNULL(@FromDate, '1900-01-01') AND invoice_date <= ISNULL(@ToDate, '9999-12-31')) AS TotalRevenue,
                    (SELECT ISNULL(SUM(amount_paid), 0) FROM Invoices WHERE invoice_date >= ISNULL(@FromDate, '1900-01-01') AND invoice_date <= ISNULL(@ToDate, '9999-12-31')) AS PaidAmount,
                    (SELECT COUNT(*) FROM Invoices WHERE status = 'Unpaid' AND invoice_date >= ISNULL(@FromDate, '1900-01-01') AND invoice_date <= ISNULL(@ToDate, '9999-12-31')) AS UnpaidInvoices";

            using (var conn = dbHelper.GetConnection())
            {
                var result = await conn.QueryFirstOrDefaultAsync<DashboardStats>(sql, new { FromDate = fromDate, ToDate = toDate });
                
                cacheManager.Set(cacheKey, result, TimeSpan.FromMinutes(2));
                return result;
            }
        }

        #endregion
    }

    #region DTOs for Optimized Queries

    public class InvoiceWithDetails
    {
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Patient info (no separate query needed)
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public string PatientEmail { get; set; }
        
        // Staff info
        public int StaffId { get; set; }
        public string StaffName { get; set; }
    }

    public class AppointmentWithDetails
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        
        public string ServiceName { get; set; }
        public decimal ServicePrice { get; set; }
    }

    public class MedicalRecordWithPrescriptions
    {
        public int RecordId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public DateTime RecordDate { get; set; }
        public string DoctorName { get; set; }
        
        public List<PrescriptionInfo> Prescriptions { get; set; }
    }

    public class PrescriptionInfo
    {
        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
    }

    public class PatientInfo
    {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }

    public class ServiceInfo
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }

    public class DashboardStats
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PaidAmount { get; set; }
        public int UnpaidInvoices { get; set; }
    }

    #endregion
}
