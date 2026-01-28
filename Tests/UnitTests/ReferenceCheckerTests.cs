using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Models;

namespace DentalClinicManagement.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for ReferenceChecker
    /// Issue #9: Prevent Deletion of In-Use Records
    /// </summary>
    [TestClass]
    public class ReferenceCheckerTests
    {
        private ReferenceChecker _referenceChecker;
        private string _testConnectionString;

        [TestInitialize]
        public void Setup()
        {
            _testConnectionString = ConfigManager.GetConnectionString("TestDatabase");
            _referenceChecker = new ReferenceChecker(_testConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test data
            CleanupTestData();
        }

        #region Service Reference Tests

        [TestMethod]
        public void IsServiceInUse_ServiceWithAppointments_ReturnsTrue()
        {
            // Arrange
            int serviceId = CreateTestService("Test Service");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            CreateTestAppointment(patientId, doctorId, serviceId);

            // Act
            bool isInUse = _referenceChecker.IsServiceInUse(serviceId);

            // Assert
            Assert.IsTrue(isInUse, "Service with appointments should be in use");
        }

        [TestMethod]
        public void IsServiceInUse_ServiceWithoutReferences_ReturnsFalse()
        {
            // Arrange
            int serviceId = CreateTestService("Unused Service");

            // Act
            bool isInUse = _referenceChecker.IsServiceInUse(serviceId);

            // Assert
            Assert.IsFalse(isInUse, "Service without references should not be in use");
        }

        [TestMethod]
        public void GetServiceUsageInfo_ServiceWithMultipleReferences_ReturnsCorrectCounts()
        {
            // Arrange
            int serviceId = CreateTestService("Popular Service");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            
            // Create 3 appointments
            for (int i = 0; i < 3; i++)
            {
                CreateTestAppointment(patientId, doctorId, serviceId);
            }

            // Act
            var usageInfo = _referenceChecker.GetServiceUsageInfo(serviceId);

            // Assert
            Assert.AreEqual(3, usageInfo.AppointmentCount, "Should have 3 appointments");
            Assert.IsTrue(usageInfo.IsInUse, "Service should be marked as in use");
        }

        [TestMethod]
        public void SafeDeleteService_ServiceInUse_ReturnsFalse()
        {
            // Arrange
            int serviceId = CreateTestService("In-Use Service");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            CreateTestAppointment(patientId, doctorId, serviceId);

            // Act
            var result = _referenceChecker.SafeDeleteService(serviceId, "TestUser", forceDelete: false);

            // Assert
            Assert.IsFalse(result.Success, "Should not allow deletion of in-use service");
            Assert.IsTrue(result.Message.Contains("Không thể xóa"), "Should contain Vietnamese error message");
            Assert.IsTrue(result.Message.Contains("lịch hẹn"), "Should mention appointments");
        }

        [TestMethod]
        public void SafeDeleteService_ServiceNotInUse_ReturnsTrue()
        {
            // Arrange
            int serviceId = CreateTestService("Unused Service");

            // Act
            var result = _referenceChecker.SafeDeleteService(serviceId, "TestUser", forceDelete: false);

            // Assert
            Assert.IsTrue(result.Success, "Should allow deletion of unused service");
        }

        [TestMethod]
        public void SafeDeleteService_ForceDelete_DeactivatesService()
        {
            // Arrange
            int serviceId = CreateTestService("Force Delete Service");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            CreateTestAppointment(patientId, doctorId, serviceId);

            // Act
            var result = _referenceChecker.SafeDeleteService(serviceId, "TestUser", forceDelete: true);

            // Assert
            Assert.IsTrue(result.Success, "Force delete should succeed");
            Assert.IsTrue(result.Message.Contains("vô hiệu hóa"), "Should mention deactivation");
            
            // Verify service is deactivated, not deleted
            var service = GetServiceById(serviceId);
            Assert.IsNotNull(service, "Service should still exist");
            Assert.IsFalse(Convert.ToBoolean(service["is_active"]), "Service should be inactive");
            Assert.IsFalse(Convert.ToBoolean(service["is_deleted"]), "Service should NOT be soft-deleted");
        }

        #endregion

        #region Medicine Reference Tests

        [TestMethod]
        public void IsMedicineInUse_MedicineWithPrescriptions_ReturnsTrue()
        {
            // Arrange
            int medicineId = CreateTestMedicine("Test Medicine");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int recordId = CreateTestMedicalRecord(patientId, doctorId);
            CreateTestPrescription(recordId, medicineId);

            // Act
            bool isInUse = _referenceChecker.IsMedicineInUse(medicineId);

            // Assert
            Assert.IsTrue(isInUse, "Medicine with prescriptions should be in use");
        }

        [TestMethod]
        public void IsMedicineInUse_MedicineWithoutReferences_ReturnsFalse()
        {
            // Arrange
            int medicineId = CreateTestMedicine("Unused Medicine");

            // Act
            bool isInUse = _referenceChecker.IsMedicineInUse(medicineId);

            // Assert
            Assert.IsFalse(isInUse, "Medicine without references should not be in use");
        }

        [TestMethod]
        public void GetMedicineUsageInfo_MedicineWithMultipleReferences_ReturnsCorrectCounts()
        {
            // Arrange
            int medicineId = CreateTestMedicine("Popular Medicine");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int recordId = CreateTestMedicalRecord(patientId, doctorId);
            
            // Create 2 prescriptions
            for (int i = 0; i < 2; i++)
            {
                CreateTestPrescription(recordId, medicineId);
            }

            // Act
            var usageInfo = _referenceChecker.GetMedicineUsageInfo(medicineId);

            // Assert
            Assert.AreEqual(2, usageInfo.PrescriptionCount, "Should have 2 prescriptions");
            Assert.IsTrue(usageInfo.IsInUse, "Medicine should be marked as in use");
        }

        [TestMethod]
        public void SafeDeleteMedicine_MedicineInUse_ReturnsFalse()
        {
            // Arrange
            int medicineId = CreateTestMedicine("In-Use Medicine");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int recordId = CreateTestMedicalRecord(patientId, doctorId);
            CreateTestPrescription(recordId, medicineId);

            // Act
            var result = _referenceChecker.SafeDeleteMedicine(medicineId, "TestUser", forceDelete: false);

            // Assert
            Assert.IsFalse(result.Success, "Should not allow deletion of in-use medicine");
            Assert.IsTrue(result.Message.Contains("Không thể xóa"), "Should contain Vietnamese error message");
            Assert.IsTrue(result.Message.Contains("đơn thuốc"), "Should mention prescriptions");
        }

        [TestMethod]
        public void SafeDeleteMedicine_MedicineNotInUse_ReturnsTrue()
        {
            // Arrange
            int medicineId = CreateTestMedicine("Unused Medicine");

            // Act
            var result = _referenceChecker.SafeDeleteMedicine(medicineId, "TestUser", forceDelete: false);

            // Assert
            Assert.IsTrue(result.Success, "Should allow deletion of unused medicine");
        }

        [TestMethod]
        public void SafeDeleteMedicine_ForceDelete_MarksUnavailable()
        {
            // Arrange
            int medicineId = CreateTestMedicine("Force Delete Medicine");
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int recordId = CreateTestMedicalRecord(patientId, doctorId);
            CreateTestPrescription(recordId, medicineId);

            // Act
            var result = _referenceChecker.SafeDeleteMedicine(medicineId, "TestUser", forceDelete: true);

            // Assert
            Assert.IsTrue(result.Success, "Force delete should succeed");
            Assert.IsTrue(result.Message.Contains("không khả dụng"), "Should mention unavailable");
            
            // Verify medicine is marked unavailable, not deleted
            var medicine = GetMedicineById(medicineId);
            Assert.IsNotNull(medicine, "Medicine should still exist");
            Assert.IsFalse(Convert.ToBoolean(medicine["is_available"]), "Medicine should be unavailable");
            Assert.IsFalse(Convert.ToBoolean(medicine["is_deleted"]), "Medicine should NOT be soft-deleted");
        }

        #endregion

        #region Patient Reference Tests

        [TestMethod]
        public void IsPatientInUse_PatientWithAppointments_ReturnsTrue()
        {
            // Arrange
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int serviceId = CreateTestService("Service");
            CreateTestAppointment(patientId, doctorId, serviceId);

            // Act
            bool isInUse = _referenceChecker.IsPatientInUse(patientId);

            // Assert
            Assert.IsTrue(isInUse, "Patient with appointments should be in use");
        }

        [TestMethod]
        public void IsPatientInUse_NewPatient_ReturnsFalse()
        {
            // Arrange
            int patientId = CreateTestPatient();

            // Act
            bool isInUse = _referenceChecker.IsPatientInUse(patientId);

            // Assert
            Assert.IsFalse(isInUse, "New patient without records should not be in use");
        }

        [TestMethod]
        public void GetPatientUsageInfo_PatientWithMultipleRecords_ReturnsCorrectCounts()
        {
            // Arrange
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int serviceId = CreateTestService("Service");
            
            // Create 2 appointments
            for (int i = 0; i < 2; i++)
            {
                CreateTestAppointment(patientId, doctorId, serviceId);
            }
            
            // Create 1 medical record
            CreateTestMedicalRecord(patientId, doctorId);

            // Act
            var usageInfo = _referenceChecker.GetPatientUsageInfo(patientId);

            // Assert
            Assert.AreEqual(2, usageInfo.AppointmentCount, "Should have 2 appointments");
            Assert.AreEqual(1, usageInfo.MedicalRecordCount, "Should have 1 medical record");
            Assert.IsTrue(usageInfo.IsInUse, "Patient should be marked as in use");
        }

        #endregion

        #region Staff Reference Tests

        [TestMethod]
        public void IsStaffInUse_StaffWithAppointments_ReturnsTrue()
        {
            // Arrange
            int patientId = CreateTestPatient();
            int doctorId = CreateTestStaff("Doctor");
            int serviceId = CreateTestService("Service");
            CreateTestAppointment(patientId, doctorId, serviceId);

            // Act
            bool isInUse = _referenceChecker.IsStaffInUse(doctorId);

            // Assert
            Assert.IsTrue(isInUse, "Staff with appointments should be in use");
        }

        [TestMethod]
        public void IsStaffInUse_NewStaff_ReturnsFalse()
        {
            // Arrange
            int staffId = CreateTestStaff("New Doctor");

            // Act
            bool isInUse = _referenceChecker.IsStaffInUse(staffId);

            // Assert
            Assert.IsFalse(isInUse, "New staff without records should not be in use");
        }

        #endregion

        #region Helper Methods

        private int CreateTestService(string name)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Services (service_name, description, price, duration_minutes, is_active, is_deleted)
                    VALUES (@Name, 'Test Description', 100000, 30, 1, 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private int CreateTestMedicine(string name)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Medicines (medicine_name, description, unit_price, stock_quantity, unit, expiry_date, is_available, is_deleted)
                    VALUES (@Name, 'Test Medicine', 50000, 100, 'Viên', DATEADD(YEAR, 1, GETDATE()), 1, 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private int CreateTestPatient()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Patients (full_name, email, phone, date_of_birth, gender, address, is_deleted)
                    VALUES (@Name, @Email, '0123456789', '1990-01-01', 'Male', 'Test Address', 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    string random = Guid.NewGuid().ToString().Substring(0, 8);
                    command.Parameters.AddWithValue("@Name", $"Test Patient {random}");
                    command.Parameters.AddWithValue("@Email", $"test{random}@test.com");
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private int CreateTestStaff(string position)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Staff (full_name, email, phone, position, is_deleted)
                    VALUES (@Name, @Email, '0987654321', @Position, 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    string random = Guid.NewGuid().ToString().Substring(0, 8);
                    command.Parameters.AddWithValue("@Name", $"Test Staff {random}");
                    command.Parameters.AddWithValue("@Email", $"staff{random}@test.com");
                    command.Parameters.AddWithValue("@Position", position);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private int CreateTestAppointment(int patientId, int doctorId, int serviceId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Appointments (appointment_code, patient_id, doctor_id, service_id, appointment_date, time_slot, status, is_deleted)
                    VALUES (@Code, @PatientId, @DoctorId, @ServiceId, @Date, '09:00', 'Pending', 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Code", $"APT{DateTime.Now.Ticks}");
                    command.Parameters.AddWithValue("@PatientId", patientId);
                    command.Parameters.AddWithValue("@DoctorId", doctorId);
                    command.Parameters.AddWithValue("@ServiceId", serviceId);
                    command.Parameters.AddWithValue("@Date", DateTime.Now.AddDays(1));
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private int CreateTestMedicalRecord(int patientId, int doctorId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO MedicalRecords (patient_id, doctor_id, visit_date, diagnosis, treatment, notes, is_deleted)
                    VALUES (@PatientId, @DoctorId, @VisitDate, 'Test Diagnosis', 'Test Treatment', 'Test Notes', 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);
                    command.Parameters.AddWithValue("@DoctorId", doctorId);
                    command.Parameters.AddWithValue("@VisitDate", DateTime.Now);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private int CreateTestPrescription(int recordId, int medicineId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Prescriptions (record_id, medicine_id, dosage, frequency, duration, is_deleted)
                    VALUES (@RecordId, @MedicineId, '1 viên', '3 lần/ngày', '7 ngày', 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@RecordId", recordId);
                    command.Parameters.AddWithValue("@MedicineId", medicineId);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        private System.Data.DataRow GetServiceById(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Services WHERE id = @Id";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new System.Data.DataTable();
                        adapter.Fill(dt);
                        
                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        
                        return null;
                    }
                }
            }
        }

        private System.Data.DataRow GetMedicineById(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Medicines WHERE id = @Id";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new System.Data.DataTable();
                        adapter.Fill(dt);
                        
                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        
                        return null;
                    }
                }
            }
        }

        private void CleanupTestData()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string[] cleanupSql = new string[]
                {
                    "DELETE FROM Prescriptions WHERE is_deleted = 0",
                    "DELETE FROM MedicalRecords WHERE is_deleted = 0",
                    "DELETE FROM Appointments WHERE is_deleted = 0",
                    "DELETE FROM Services WHERE service_name LIKE 'Test%' OR service_name LIKE '%Test%'",
                    "DELETE FROM Medicines WHERE medicine_name LIKE 'Test%' OR medicine_name LIKE '%Test%'",
                    "DELETE FROM Patients WHERE email LIKE '%@test.com'",
                    "DELETE FROM Staff WHERE email LIKE '%@test.com'"
                };

                foreach (var sql in cleanupSql)
                {
                    using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion
    }
}
