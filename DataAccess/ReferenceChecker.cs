using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DentalClinicManagement.DataAccess
{
    /// <summary>
    /// Helper class to check if records are in use before deletion
    /// Prevents deletion of services/medicines that are referenced by other records
    /// Issue #9: Prevent Deletion of In-Use Records
    /// </summary>
    public class ReferenceChecker
    {
        private readonly string _connectionString;

        public ReferenceChecker(string connectionString = null)
        {
            _connectionString = connectionString ?? ConfigManager.GetConnectionString("DefaultConnection");
        }

        #region Service Reference Checks

        /// <summary>
        /// Check if a service is in use (referenced by appointments or invoices)
        /// </summary>
        /// <param name="serviceId">Service ID to check</param>
        /// <returns>True if service is in use</returns>
        public bool IsServiceInUse(int serviceId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            CASE 
                                WHEN EXISTS (
                                    SELECT 1 FROM Appointments 
                                    WHERE service_id = @ServiceId AND is_deleted = 0
                                ) THEN 1
                                WHEN EXISTS (
                                    SELECT 1 FROM InvoiceDetails 
                                    WHERE service_id = @ServiceId AND is_deleted = 0
                                ) THEN 1
                                ELSE 0
                            END AS IsInUse";

                    bool isInUse = connection.QueryFirstOrDefault<bool>(sql, new { ServiceId = serviceId });
                    return isInUse;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking if service {serviceId} is in use", ex);
                throw;
            }
        }

        /// <summary>
        /// Get detailed usage information for a service
        /// </summary>
        /// <param name="serviceId">Service ID</param>
        /// <returns>ServiceUsageInfo object</returns>
        public ServiceUsageInfo GetServiceUsageInfo(int serviceId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Appointments 
                             WHERE service_id = @ServiceId AND is_deleted = 0) AS AppointmentCount,
                            (SELECT COUNT(*) FROM InvoiceDetails 
                             WHERE service_id = @ServiceId AND is_deleted = 0) AS InvoiceDetailCount,
                            (SELECT COUNT(*) FROM Appointments 
                             WHERE service_id = @ServiceId AND is_deleted = 0 
                             AND appointment_date >= GETDATE()) AS UpcomingAppointmentCount";

                    var result = connection.QueryFirstOrDefault<dynamic>(sql, new { ServiceId = serviceId });

                    return new ServiceUsageInfo
                    {
                        ServiceId = serviceId,
                        AppointmentCount = result.AppointmentCount,
                        InvoiceDetailCount = result.InvoiceDetailCount,
                        UpcomingAppointmentCount = result.UpcomingAppointmentCount,
                        IsInUse = (result.AppointmentCount > 0 || result.InvoiceDetailCount > 0)
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting usage info for service {serviceId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of appointments using a service
        /// </summary>
        /// <param name="serviceId">Service ID</param>
        /// <returns>DataTable with appointment details</returns>
        public DataTable GetAppointmentsUsingService(int serviceId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            a.id,
                            a.appointment_code,
                            p.full_name AS patient_name,
                            s.full_name AS doctor_name,
                            a.appointment_date,
                            a.time_slot,
                            a.status
                        FROM Appointments a
                        INNER JOIN Patients p ON a.patient_id = p.id
                        INNER JOIN Staff s ON a.doctor_id = s.id
                        WHERE a.service_id = @ServiceId 
                        AND a.is_deleted = 0
                        ORDER BY a.appointment_date DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@ServiceId", serviceId);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting appointments for service {serviceId}", ex);
                throw;
            }
        }

        #endregion

        #region Medicine Reference Checks

        /// <summary>
        /// Check if a medicine is in use (referenced by prescriptions or inventory)
        /// </summary>
        /// <param name="medicineId">Medicine ID to check</param>
        /// <returns>True if medicine is in use</returns>
        public bool IsMedicineInUse(int medicineId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            CASE 
                                WHEN EXISTS (
                                    SELECT 1 FROM Prescriptions 
                                    WHERE medicine_id = @MedicineId AND is_deleted = 0
                                ) THEN 1
                                WHEN EXISTS (
                                    SELECT 1 FROM Inventory 
                                    WHERE medicine_id = @MedicineId AND is_deleted = 0
                                ) THEN 1
                                ELSE 0
                            END AS IsInUse";

                    bool isInUse = connection.QueryFirstOrDefault<bool>(sql, new { MedicineId = medicineId });
                    return isInUse;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking if medicine {medicineId} is in use", ex);
                throw;
            }
        }

        /// <summary>
        /// Get detailed usage information for a medicine
        /// </summary>
        /// <param name="medicineId">Medicine ID</param>
        /// <returns>MedicineUsageInfo object</returns>
        public MedicineUsageInfo GetMedicineUsageInfo(int medicineId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Prescriptions 
                             WHERE medicine_id = @MedicineId AND is_deleted = 0) AS PrescriptionCount,
                            (SELECT COUNT(*) FROM Inventory 
                             WHERE medicine_id = @MedicineId AND is_deleted = 0) AS InventoryRecordCount,
                            (SELECT ISNULL(SUM(quantity), 0) FROM Inventory 
                             WHERE medicine_id = @MedicineId AND is_deleted = 0) AS TotalStockQuantity,
                            (SELECT COUNT(DISTINCT p.patient_id) FROM Prescriptions p
                             WHERE p.medicine_id = @MedicineId AND p.is_deleted = 0) AS PatientCount";

                    var result = connection.QueryFirstOrDefault<dynamic>(sql, new { MedicineId = medicineId });

                    return new MedicineUsageInfo
                    {
                        MedicineId = medicineId,
                        PrescriptionCount = result.PrescriptionCount,
                        InventoryRecordCount = result.InventoryRecordCount,
                        TotalStockQuantity = result.TotalStockQuantity,
                        PatientCount = result.PatientCount,
                        IsInUse = (result.PrescriptionCount > 0 || result.InventoryRecordCount > 0)
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting usage info for medicine {medicineId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of prescriptions using a medicine
        /// </summary>
        /// <param name="medicineId">Medicine ID</param>
        /// <returns>DataTable with prescription details</returns>
        public DataTable GetPrescriptionsUsingMedicine(int medicineId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            pr.id,
                            p.full_name AS patient_name,
                            mr.visit_date,
                            s.full_name AS doctor_name,
                            pr.dosage,
                            pr.frequency,
                            pr.duration
                        FROM Prescriptions pr
                        INNER JOIN MedicalRecords mr ON pr.record_id = mr.id
                        INNER JOIN Patients p ON mr.patient_id = p.id
                        INNER JOIN Staff s ON mr.doctor_id = s.id
                        WHERE pr.medicine_id = @MedicineId 
                        AND pr.is_deleted = 0
                        ORDER BY mr.visit_date DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@MedicineId", medicineId);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting prescriptions for medicine {medicineId}", ex);
                throw;
            }
        }

        #endregion

        #region Patient Reference Checks

        /// <summary>
        /// Check if a patient has any active records
        /// </summary>
        /// <param name="patientId">Patient ID to check</param>
        /// <returns>True if patient has active records</returns>
        public bool IsPatientInUse(int patientId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            CASE 
                                WHEN EXISTS (
                                    SELECT 1 FROM Appointments 
                                    WHERE patient_id = @PatientId AND is_deleted = 0
                                ) THEN 1
                                WHEN EXISTS (
                                    SELECT 1 FROM Invoices 
                                    WHERE patient_id = @PatientId AND is_deleted = 0
                                ) THEN 1
                                WHEN EXISTS (
                                    SELECT 1 FROM MedicalRecords 
                                    WHERE patient_id = @PatientId AND is_deleted = 0
                                ) THEN 1
                                ELSE 0
                            END AS IsInUse";

                    bool isInUse = connection.QueryFirstOrDefault<bool>(sql, new { PatientId = patientId });
                    return isInUse;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking if patient {patientId} is in use", ex);
                throw;
            }
        }

        /// <summary>
        /// Get patient usage summary
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <returns>PatientUsageInfo object</returns>
        public PatientUsageInfo GetPatientUsageInfo(int patientId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            (SELECT COUNT(*) FROM Appointments 
                             WHERE patient_id = @PatientId AND is_deleted = 0) AS AppointmentCount,
                            (SELECT COUNT(*) FROM Invoices 
                             WHERE patient_id = @PatientId AND is_deleted = 0) AS InvoiceCount,
                            (SELECT COUNT(*) FROM MedicalRecords 
                             WHERE patient_id = @PatientId AND is_deleted = 0) AS MedicalRecordCount,
                            (SELECT COUNT(*) FROM Appointments 
                             WHERE patient_id = @PatientId AND is_deleted = 0 
                             AND appointment_date >= GETDATE()) AS UpcomingAppointmentCount";

                    var result = connection.QueryFirstOrDefault<dynamic>(sql, new { PatientId = patientId });

                    return new PatientUsageInfo
                    {
                        PatientId = patientId,
                        AppointmentCount = result.AppointmentCount,
                        InvoiceCount = result.InvoiceCount,
                        MedicalRecordCount = result.MedicalRecordCount,
                        UpcomingAppointmentCount = result.UpcomingAppointmentCount,
                        IsInUse = (result.AppointmentCount > 0 || result.InvoiceCount > 0 || result.MedicalRecordCount > 0)
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting usage info for patient {patientId}", ex);
                throw;
            }
        }

        #endregion

        #region Staff Reference Checks

        /// <summary>
        /// Check if a staff member has any active records
        /// </summary>
        /// <param name="staffId">Staff ID to check</param>
        /// <returns>True if staff has active records</returns>
        public bool IsStaffInUse(int staffId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT 
                            CASE 
                                WHEN EXISTS (
                                    SELECT 1 FROM Appointments 
                                    WHERE doctor_id = @StaffId AND is_deleted = 0
                                ) THEN 1
                                WHEN EXISTS (
                                    SELECT 1 FROM MedicalRecords 
                                    WHERE doctor_id = @StaffId AND is_deleted = 0
                                ) THEN 1
                                WHEN EXISTS (
                                    SELECT 1 FROM Users 
                                    WHERE staff_id = @StaffId AND is_deleted = 0
                                ) THEN 1
                                ELSE 0
                            END AS IsInUse";

                    bool isInUse = connection.QueryFirstOrDefault<bool>(sql, new { StaffId = staffId });
                    return isInUse;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking if staff {staffId} is in use", ex);
                throw;
            }
        }

        #endregion

        #region Safe Delete Methods

        /// <summary>
        /// Safely soft delete a service with validation
        /// </summary>
        /// <param name="serviceId">Service ID</param>
        /// <param name="deletedBy">User who is deleting</param>
        /// <param name="forceDelete">Force delete even if in use (deactivate only)</param>
        /// <returns>Result object</returns>
        public DeleteResult SafeDeleteService(int serviceId, string deletedBy, bool forceDelete = false)
        {
            try
            {
                var usageInfo = GetServiceUsageInfo(serviceId);

                if (usageInfo.IsInUse && !forceDelete)
                {
                    return new DeleteResult
                    {
                        Success = false,
                        Message = $"Không thể xóa dịch vụ này!\n\n" +
                                 $"Đang được sử dụng bởi:\n" +
                                 $"- {usageInfo.AppointmentCount} lịch hẹn\n" +
                                 $"- {usageInfo.InvoiceDetailCount} hóa đơn\n\n" +
                                 (usageInfo.UpcomingAppointmentCount > 0 
                                     ? $"⚠️ Có {usageInfo.UpcomingAppointmentCount} lịch hẹn sắp tới!\n\n" 
                                     : "") +
                                 $"Vui lòng chọn 'Vô hiệu hóa' thay vì xóa.",
                        UsageInfo = usageInfo
                    };
                }

                // If force delete, just deactivate instead of soft delete
                if (forceDelete && usageInfo.IsInUse)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        string sql = @"
                            UPDATE Services 
                            SET is_active = 0, 
                                updated_at = @UpdatedAt 
                            WHERE id = @ServiceId";

                        connection.Execute(sql, new 
                        { 
                            ServiceId = serviceId, 
                            UpdatedAt = DateTime.Now 
                        });

                        Logger.Warning($"Service {serviceId} deactivated (force delete) by {deletedBy}");

                        return new DeleteResult
                        {
                            Success = true,
                            Message = "Dịch vụ đã được vô hiệu hóa (không xóa hoàn toàn do đang được sử dụng)."
                        };
                    }
                }

                // Safe to soft delete
                var softDeleteHelper = new SoftDeleteHelper(_connectionString);
                bool deleted = softDeleteHelper.SoftDelete("Services", serviceId, deletedBy);

                return new DeleteResult
                {
                    Success = deleted,
                    Message = deleted ? "Xóa dịch vụ thành công!" : "Không thể xóa dịch vụ."
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error safely deleting service {serviceId}", ex);
                return new DeleteResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Safely soft delete a medicine with validation
        /// </summary>
        /// <param name="medicineId">Medicine ID</param>
        /// <param name="deletedBy">User who is deleting</param>
        /// <param name="forceDelete">Force delete even if in use (mark unavailable only)</param>
        /// <returns>Result object</returns>
        public DeleteResult SafeDeleteMedicine(int medicineId, string deletedBy, bool forceDelete = false)
        {
            try
            {
                var usageInfo = GetMedicineUsageInfo(medicineId);

                if (usageInfo.IsInUse && !forceDelete)
                {
                    return new DeleteResult
                    {
                        Success = false,
                        Message = $"Không thể xóa thuốc này!\n\n" +
                                 $"Đang được sử dụng bởi:\n" +
                                 $"- {usageInfo.PrescriptionCount} đơn thuốc\n" +
                                 $"- {usageInfo.PatientCount} bệnh nhân\n" +
                                 $"- {usageInfo.InventoryRecordCount} bản ghi kho\n" +
                                 $"- Tồn kho: {usageInfo.TotalStockQuantity} đơn vị\n\n" +
                                 $"Vui lòng chọn 'Đánh dấu không khả dụng' thay vì xóa.",
                        UsageInfo = usageInfo
                    };
                }

                // If force delete, just mark as unavailable
                if (forceDelete && usageInfo.IsInUse)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        string sql = @"
                            UPDATE Medicines 
                            SET is_available = 0, 
                                updated_at = @UpdatedAt 
                            WHERE id = @MedicineId";

                        connection.Execute(sql, new 
                        { 
                            MedicineId = medicineId, 
                            UpdatedAt = DateTime.Now 
                        });

                        Logger.Warning($"Medicine {medicineId} marked unavailable (force delete) by {deletedBy}");

                        return new DeleteResult
                        {
                            Success = true,
                            Message = "Thuốc đã được đánh dấu không khả dụng (không xóa hoàn toàn do đang được sử dụng)."
                        };
                    }
                }

                // Safe to soft delete
                var softDeleteHelper = new SoftDeleteHelper(_connectionString);
                bool deleted = softDeleteHelper.SoftDelete("Medicines", medicineId, deletedBy);

                return new DeleteResult
                {
                    Success = deleted,
                    Message = deleted ? "Xóa thuốc thành công!" : "Không thể xóa thuốc."
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error safely deleting medicine {medicineId}", ex);
                return new DeleteResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        #endregion
    }

    #region Usage Info Classes

    public class ServiceUsageInfo
    {
        public int ServiceId { get; set; }
        public int AppointmentCount { get; set; }
        public int InvoiceDetailCount { get; set; }
        public int UpcomingAppointmentCount { get; set; }
        public bool IsInUse { get; set; }

        public override string ToString()
        {
            return $"Service {ServiceId}: Appointments={AppointmentCount}, Invoices={InvoiceDetailCount}, Upcoming={UpcomingAppointmentCount}";
        }
    }

    public class MedicineUsageInfo
    {
        public int MedicineId { get; set; }
        public int PrescriptionCount { get; set; }
        public int InventoryRecordCount { get; set; }
        public int TotalStockQuantity { get; set; }
        public int PatientCount { get; set; }
        public bool IsInUse { get; set; }

        public override string ToString()
        {
            return $"Medicine {MedicineId}: Prescriptions={PrescriptionCount}, Patients={PatientCount}, Stock={TotalStockQuantity}";
        }
    }

    public class PatientUsageInfo
    {
        public int PatientId { get; set; }
        public int AppointmentCount { get; set; }
        public int InvoiceCount { get; set; }
        public int MedicalRecordCount { get; set; }
        public int UpcomingAppointmentCount { get; set; }
        public bool IsInUse { get; set; }
    }

    public class DeleteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object UsageInfo { get; set; }
    }

    #endregion
}
