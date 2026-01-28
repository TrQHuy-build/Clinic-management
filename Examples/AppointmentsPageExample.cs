using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DentalClinicManagement.Base;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Models;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Examples
{
    /// <summary>
    /// Example: Appointment Management Page using BaseDataPage
    /// Shows how to handle status management and date filtering
    /// </summary>
    public partial class AppointmentsPageExample : BaseDataPage<Appointment>
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private QueryOptimizer queryOptimizer = new QueryOptimizer();

        private ComboBox cmbStatusFilter;
        private DateTimePicker dtpFromDate, dtpToDate;
        private Button btnFilter;

        public AppointmentsPageExample()
        {
            // Base class will call InitializeComponent() and setup everything
            AddCustomFilters();
        }

        #region Abstract Method Implementations (Required)

        protected override List<DataGridViewColumn> GetColumns()
        {
            // Use predefined template - 1 line vs 60+ lines!
            // Or customize with factory methods:
            return new List<DataGridViewColumn>
            {
                DataGridViewTemplate.CreateIdColumn(),
                DataGridViewTemplate.CreateTextColumn("appointment_code", "Mã lịch hẹn", 120),
                DataGridViewTemplate.CreateTextColumn("patient_name", "Bệnh nhân", 180),
                DataGridViewTemplate.CreateTextColumn("doctor_name", "Bác sĩ", 160),
                DataGridViewTemplate.CreateDateTimeColumn("appointment_date", "Ngày hẹn", 140),
                DataGridViewTemplate.CreateTextColumn("time_slot", "Giờ hẹn", 100),
                DataGridViewTemplate.CreateTextColumn("service_name", "Dịch vụ", 180),
                DataGridViewTemplate.CreateTextColumn("status", "Trạng thái", 120),
                DataGridViewTemplate.CreateTextColumn("notes", "Ghi chú", 200)
            };
        }

        protected override async Task<List<Appointment>> LoadDataAsync()
        {
            // Use QueryOptimizer to load appointments with patient/doctor/service info
            // Prevents N+1 queries!
            var appointmentsWithDetails = await queryOptimizer.LoadAppointmentsWithDetailsAsync();

            return appointmentsWithDetails.Select(apt => new Appointment
            {
                id = apt.AppointmentId,
                appointment_code = apt.AppointmentCode,
                patient_id = apt.PatientId,
                doctor_id = apt.DoctorId,
                service_id = apt.ServiceId,
                appointment_date = apt.AppointmentDate,
                time_slot = apt.TimeSlot,
                status = apt.Status,
                notes = apt.Notes,
                // From JOIN
                patient_name = apt.PatientName,
                doctor_name = apt.DoctorName,
                service_name = apt.ServiceName
            }).ToList();
        }

        protected override object[] EntityToRow(Appointment appointment)
        {
            return new object[]
            {
                appointment.id,
                appointment.appointment_code,
                appointment.patient_name ?? "N/A",
                appointment.doctor_name ?? "N/A",
                appointment.appointment_date,
                appointment.time_slot,
                appointment.service_name ?? "N/A",
                appointment.status,
                appointment.notes
            };
        }

        protected override Appointment RowToEntity(DataGridViewRow row)
        {
            return new Appointment
            {
                id = (int)row.Cells["id"].Value,
                appointment_code = row.Cells["appointment_code"].Value?.ToString(),
                patient_name = row.Cells["patient_name"].Value?.ToString(),
                doctor_name = row.Cells["doctor_name"].Value?.ToString(),
                appointment_date = (DateTime)row.Cells["appointment_date"].Value,
                time_slot = row.Cells["time_slot"].Value?.ToString(),
                service_name = row.Cells["service_name"].Value?.ToString(),
                status = row.Cells["status"].Value?.ToString(),
                notes = row.Cells["notes"].Value?.ToString()
            };
        }

        protected override string GetCacheKey() => "appointments_all";

        #endregion

        #region Virtual Method Overrides (Optional)

        protected override bool FilterEntity(Appointment appointment, string searchText)
        {
            searchText = searchText.ToLower();
            return appointment.appointment_code?.ToLower().Contains(searchText) == true ||
                   appointment.patient_name?.ToLower().Contains(searchText) == true ||
                   appointment.doctor_name?.ToLower().Contains(searchText) == true ||
                   appointment.service_name?.ToLower().Contains(searchText) == true ||
                   appointment.status?.ToLower().Contains(searchText) == true;
        }

        protected override void FormatColumns()
        {
            base.FormatColumns();

            // Custom formatting for status column with colors
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["status"].Value != null)
                {
                    string status = row.Cells["status"].Value.ToString();

                    switch (status)
                    {
                        case "Scheduled":
                            row.Cells["status"].Style.BackColor = Color.LightBlue;
                            row.Cells["status"].Style.ForeColor = Color.DarkBlue;
                            break;
                        case "Confirmed":
                            row.Cells["status"].Style.BackColor = Color.LightGreen;
                            row.Cells["status"].Style.ForeColor = Color.DarkGreen;
                            break;
                        case "Completed":
                            row.Cells["status"].Style.BackColor = Color.LightGray;
                            row.Cells["status"].Style.ForeColor = Color.Black;
                            break;
                        case "Cancelled":
                            row.Cells["status"].Style.BackColor = Color.LightCoral;
                            row.Cells["status"].Style.ForeColor = Color.DarkRed;
                            break;
                        case "No-show":
                            row.Cells["status"].Style.BackColor = Color.Orange;
                            row.Cells["status"].Style.ForeColor = Color.DarkOrange;
                            break;
                    }
                }

                // Highlight past appointments
                if (row.Cells["appointment_date"].Value != null)
                {
                    DateTime aptDate = (DateTime)row.Cells["appointment_date"].Value;
                    if (aptDate.Date < DateTime.Today)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
                    }
                }
            }
        }

        protected override List<ToolStripMenuItem> GetContextMenuItems()
        {
            var items = new List<ToolStripMenuItem>();

            // Status actions
            items.Add(new ToolStripMenuItem("Confirm", null, async (s, e) => await UpdateStatusAsync("Confirmed")));
            items.Add(new ToolStripMenuItem("Complete", null, async (s, e) => await UpdateStatusAsync("Completed")));
            items.Add(new ToolStripMenuItem("Cancel", null, async (s, e) => await UpdateStatusAsync("Cancelled")));
            items.Add(new ToolStripMenuItem("Mark No-show", null, async (s, e) => await UpdateStatusAsync("No-show")));
            items.Add(new ToolStripSeparator());

            // Standard actions
            items.AddRange(base.GetContextMenuItems()); // View, Edit, Delete

            items.Add(new ToolStripSeparator());
            items.Add(new ToolStripMenuItem("Send Reminder", null, async (s, e) => await SendReminderAsync()));
            items.Add(new ToolStripMenuItem("Reschedule", null, async (s, e) => await RescheduleAsync()));

            return items;
        }

        protected override async Task AddAsync()
        {
            var form = new AppointmentAddEditForm();
            if (await CodeTemplates.OpenDialogAndRefresh(form, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Appointment created successfully!");
            }
        }

        protected override async Task ViewAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Please select an appointment to view.");
                return;
            }

            var form = new AppointmentViewForm(selectedItem);
            CodeTemplates.OpenDialog(form);
        }

        protected override async Task EditAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Please select an appointment to edit.");
                return;
            }

            // Check if appointment can be edited
            if (selectedItem.status == "Completed" || selectedItem.status == "Cancelled")
            {
                MessageBoxHelper.ShowWarning($"Cannot edit {selectedItem.status.ToLower()} appointment.");
                return;
            }

            var form = new AppointmentAddEditForm(selectedItem);
            if (await CodeTemplates.OpenDialogAndRefresh(form, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Appointment updated successfully!");
            }
        }

        protected override async Task DeleteAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Please select an appointment to delete.");
                return;
            }

            // Check if appointment can be deleted
            if (selectedItem.status == "Completed")
            {
                MessageBoxHelper.ShowWarning("Cannot delete completed appointment.");
                return;
            }

            if (!CodeTemplates.ConfirmDelete($"appointment '{selectedItem.appointment_code}'"))
                return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    await dbHelper.ExecuteNonQueryAsync(
                        "DELETE FROM Appointments WHERE id = @id",
                        new { id = selectedItem.id }
                    );

                    await RefreshAsync();
                    MessageBoxHelper.ShowSuccess("Appointment deleted successfully!");
                    cacheManager.InvalidateRelated("appointment");
                },
                "Error deleting appointment"
            );
        }

        protected override bool EnablePagination => true;
        protected override int PageSize => 100;

        #endregion

        #region Custom Filters

        private void AddCustomFilters()
        {
            // Add custom filter controls to top panel
            var filterPanel = new Panel
            {
                Height = 35,
                Dock = DockStyle.Top,
                Padding = new Padding(5)
            };

            // Status filter
            var lblStatus = new Label { Text = "Status:", AutoSize = true, Top = 8, Left = 5 };
            cmbStatusFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = 60,
                Top = 5,
                Width = 120
            };
            cmbStatusFilter.Items.AddRange(new object[] { "All", "Scheduled", "Confirmed", "Completed", "Cancelled", "No-show" });
            cmbStatusFilter.SelectedIndex = 0;

            // Date range
            var lblFrom = new Label { Text = "From:", AutoSize = true, Top = 8, Left = 190 };
            dtpFromDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Left = 235,
                Top = 5,
                Width = 120,
                Value = DateTime.Today.AddMonths(-1)
            };

            var lblTo = new Label { Text = "To:", AutoSize = true, Top = 8, Left = 365 };
            dtpToDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Left = 395,
                Top = 5,
                Width = 120,
                Value = DateTime.Today.AddMonths(1)
            };

            btnFilter = new Button
            {
                Text = "Apply Filter",
                Left = 525,
                Top = 4,
                Width = 100,
                Height = 25
            };
            btnFilter.Click += async (s, e) => await ApplyCustomFiltersAsync();

            filterPanel.Controls.AddRange(new Control[] { lblStatus, cmbStatusFilter, lblFrom, dtpFromDate, lblTo, dtpToDate, btnFilter });

            // Insert at top of form
            this.Controls.Add(filterPanel);
            filterPanel.BringToFront();
        }

        private async Task ApplyCustomFiltersAsync()
        {
            await CodeTemplates.ExecuteWithLoadingAsync(
                this.FindForm(),
                async () =>
                {
                    // Load data with filters
                    var allData = await LoadDataAsync();

                    // Apply status filter
                    if (cmbStatusFilter.SelectedItem.ToString() != "All")
                    {
                        allData = allData.Where(a => a.status == cmbStatusFilter.SelectedItem.ToString()).ToList();
                    }

                    // Apply date range filter
                    allData = allData.Where(a =>
                        a.appointment_date.Date >= dtpFromDate.Value.Date &&
                        a.appointment_date.Date <= dtpToDate.Value.Date
                    ).ToList();

                    // Update grid
                    currentData = allData;
                    BindDataToGrid();
                },
                "Applying filters...",
                "Error applying filters"
            );
        }

        #endregion

        #region Custom Methods

        private async Task UpdateStatusAsync(string newStatus)
        {
            if (selectedItem == null) return;

            if (selectedItem.status == newStatus)
            {
                MessageBoxHelper.ShowInfo($"Appointment is already {newStatus.ToLower()}.");
                return;
            }

            if (!CodeTemplates.ConfirmAction($"Change appointment status to {newStatus}?", "Confirm Status Change"))
                return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    await dbHelper.ExecuteNonQueryAsync(
                        "UPDATE Appointments SET status = @status WHERE id = @id",
                        new { status = newStatus, id = selectedItem.id }
                    );

                    await RefreshAsync();
                    MessageBoxHelper.ShowSuccess($"Status changed to {newStatus}");
                    cacheManager.InvalidateRelated("appointment");
                },
                "Error updating status"
            );
        }

        private async Task SendReminderAsync()
        {
            if (selectedItem == null) return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    // Get patient info
                    var patient = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                        "SELECT * FROM Patients WHERE id = @id",
                        new { id = selectedItem.patient_id }
                    );

                    if (string.IsNullOrEmpty(patient?.phone) && string.IsNullOrEmpty(patient?.email))
                    {
                        MessageBoxHelper.ShowWarning("Patient has no contact information.");
                        return;
                    }

                    // Simulate sending reminder (SMS/Email)
                    await Task.Delay(1000);

                    string message = $"Reminder sent to {patient.full_name}";
                    if (!string.IsNullOrEmpty(patient.phone))
                        message += $" (SMS: {patient.phone})";
                    if (!string.IsNullOrEmpty(patient.email))
                        message += $" (Email: {patient.email})";

                    MessageBoxHelper.ShowSuccess(message);
                },
                "Error sending reminder"
            );
        }

        private async Task RescheduleAsync()
        {
            if (selectedItem == null) return;

            if (selectedItem.status == "Completed" || selectedItem.status == "Cancelled")
            {
                MessageBoxHelper.ShowWarning($"Cannot reschedule {selectedItem.status.ToLower()} appointment.");
                return;
            }

            var form = new AppointmentRescheduleForm(selectedItem);
            if (await CodeTemplates.OpenDialogAndRefresh(form, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Appointment rescheduled successfully!");
            }
        }

        #endregion
    }

    /// <summary>
    /// Add/Edit form for appointments
    /// </summary>
    public partial class AppointmentAddEditForm : Form
    {
        private ValidationHelper validationHelper;
        private DatabaseHelper dbHelper;
        private Appointment appointment;
        private bool isEditMode;

        private ComboBox cmbPatient, cmbDoctor, cmbService, cmbTimeSlot, cmbStatus;
        private DateTimePicker dtpAppointmentDate;
        private TextBox txtNotes;
        private Button btnSave, btnCancel;

        public AppointmentAddEditForm(Appointment appointment = null)
        {
            this.appointment = appointment ?? new Appointment();
            this.isEditMode = appointment != null;

            InitializeForm();
        }

        private void InitializeForm()
        {
            CodeTemplates.InitializeForm(
                this,
                isEditMode ? "Edit Appointment" : "New Appointment",
                700,
                500,
                FormStartPosition.CenterScreen
            );

            validationHelper = new ValidationHelper(this);
            dbHelper = new DatabaseHelper();

            SetupControls();
            SetupValidation();
            LoadInitialData();
        }

        private void SetupControls()
        {
            // Create controls
            cmbPatient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDoctor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbService = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            dtpAppointmentDate = new DateTimePicker { Format = DateTimePickerFormat.Short };
            cmbTimeSlot = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            txtNotes = new TextBox { Multiline = true, Height = 80 };
            btnSave = new Button { Text = "Save" };
            btnCancel = new Button { Text = "Cancel" };

            // Event handlers
            btnSave.Click += btnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            dtpAppointmentDate.ValueChanged += async (s, e) => await LoadAvailableTimeSlotsAsync();
            cmbDoctor.SelectedIndexChanged += async (s, e) => await LoadAvailableTimeSlotsAsync();

            // Load existing data
            if (isEditMode)
            {
                dtpAppointmentDate.Value = appointment.appointment_date;
                cmbTimeSlot.SelectedItem = appointment.time_slot;
                cmbStatus.SelectedItem = appointment.status;
                txtNotes.Text = appointment.notes;
            }
        }

        private void SetupValidation()
        {
            CodeTemplates.SetupRealtimeValidation(
                validationHelper,
                (cmbPatient, cb => validationHelper.ValidateComboBox(cb, "Patient")),
                (cmbDoctor, cb => validationHelper.ValidateComboBox(cb, "Doctor")),
                (cmbService, cb => validationHelper.ValidateComboBox(cb, "Service")),
                (cmbTimeSlot, cb => validationHelper.ValidateComboBox(cb, "Time slot"))
            );
        }

        private async void LoadInitialData()
        {
            await CodeTemplates.ExecuteWithLoadingAsync(
                this,
                async () =>
                {
                    // Load patients
                    var patients = await dbHelper.ExecuteQueryAsync<Patient>(
                        "SELECT id, full_name FROM Patients ORDER BY full_name"
                    );
                    CodeTemplates.BindComboBox(cmbPatient, patients.ToList(), "full_name", "id");

                    // Load doctors
                    var doctors = await dbHelper.ExecuteQueryAsync<Staff>(
                        "SELECT id, full_name FROM Staff WHERE position = 'Doctor' ORDER BY full_name"
                    );
                    CodeTemplates.BindComboBox(cmbDoctor, doctors.ToList(), "full_name", "id");

                    // Load services
                    var services = await dbHelper.ExecuteQueryAsync<Service>(
                        "SELECT id, service_name FROM Services ORDER BY service_name"
                    );
                    CodeTemplates.BindComboBox(cmbService, services.ToList(), "service_name", "id");

                    // Load statuses
                    cmbStatus.Items.AddRange(new[] { "Scheduled", "Confirmed", "Completed", "Cancelled", "No-show" });

                    if (isEditMode)
                    {
                        cmbPatient.SelectedValue = appointment.patient_id;
                        cmbDoctor.SelectedValue = appointment.doctor_id;
                        cmbService.SelectedValue = appointment.service_id;
                        cmbStatus.SelectedItem = appointment.status;
                    }
                    else
                    {
                        cmbStatus.SelectedIndex = 0; // Default to Scheduled
                    }

                    await LoadAvailableTimeSlotsAsync();
                },
                "Loading...",
                "Error loading data"
            );
        }

        private async Task LoadAvailableTimeSlotsAsync()
        {
            if (cmbDoctor.SelectedValue == null) return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    int doctorId = (int)cmbDoctor.SelectedValue;
                    DateTime date = dtpAppointmentDate.Value.Date;

                    // Get booked time slots
                    var booked = await dbHelper.ExecuteQueryAsync<string>(
                        @"SELECT time_slot FROM Appointments 
                          WHERE doctor_id = @doctorId 
                          AND CAST(appointment_date AS DATE) = @date
                          AND status NOT IN ('Cancelled', 'No-show')",
                        new { doctorId, date }
                    );

                    // All available time slots
                    var allSlots = new[] { "08:00", "09:00", "10:00", "11:00", "13:00", "14:00", "15:00", "16:00", "17:00" };

                    // Filter out booked slots
                    var available = allSlots.Where(slot => !booked.Contains(slot)).ToList();

                    // Keep current slot if editing
                    if (isEditMode && !available.Contains(appointment.time_slot))
                    {
                        available.Add(appointment.time_slot);
                    }

                    cmbTimeSlot.Items.Clear();
                    cmbTimeSlot.Items.AddRange(available.OrderBy(s => s).ToArray());

                    if (isEditMode && available.Contains(appointment.time_slot))
                    {
                        cmbTimeSlot.SelectedItem = appointment.time_slot;
                    }
                },
                "Error loading time slots"
            );
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            bool success = await CodeTemplates.SaveWithValidationAsync(
                validationHelper,
                ValidateForm,
                SaveToDatabase,
                isEditMode ? "Appointment updated successfully!" : "Appointment created successfully!",
                "Error saving appointment"
            );

            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateForm()
        {
            return validationHelper.ValidateForm(
                cmbPatient,
                cmbDoctor,
                cmbService,
                cmbTimeSlot
            );
        }

        private async Task SaveToDatabase()
        {
            appointment.patient_id = (int)cmbPatient.SelectedValue;
            appointment.doctor_id = (int)cmbDoctor.SelectedValue;
            appointment.service_id = (int)cmbService.SelectedValue;
            appointment.appointment_date = dtpAppointmentDate.Value.Date.Add(TimeSpan.Parse(cmbTimeSlot.SelectedItem.ToString()));
            appointment.time_slot = cmbTimeSlot.SelectedItem.ToString();
            appointment.status = cmbStatus.SelectedItem.ToString();
            appointment.notes = txtNotes.Text;

            if (isEditMode)
            {
                await dbHelper.ExecuteNonQueryAsync(
                    @"UPDATE Appointments SET
                        patient_id = @patient_id,
                        doctor_id = @doctor_id,
                        service_id = @service_id,
                        appointment_date = @appointment_date,
                        time_slot = @time_slot,
                        status = @status,
                        notes = @notes
                      WHERE id = @id",
                    appointment
                );
            }
            else
            {
                appointment.appointment_code = $"APT{DateTime.Now:yyyyMMddHHmmss}";

                await dbHelper.ExecuteNonQueryAsync(
                    @"INSERT INTO Appointments (appointment_code, patient_id, doctor_id, service_id,
                        appointment_date, time_slot, status, notes)
                      VALUES (@appointment_code, @patient_id, @doctor_id, @service_id,
                        @appointment_date, @time_slot, @status, @notes)",
                    appointment
                );
            }

            CacheManager.Instance.InvalidateRelated("appointment");
        }
    }

    /// <summary>
    /// View form for appointment details
    /// </summary>
    public partial class AppointmentViewForm : Form
    {
        private Appointment appointment;

        public AppointmentViewForm(Appointment appointment)
        {
            this.appointment = appointment;

            CodeTemplates.InitializeForm(
                this,
                $"Appointment Details - {appointment.appointment_code}",
                600,
                500,
                FormStartPosition.CenterScreen
            );

            DisplayAppointmentInfo();
        }

        private void DisplayAppointmentInfo()
        {
            // Create labels to display appointment information
            // ... (simplified for brevity)
        }
    }

    /// <summary>
    /// Reschedule form
    /// </summary>
    public partial class AppointmentRescheduleForm : Form
    {
        private Appointment appointment;
        private DateTimePicker dtpNewDate;
        private ComboBox cmbNewTimeSlot;
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public AppointmentRescheduleForm(Appointment appointment)
        {
            this.appointment = appointment;

            CodeTemplates.InitializeForm(
                this,
                $"Reschedule - {appointment.appointment_code}",
                500,
                300,
                FormStartPosition.CenterScreen
            );

            SetupControls();
        }

        private void SetupControls()
        {
            // Similar to AppointmentAddEditForm but focused on date/time only
            // ...
        }
    }
}
