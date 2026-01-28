USE DentalClinicDB;
GO

-- Drop existing objects
IF OBJECT_ID('vw_AppointmentFull', 'V') IS NOT NULL
    DROP VIEW vw_AppointmentFull;
GO

IF OBJECT_ID('trg_Prescription_AfterInsert', 'TR') IS NOT NULL
    DROP TRIGGER trg_Prescription_AfterInsert;
GO

-- Create View
CREATE VIEW vw_AppointmentFull
AS
SELECT 
    a.appointment_id,
    a.appointment_code,
    a.appointment_date,
    a.status,
    a.notes,
    a.check_in_time,
    a.queue_number,
    a.estimated_start_time,
    ISNULL(u.fullname, a.patient_name) AS patient_name,
    ISNULL(u.phone, a.phone) AS patient_phone,
    ISNULL(u.email, a.email) AS patient_email,
    p.patient_id,
    s.service_id,
    s.service_name,
    s.price AS service_price,
    st.staff_id AS doctor_id,
    doctor_u.fullname AS doctor_name,
    st.specialization,
    a.created_at,
    a.updated_at
FROM Appointment a
LEFT JOIN Patient p ON a.patient_id = p.patient_id
LEFT JOIN UserAccount u ON p.user_id = u.user_id
LEFT JOIN Service s ON a.service_id = s.service_id
LEFT JOIN Staff st ON a.assigned_doctor_id = st.staff_id
LEFT JOIN UserAccount doctor_u ON st.user_id = doctor_u.user_id
WHERE a.is_deleted = 0;
GO

PRINT 'View created successfully';
GO

-- Create Trigger
CREATE TRIGGER trg_Prescription_AfterInsert
ON Prescription
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Deduct stock
    UPDATE m
    SET m.stock_quantity = m.stock_quantity - i.quantity
    FROM Medicine m
    INNER JOIN inserted i ON m.medicine_id = i.medicine_id
    WHERE m.stock_quantity >= i.quantity;
END;
GO

PRINT 'Trigger created successfully';
GO
