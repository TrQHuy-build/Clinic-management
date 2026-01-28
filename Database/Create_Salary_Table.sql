-- =============================================
-- CREATE SALARY TABLE AND SEED DATA
-- =============================================

USE DentalClinicDB;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Kiểm tra và tạo bảng Salary nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Salary')
BEGIN
    CREATE TABLE Salary (
        salary_id INT IDENTITY(1,1) PRIMARY KEY,
        staff_id INT NOT NULL,
        month INT NOT NULL CHECK (month BETWEEN 1 AND 12),
        year INT NOT NULL CHECK (year >= 2020 AND year <= 2100),
        base_salary DECIMAL(15,2) NOT NULL DEFAULT 0,
        bonus DECIMAL(15,2) DEFAULT 0,
        deduction DECIMAL(15,2) DEFAULT 0,
        total_salary AS (base_salary + ISNULL(bonus, 0) - ISNULL(deduction, 0)) PERSISTED,
        notes NVARCHAR(500),
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (staff_id) REFERENCES Staff(staff_id),
        CONSTRAINT UK_Salary_Staff_Month_Year UNIQUE (staff_id, month, year)
    );
    
    PRINT 'Table Salary created successfully';
END
ELSE
BEGIN
    PRINT 'Table Salary already exists';
END
GO

-- Kiểm tra và thêm dữ liệu mẫu cho bảng Salary
-- Tạo lương cho 6 tháng gần nhất

DECLARE @CurrentMonth INT = MONTH(GETDATE());
DECLARE @CurrentYear INT = YEAR(GETDATE());
DECLARE @MonthsToGenerate INT = 6;
DECLARE @i INT = 0;

PRINT ''
PRINT 'Seeding salary data for last 6 months...'
PRINT ''

WHILE @i < @MonthsToGenerate
BEGIN
    DECLARE @TargetMonth INT = @CurrentMonth - @i;
    DECLARE @TargetYear INT = @CurrentYear;
    
    -- Xử lý tháng âm (năm trước)
    WHILE @TargetMonth <= 0
    BEGIN
        SET @TargetMonth = @TargetMonth + 12;
        SET @TargetYear = @TargetYear - 1;
    END
    
    -- Insert salary cho tất cả staff trong tháng đó
    INSERT INTO Salary (staff_id, month, year, base_salary, bonus, deduction, notes)
    SELECT 
        s.staff_id,
        @TargetMonth AS month,
        @TargetYear AS year,
        ISNULL(s.salary, 10000000) AS base_salary,
        CASE 
            WHEN s.position = 'Doctor' THEN 
                CASE 
                    WHEN @TargetMonth IN (1, 6, 12) THEN 3000000 -- Tháng lễ tết
                    ELSE 1000000 + (ABS(CHECKSUM(NEWID())) % 2000000) -- Random 1-3 triệu
                END
            ELSE 
                CASE 
                    WHEN @TargetMonth IN (1, 6, 12) THEN 1000000
                    ELSE 500000 + (ABS(CHECKSUM(NEWID())) % 500000) -- Random 0.5-1 triệu
                END
        END AS bonus,
        CASE 
            WHEN (ABS(CHECKSUM(NEWID())) % 10) < 2 THEN -- 20% chance có khấu trừ
                200000 + (ABS(CHECKSUM(NEWID())) % 800000) -- Random 200k-1M
            ELSE 0
        END AS deduction,
        CASE 
            WHEN @TargetMonth IN (1, 6, 12) THEN N'Thưởng tháng lễ tết'
            WHEN (ABS(CHECKSUM(NEWID())) % 10) < 2 THEN N'Khấu trừ do đi muộn/vắng mặt'
            ELSE N'Lương tháng ' + CAST(@TargetMonth AS NVARCHAR) + '/' + CAST(@TargetYear AS NVARCHAR)
        END AS notes
    FROM Staff s
    INNER JOIN UserAccount u ON s.user_id = u.user_id
    WHERE u.status = 'active'
    AND NOT EXISTS (
        SELECT 1 FROM Salary sal 
        WHERE sal.staff_id = s.staff_id 
        AND sal.month = @TargetMonth 
        AND sal.year = @TargetYear
    );
    
    DECLARE @InsertedCount INT = @@ROWCOUNT;
    PRINT '  Month ' + CAST(@TargetMonth AS VARCHAR) + '/' + CAST(@TargetYear AS VARCHAR) + ': ' + CAST(@InsertedCount AS VARCHAR) + ' salary records';
    
    SET @i = @i + 1;
END

PRINT ''
PRINT 'Salary seeding completed!'
PRINT ''

-- Summary
SELECT 
    'SUMMARY' AS [Report],
    COUNT(DISTINCT staff_id) AS [Total Staff],
    COUNT(*) AS [Total Salary Records],
    MIN(CAST(month AS VARCHAR) + '/' + CAST(year AS VARCHAR)) AS [Earliest Month],
    MAX(CAST(month AS VARCHAR) + '/' + CAST(year AS VARCHAR)) AS [Latest Month],
    SUM(total_salary) AS [Total Paid],
    AVG(total_salary) AS [Average Salary]
FROM Salary;
GO

-- Show sample data
PRINT ''
PRINT 'Sample salary data:'
PRINT ''

SELECT TOP 10
    u.fullname AS [Nhân viên],
    s.position AS [Chức vụ],
    sal.month AS [Tháng],
    sal.year AS [Năm],
    FORMAT(sal.base_salary, 'N0') AS [Lương CB],
    FORMAT(sal.bonus, 'N0') AS [Thưởng],
    FORMAT(sal.deduction, 'N0') AS [Khấu trừ],
    FORMAT(sal.total_salary, 'N0') AS [Thực lĩnh],
    sal.notes AS [Ghi chú]
FROM Salary sal
INNER JOIN Staff s ON sal.staff_id = s.staff_id
INNER JOIN UserAccount u ON s.user_id = u.user_id
ORDER BY sal.year DESC, sal.month DESC, u.fullname;
GO

PRINT ''
PRINT '✅ Salary table created and seeded successfully!'
PRINT ''
