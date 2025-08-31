-- =============================================
-- HospitalManagement CRUD Stored Procedures
-- Drops and recreates all CRUD procs for all tables
-- =============================================

-- ========== APPOINTMENT ==========
IF OBJECT_ID('dbo.PR_Appointment_SelectAll', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Appointment_SelectAll;
GO
CREATE PROCEDURE dbo.PR_Appointment_SelectAll
AS
BEGIN
    SELECT * FROM dbo.Appointment;
END
GO

IF OBJECT_ID('dbo.PR_Appointment_SelectByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Appointment_SelectByPK;
GO
CREATE PROCEDURE dbo.PR_Appointment_SelectByPK
    @AppointmentID INT
AS
BEGIN
    SELECT * FROM dbo.Appointment WHERE AppointmentID = @AppointmentID;
END
GO

IF OBJECT_ID('dbo.PR_Appointment_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Appointment_Insert;
GO
CREATE PROCEDURE dbo.PR_Appointment_Insert
    @DoctorID INT,
    @PatientID INT,
    @AppointmentDate DATETIME,
    @AppointmentStatus NVARCHAR(20),
    @Description NVARCHAR(250),
    @SpecialRemarks NVARCHAR(100),
    @Created DATETIME,
    @Modified DATETIME,
    @UserID INT,
    @TotalConsultedAmount DECIMAL(18,2),
    @NewAppointmentID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Appointment (DoctorID, PatientID, AppointmentDate, AppointmentStatus, Description, SpecialRemarks, Created, Modified, UserID, TotalConsultedAmount)
    VALUES (@DoctorID, @PatientID, @AppointmentDate, @AppointmentStatus, @Description, @SpecialRemarks, @Created, @Modified, @UserID, @TotalConsultedAmount);
    SET @NewAppointmentID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.PR_Appointment_UpdateByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Appointment_UpdateByPK;
GO
CREATE PROCEDURE dbo.PR_Appointment_UpdateByPK
    @AppointmentID INT,
    @DoctorID INT,
    @PatientID INT,
    @AppointmentDate DATETIME,
    @AppointmentStatus NVARCHAR(20),
    @Description NVARCHAR(250),
    @SpecialRemarks NVARCHAR(100),
    @Modified DATETIME,
    @UserID INT,
    @TotalConsultedAmount DECIMAL(18,2)
AS
BEGIN
    UPDATE dbo.Appointment
    SET DoctorID = @DoctorID,
        PatientID = @PatientID,
        AppointmentDate = @AppointmentDate,
        AppointmentStatus = @AppointmentStatus,
        Description = @Description,
        SpecialRemarks = @SpecialRemarks,
        Modified = @Modified,
        UserID = @UserID,
        TotalConsultedAmount = @TotalConsultedAmount
    WHERE AppointmentID = @AppointmentID;
END
GO

IF OBJECT_ID('dbo.PR_Appointment_DeleteByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Appointment_DeleteByPK;
GO
CREATE PROCEDURE dbo.PR_Appointment_DeleteByPK
    @AppointmentID INT
AS
BEGIN
    DELETE FROM dbo.Appointment WHERE AppointmentID = @AppointmentID;
END
GO

-- ========== DEPARTMENT ==========
IF OBJECT_ID('dbo.PR_Department_SelectAll', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Department_SelectAll;
GO
CREATE PROCEDURE dbo.PR_Department_SelectAll
AS
BEGIN
    SELECT * FROM dbo.Department;
END
GO

IF OBJECT_ID('dbo.PR_Department_SelectByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Department_SelectByPK;
GO
CREATE PROCEDURE dbo.PR_Department_SelectByPK
    @DepartmentID INT
AS
BEGIN
    SELECT * FROM dbo.Department WHERE DepartmentID = @DepartmentID;
END
GO

IF OBJECT_ID('dbo.PR_Department_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Department_Insert;
GO
CREATE PROCEDURE dbo.PR_Department_Insert
    @DepartmentName NVARCHAR(100),
    @Description NVARCHAR(250),
    @IsActive BIT,
    @Created DATETIME,
    @Modified DATETIME,
    @UserID INT,
    @NewDepartmentID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Department (DepartmentName, Description, IsActive, Created, Modified, UserID)
    VALUES (@DepartmentName, @Description, @IsActive, @Created, @Modified, @UserID);
    SET @NewDepartmentID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.PR_Department_UpdateByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Department_UpdateByPK;
GO
CREATE PROCEDURE dbo.PR_Department_UpdateByPK
    @DepartmentID INT,
    @DepartmentName NVARCHAR(100),
    @Description NVARCHAR(250),
    @IsActive BIT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE dbo.Department
    SET DepartmentName = @DepartmentName,
        Description = @Description,
        IsActive = @IsActive,
        Modified = @Modified,
        UserID = @UserID
    WHERE DepartmentID = @DepartmentID;
END
GO

IF OBJECT_ID('dbo.PR_Department_DeleteByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Department_DeleteByPK;
GO
CREATE PROCEDURE dbo.PR_Department_DeleteByPK
    @DepartmentID INT
AS
BEGIN
    DELETE FROM dbo.Department WHERE DepartmentID = @DepartmentID;
END
GO

-- ========== DOCTOR ==========
IF OBJECT_ID('dbo.PR_Doctor_SelectAll', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Doctor_SelectAll;
GO
CREATE PROCEDURE dbo.PR_Doctor_SelectAll
AS
BEGIN
    SELECT * FROM dbo.Doctor;
END
GO

IF OBJECT_ID('dbo.PR_Doctor_SelectByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Doctor_SelectByPK;
GO
CREATE PROCEDURE dbo.PR_Doctor_SelectByPK
    @DoctorID INT
AS
BEGIN
    SELECT * FROM dbo.Doctor WHERE DoctorID = @DoctorID;
END
GO

IF OBJECT_ID('dbo.PR_Doctor_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Doctor_Insert;
GO
CREATE PROCEDURE dbo.PR_Doctor_Insert
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Qualification NVARCHAR(100),
    @Specialization NVARCHAR(100),
    @IsActive BIT,
    @Created DATETIME,
    @Modified DATETIME,
    @UserID INT,
    @NewDoctorID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Doctor (Name, Phone, Email, Qualification, Specialization, IsActive, Created, Modified, UserID)
    VALUES (@Name, @Phone, @Email, @Qualification, @Specialization, @IsActive, @Created, @Modified, @UserID);
    SET @NewDoctorID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.PR_Doctor_UpdateByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Doctor_UpdateByPK;
GO
CREATE PROCEDURE dbo.PR_Doctor_UpdateByPK
    @DoctorID INT,
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Qualification NVARCHAR(100),
    @Specialization NVARCHAR(100),
    @IsActive BIT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE dbo.Doctor
    SET Name = @Name,
        Phone = @Phone,
        Email = @Email,
        Qualification = @Qualification,
        Specialization = @Specialization,
        IsActive = @IsActive,
        Modified = @Modified,
        UserID = @UserID
    WHERE DoctorID = @DoctorID;
END
GO

IF OBJECT_ID('dbo.PR_Doctor_DeleteByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Doctor_DeleteByPK;
GO
CREATE PROCEDURE dbo.PR_Doctor_DeleteByPK
    @DoctorID INT
AS
BEGIN
    DELETE FROM DoctorDepartment WHERE DoctorID = @DoctorID;
    DELETE FROM Doctor WHERE DoctorID = @DoctorID;
END
GO

-- ========== DOCTORDEPARTMENT ==========
IF OBJECT_ID('dbo.PR_DoctorDepartment_SelectAll', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_DoctorDepartment_SelectAll;
GO
CREATE PROCEDURE dbo.PR_DoctorDepartment_SelectAll
AS
BEGIN
    SELECT * FROM dbo.DoctorDepartment;
END
GO

IF OBJECT_ID('dbo.PR_DoctorDepartment_SelectByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_DoctorDepartment_SelectByPK;
GO
CREATE PROCEDURE dbo.PR_DoctorDepartment_SelectByPK
    @DoctorDepartmentID INT
AS
BEGIN
    SELECT * FROM dbo.DoctorDepartment WHERE DoctorDepartmentID = @DoctorDepartmentID;
END
GO

IF OBJECT_ID('dbo.PR_DoctorDepartment_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_DoctorDepartment_Insert;
GO
CREATE PROCEDURE dbo.PR_DoctorDepartment_Insert
    @DoctorID INT,
    @DepartmentID INT,
    @Created DATETIME,
    @Modified DATETIME,
    @UserID INT,
    @NewDoctorDepartmentID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.DoctorDepartment (DoctorID, DepartmentID, Created, Modified, UserID)
    VALUES (@DoctorID, @DepartmentID, @Created, @Modified, @UserID);
    SET @NewDoctorDepartmentID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.PR_DoctorDepartment_UpdateByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_DoctorDepartment_UpdateByPK;
GO
CREATE PROCEDURE dbo.PR_DoctorDepartment_UpdateByPK
    @DoctorDepartmentID INT,
    @DoctorID INT,
    @DepartmentID INT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE dbo.DoctorDepartment
    SET DoctorID = @DoctorID,
        DepartmentID = @DepartmentID,
        Modified = @Modified,
        UserID = @UserID
    WHERE DoctorDepartmentID = @DoctorDepartmentID;
END
GO

IF OBJECT_ID('dbo.PR_DoctorDepartment_DeleteByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_DoctorDepartment_DeleteByPK;
GO
CREATE PROCEDURE dbo.PR_DoctorDepartment_DeleteByPK
    @DoctorDepartmentID INT
AS
BEGIN
    DELETE FROM dbo.DoctorDepartment WHERE DoctorDepartmentID = @DoctorDepartmentID;
END
GO

-- ========== USER ==========
IF OBJECT_ID('dbo.PR_User_SelectAll', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_User_SelectAll;
GO
CREATE PROCEDURE dbo.PR_User_SelectAll
AS
BEGIN
    SELECT * FROM dbo.[User];
END
GO

IF OBJECT_ID('dbo.PR_User_SelectByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_User_SelectByPK;
GO
CREATE PROCEDURE dbo.PR_User_SelectByPK
    @UserID INT
AS
BEGIN
    SELECT * FROM dbo.[User] WHERE UserID = @UserID;
END
GO

IF OBJECT_ID('dbo.PR_User_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_User_Insert;
GO
CREATE PROCEDURE dbo.PR_User_Insert
    @UserName NVARCHAR(100),
    @Password NVARCHAR(100),
    @Email NVARCHAR(100),
    @MobileNo NVARCHAR(100),
    @IsActive BIT,
    @Created DATETIME,
    @Modified DATETIME,
    @NewUserID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.[User] (UserName, Password, Email, MobileNo, IsActive, Created, Modified)
    VALUES (@UserName, @Password, @Email, @MobileNo, @IsActive, @Created, @Modified);
    SET @NewUserID = SCOPE_IDENTITY();
END
GO

-- Add a validate-login stored procedure so the application can authenticate users.
-- This returns the columns the controller expects: UserID, UserName, Email and Role (role is provided as empty string if the table doesn't have one).
IF OBJECT_ID('dbo.PR_User_ValidateLogin', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_User_ValidateLogin;
GO
CREATE PROCEDURE dbo.PR_User_ValidateLogin
    @UserName NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    -- Select matching active user; include a Role column to satisfy Reader access in controller.
    SELECT TOP 1
        UserID,
        UserName,
        Email,
        -- If your User table has a Role column, replace the next line with that column name.
        CAST('' AS NVARCHAR(50)) AS Role
    FROM dbo.[User]
    WHERE UserName = @UserName
      AND Password = @Password
      AND IsActive = 1;
END
GO

IF OBJECT_ID('dbo.PR_User_DeleteByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_User_DeleteByPK;
GO
CREATE PROCEDURE dbo.PR_User_DeleteByPK
    @UserID INT
AS
BEGIN
    DELETE FROM dbo.[User] WHERE UserID = @UserID;
END
GO

-- ========== PATIENT ==========
IF OBJECT_ID('dbo.PR_Patient_SelectAll', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Patient_SelectAll;
GO
CREATE PROCEDURE dbo.PR_Patient_SelectAll
AS
BEGIN
    SELECT * FROM dbo.Patient;
END
GO

IF OBJECT_ID('dbo.PR_Patient_SelectByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Patient_SelectByPK;
GO
CREATE PROCEDURE dbo.PR_Patient_SelectByPK
    @PatientID INT
AS
BEGIN
    SELECT * FROM dbo.Patient WHERE PatientID = @PatientID;
END
GO

IF OBJECT_ID('dbo.PR_Patient_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Patient_Insert;
GO
CREATE PROCEDURE dbo.PR_Patient_Insert
    @Name NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(100),
    @Address NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @IsActive BIT,
    @Created DATETIME,
    @Modified DATETIME,
    @UserID INT,
    @NewPatientID INT OUTPUT
AS
BEGIN
    INSERT INTO dbo.Patient (Name, DateOfBirth, Gender, Email, Phone, Address, City, State, IsActive, Created, Modified, UserID)
    VALUES (@Name, @DateOfBirth, @Gender, @Email, @Phone, @Address, @City, @State, @IsActive, @Created, @Modified, @UserID);
    SET @NewPatientID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.PR_Patient_UpdateByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Patient_UpdateByPK;
GO
CREATE PROCEDURE dbo.PR_Patient_UpdateByPK
    @PatientID INT,
    @Name NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(100),
    @Address NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @IsActive BIT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE dbo.Patient
    SET Name = @Name,
        DateOfBirth = @DateOfBirth,
        Gender = @Gender,
        Email = @Email,
        Phone = @Phone,
        Address = @Address,
        City = @City,
        State = @State,
        IsActive = @IsActive,
        Modified = @Modified,
        UserID = @UserID
    WHERE PatientID = @PatientID;
END
GO

IF OBJECT_ID('dbo.PR_Patient_DeleteByPK', 'P') IS NOT NULL DROP PROCEDURE dbo.PR_Patient_DeleteByPK;
GO
CREATE PROCEDURE dbo.PR_Patient_DeleteByPK
    @PatientID INT
AS
BEGIN
    DELETE FROM dbo.Patient WHERE PatientID = @PatientID;
END
GO