using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DentalClinicManagement.Models;

namespace DentalClinicManagement.Tests.Helpers
{
    /// <summary>
    /// Test data factory for creating test objects
    /// Provides consistent test data across all tests
    /// </summary>
    public static class TestDataFactory
    {
        private static Random random = new Random();

        #region Patient Test Data

        public static Patient CreateTestPatient(string namePrefix = "Test")
        {
            return new Patient
            {
                full_name = $"{namePrefix} Patient {random.Next(1000, 9999)}",
                email = $"test{random.Next(1000, 9999)}@test.com",
                phone = $"0{random.Next(100000000, 999999999)}",
                date_of_birth = DateTime.Now.AddYears(-random.Next(18, 80)),
                gender = random.Next(2) == 0 ? "Male" : "Female",
                address = $"{random.Next(1, 999)} Test Street, Test City",
                created_at = DateTime.Now
            };
        }

        public static List<Patient> CreateTestPatients(int count)
        {
            var patients = new List<Patient>();
            for (int i = 0; i < count; i++)
            {
                patients.Add(CreateTestPatient($"Test{i}"));
            }
            return patients;
        }

        public static Patient CreatePatientWithId(int id)
        {
            var patient = CreateTestPatient();
            patient.id = id;
            return patient;
        }

        #endregion

        #region Service Test Data

        public static Service CreateTestService(string namePrefix = "Test")
        {
            return new Service
            {
                service_name = $"{namePrefix} Service {random.Next(1000, 9999)}",
                description = "Test service description",
                price = random.Next(100000, 1000000),
                duration_minutes = random.Next(15, 120),
                is_active = true
            };
        }

        public static List<Service> CreateTestServices(int count)
        {
            var services = new List<Service>();
            for (int i = 0; i < count; i++)
            {
                services.Add(CreateTestService($"Test{i}"));
            }
            return services;
        }

        #endregion

        #region Staff Test Data

        public static Staff CreateTestStaff(string position = "Doctor")
        {
            return new Staff
            {
                full_name = $"Test {position} {random.Next(1000, 9999)}",
                email = $"staff{random.Next(1000, 9999)}@test.com",
                phone = $"0{random.Next(100000000, 999999999)}",
                position = position,
                specialization = position == "Doctor" ? "General Dentistry" : null,
                is_active = true,
                hire_date = DateTime.Now.AddYears(-random.Next(1, 10))
            };
        }

        public static List<Staff> CreateTestStaffMembers(int count)
        {
            var staff = new List<Staff>();
            string[] positions = { "Doctor", "Nurse", "Receptionist", "Assistant" };
            
            for (int i = 0; i < count; i++)
            {
                staff.Add(CreateTestStaff(positions[i % positions.Length]));
            }
            return staff;
        }

        #endregion

        #region Appointment Test Data

        public static Appointment CreateTestAppointment(int patientId, int doctorId, int serviceId)
        {
            return new Appointment
            {
                appointment_code = $"APT{DateTime.Now:yyyyMMddHHmmss}{random.Next(1000, 9999)}",
                patient_id = patientId,
                doctor_id = doctorId,
                service_id = serviceId,
                appointment_date = DateTime.Now.AddDays(random.Next(1, 30)),
                time_slot = $"{random.Next(8, 17):D2}:00",
                status = "Scheduled",
                notes = "Test appointment notes"
            };
        }

        public static List<Appointment> CreateTestAppointments(int count, int patientId, int doctorId, int serviceId)
        {
            var appointments = new List<Appointment>();
            for (int i = 0; i < count; i++)
            {
                appointments.Add(CreateTestAppointment(patientId, doctorId, serviceId));
            }
            return appointments;
        }

        #endregion

        #region Invoice Test Data

        public static Invoice CreateTestInvoice(int patientId)
        {
            decimal totalAmount = random.Next(100000, 5000000);
            decimal discount = totalAmount * (decimal)(random.Next(0, 20) / 100.0);

            return new Invoice
            {
                invoice_code = $"INV{DateTime.Now:yyyyMMddHHmmss}{random.Next(1000, 9999)}",
                patient_id = patientId,
                invoice_date = DateTime.Now,
                total_amount = totalAmount,
                discount = discount,
                final_amount = totalAmount - discount,
                payment_status = "Pending",
                created_by = "TestUser"
            };
        }

        #endregion

        #region User Test Data

