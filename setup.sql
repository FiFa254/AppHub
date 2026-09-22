/* ============================================================================
   AppHub — สร้าง Database + Tables + Seed data
   รันซ้ำได้ (สร้างเฉพาะที่ยังไม่มี)
   ============================================================================ */

IF DB_ID(N'AppHubDB') IS NULL
    CREATE DATABASE AppHubDB;
GO

USE AppHubDB;
GO

/* ─── Users ────────────────────────────────────────────────────────────────── */
IF OBJECT_ID(N'dbo.t_Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.t_Users
    (
        User_id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_t_Users PRIMARY KEY,
        Username       NVARCHAR(50)      NOT NULL CONSTRAINT UQ_t_Users_Username UNIQUE,
        Password_hash  VARCHAR(64)       NOT NULL,   -- SHA-256 hex (lowercase)
        Full_name      NVARCHAR(100)     NOT NULL,
        Is_admin       BIT               NOT NULL CONSTRAINT DF_t_Users_Is_admin  DEFAULT (0),
        Is_active      BIT               NOT NULL CONSTRAINT DF_t_Users_Is_active DEFAULT (1),
        Created_date   DATETIME          NOT NULL CONSTRAINT DF_t_Users_Created   DEFAULT (GETDATE())
    );
END
GO

/* ─── User ↔ Module permission ─────────────────────────────────────────────── */
IF OBJECT_ID(N'dbo.t_UserModule', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.t_UserModule
    (
        User_id      INT          NOT NULL
            CONSTRAINT FK_t_UserModule_User REFERENCES dbo.t_Users (User_id) ON DELETE CASCADE,
        Module_code  VARCHAR(20)  NOT NULL,          -- CRUD / IMPORT / REPORT / SCAN
        CONSTRAINT PK_t_UserModule PRIMARY KEY (User_id, Module_code)
    );
END
GO

/* ─── Customers ────────────────────────────────────────────────────────────── */
IF OBJECT_ID(N'dbo.t_Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.t_Customers
    (
        Customer_id    INT IDENTITY(1,1) NOT NULL CONSTRAINT UQ_t_Customers_Id UNIQUE,
        Customer_code  NVARCHAR(20)      NOT NULL CONSTRAINT PK_t_Customers PRIMARY KEY,
        Full_name      NVARCHAR(200)     NOT NULL,
        Phone          NVARCHAR(50)      NULL,
        Email          NVARCHAR(200)     NULL,
        Address        NVARCHAR(500)     NULL,
        Created_date   DATETIME          NOT NULL CONSTRAINT DF_t_Customers_Created DEFAULT (GETDATE()),
        Updated_date   DATETIME          NULL
    );
END
GO

/* ─── Scan log (ไม่ผูก FK — log ต้องอยู่ต่อแม้ลบลูกค้า) ───────────────────────── */
IF OBJECT_ID(N'dbo.t_ScanLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.t_ScanLog
    (
        Log_id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_t_ScanLog PRIMARY KEY,
        Customer_code  NVARCHAR(20)      NOT NULL,
        Scanned_by     NVARCHAR(50)      NOT NULL,
        Scan_date      DATETIME          NOT NULL CONSTRAINT DF_t_ScanLog_Scan_date DEFAULT (GETDATE()),
        Note           NVARCHAR(500)     NULL
    );

    CREATE INDEX IX_t_ScanLog_Scan_date ON dbo.t_ScanLog (Scan_date DESC);
END
GO

/* ─── Seed: users ──────────────────────────────────────────────────────────── */
IF NOT EXISTS (SELECT 1 FROM dbo.t_Users WHERE Username = N'admin')
    INSERT INTO dbo.t_Users (Username, Password_hash, Full_name, Is_admin)
    VALUES (N'admin',
            LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin1234'), 2)),
            N'ผู้ดูแลระบบ', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.t_Users WHERE Username = N'user1')
    INSERT INTO dbo.t_Users (Username, Password_hash, Full_name, Is_admin)
    VALUES (N'user1',
            LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'user1234'), 2)),
            N'ผู้ใช้ทั่วไป', 0);
GO

/* ─── Seed: permissions (admin ได้ทุก module อยู่แล้ว แต่ใส่ไว้ให้ครบ) ────────── */
INSERT INTO dbo.t_UserModule (User_id, Module_code)
SELECT u.User_id, m.Module_code
FROM   dbo.t_Users u
CROSS  JOIN (VALUES ('CRUD'), ('IMPORT'), ('REPORT'), ('SCAN')) m(Module_code)
WHERE  u.Username = N'admin'
  AND  NOT EXISTS (SELECT 1 FROM dbo.t_UserModule x
                   WHERE x.User_id = u.User_id AND x.Module_code = m.Module_code);

INSERT INTO dbo.t_UserModule (User_id, Module_code)
SELECT u.User_id, m.Module_code
FROM   dbo.t_Users u
CROSS  JOIN (VALUES ('CRUD'), ('REPORT')) m(Module_code)
WHERE  u.Username = N'user1'
  AND  NOT EXISTS (SELECT 1 FROM dbo.t_UserModule x
                   WHERE x.User_id = u.User_id AND x.Module_code = m.Module_code);
GO

/* ─── Seed: customers (C001–C003 ใช้ทดสอบ import ซ้ำ) ──────────────────────── */
INSERT INTO dbo.t_Customers (Customer_code, Full_name, Phone, Email, Address)
SELECT s.Customer_code, s.Full_name, s.Phone, s.Email, s.Address
FROM (VALUES
    (N'C001', N'สมชาย ใจดี',     N'081-111-1111', N'somchai@example.com', N'กรุงเทพมหานคร'),
    (N'C002', N'สมหญิง รักงาน',   N'082-222-2222', N'somying@example.com', N'เชียงใหม่'),
    (N'C003', N'วิชัย มั่นคง',     N'083-333-3333', N'wichai@example.com',  N'ขอนแก่น'),
    (N'C004', N'มาลี สวยงาม',     N'084-444-4444', N'malee@example.com',   N'ภูเก็ต'),
    (N'C005', N'ประเสริฐ ศรีสุข', N'085-555-5555', N'prasert@example.com', N'ชลบุรี')
) s(Customer_code, Full_name, Phone, Email, Address)
WHERE NOT EXISTS (SELECT 1 FROM dbo.t_Customers c WHERE c.Customer_code = s.Customer_code);
GO

PRINT N'AppHubDB พร้อมใช้งาน';