        public static User CreateTestUser(string role = "Doctor")
        {
            return new User
            {
                username = $"testuser{random.Next(1000, 9999)}",
                password_hash = "test_hash", // Will be hashed by PasswordHasher
                full_name = $"Test User {random.Next(1000, 9999)}",
                email = $"user{random.Next(1000, 9999)}@test.com",
                role = role,
                is_active = true,
                created_at = DateTime.Now
            };
        }

        #endregion

        #region MedicalRecord Test Data

        public static MedicalRecord CreateTestMedicalRecord(int patientId, int doctorId)
        {
            return new MedicalRecord
            {
                patient_id = patientId,
                doctor_id = doctorId,
                visit_date = DateTime.Now,
                diagnosis = "Test diagnosis",
                symptoms = "Test symptoms",
                treatment_plan = "Test treatment plan",
                notes = "Test medical notes"
            };
        }

        #endregion

        #region Medicine Test Data

        public static Medicine CreateTestMedicine()
        {
            return new Medicine
            {
                medicine_name = $"Test Medicine {random.Next(1000, 9999)}",
                description = "Test medicine description",
                unit_price = random.Next(10000, 500000),
                stock_quantity = random.Next(10, 1000),
                unit = "Viên",
                expiry_date = DateTime.Now.AddYears(2),
                is_available = true
            };
        }

        public static List<Medicine> CreateTestMedicines(int count)
        {
            var medicines = new List<Medicine>();
            for (int i = 0; i < count; i++)
            {
                medicines.Add(CreateTestMedicine());
            }
            return medicines;
        }

        #endregion

        #region DataTable Test Data

        public static DataTable CreateTestPatientDataTable(int rowCount)
        {
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("full_name", typeof(string));
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("phone", typeof(string));
            dt.Columns.Add("date_of_birth", typeof(DateTime));
            dt.Columns.Add("gender", typeof(string));

            for (int i = 1; i <= rowCount; i++)
            {
                dt.Rows.Add(
                    i,
                    $"Test Patient {i}",
                    $"test{i}@test.com",
                    $"012345678{i % 10}",
                    DateTime.Now.AddYears(-30),
                    i % 2 == 0 ? "Male" : "Female"
                );
            }

            return dt;
        }

        public static DataTable CreateTestServiceDataTable(int rowCount)
        {
            var dt = new DataTable();
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("service_name", typeof(string));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("price", typeof(decimal));
            dt.Columns.Add("duration_minutes", typeof(int));

            for (int i = 1; i <= rowCount; i++)
            {
                dt.Rows.Add(
                    i,
                    $"Test Service {i}",
                    "Test description",
                    random.Next(100000, 1000000),
                    random.Next(30, 120)
                );
            }

            return dt;
        }

        #endregion

        #region Validation Test Data

        public static class ValidationTestData
        {
            public static readonly string[] ValidEmails = new[]
            {
                "test@example.com",
                "user.name@example.co.uk",
                "user+tag@example.com",
                "test123@test-domain.com"
            };

            public static readonly string[] InvalidEmails = new[]
            {
                "invalid",
                "@example.com",
                "test@",
                "test @example.com",
                ""
            };

            public static readonly string[] ValidPhones = new[]
            {
                "0123456789",
                "0987654321",
                "+84123456789",
                "84123456789"
            };

            public static readonly string[] InvalidPhones = new[]
            {
                "123",
                "abcdefghij",
                "012-345-6789",
                ""
            };

            public static readonly string[] ValidPasswords = new[]
            {
                "Test@123456",
                "Strong#Pass1",
                "MyP@ssw0rd",
                "Secure$123"
            };

            public static readonly string[] WeakPasswords = new[]
            {
                "test123",      // No uppercase, no special char
                "TEST@123",     // No lowercase
                "Test@abc",     // No digit
                "Test123",      // No special char
                "Test@12"       // Too short
            };
        }

        #endregion

        #region Random Helpers

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static DateTime RandomDate(int minYearsAgo, int maxYearsAgo)
        {
            var start = DateTime.Now.AddYears(-maxYearsAgo);
            var end = DateTime.Now.AddYears(-minYearsAgo);
            int range = (end - start).Days;
            return start.AddDays(random.Next(range));
        }

        public static decimal RandomDecimal(decimal min, decimal max)
        {
            return (decimal)random.NextDouble() * (max - min) + min;
        }

        #endregion
    }
}
