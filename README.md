# AppHub — Multi-Module WinForms Application

> .NET Framework 4.7.2 · SQL Server LocalDB · Visual Studio 2022

---

## โครงสร้าง Solution

```
AppHub.sln
├── AppHub.Core         ← Shared library (SQL, AppSession, Base forms)
├── AppHub.Launcher     ← Startup project (Login, MainForm MDI, User Management)
├── AppHub.CRUD         ← จัดการข้อมูลลูกค้า (CRUD)
├── AppHub.Import       ← นำเข้าข้อมูลจาก Excel
├── AppHub.Report       ← รายงานและ Export CSV
└── AppHub.Scan         ← Scan / Input รหัสลูกค้า
```

---

## ขั้นตอนการติดตั้ง

### 1. สร้างฐานข้อมูล

เปิด **SQL Server Management Studio** หรือ **Azure Data Studio** แล้วรัน:

```
setup.sql
```

ไฟล์นี้จะสร้าง database `AppHubDB` และ seed ข้อมูลเริ่มต้นให้อัตโนมัติ

### 2. Restore NuGet (EPPlus 5.8.14)

ใน Visual Studio: คลิกขวาที่ Solution → **Restore NuGet Packages**

### 3. เปิด Solution

เปิดไฟล์ `AppHub.sln` ใน Visual Studio 2022

### 4. ตั้ง Startup Project

คลิกขวาที่ `AppHub.Launcher` → **Set as Startup Project**

### 5. Build & Run

กด **F5** หรือ **Ctrl+F5**

---

## บัญชีผู้ใช้ Demo

| Username | Password  | สิทธิ์                           |
|----------|-----------|----------------------------------|
| admin    | admin1234 | Admin — เข้าถึงทุกโมดูล + จัดการผู้ใช้ |
| user1    | user1234  | User — เฉพาะ CRUD + Report        |

---

## โมดูล

| โมดูล   | Module Code | รายละเอียด                                |
|--------|-------------|------------------------------------------|
| CRUD   | CRUD        | เพิ่ม/แก้ไข/ลบ/ค้นหาข้อมูลลูกค้า         |
| Import | IMPORT      | นำเข้าไฟล์ Excel — ข้ามรายการซ้ำ          |
| Report | REPORT      | กรองและ Export รายงานเป็น CSV             |
| Scan   | SCAN        | Scan/Input รหัสลูกค้าและบันทึก Log       |

---

## Connection String

แก้ไขได้ที่ `AppHub.Launcher\App.config`:

```xml
<add name="AppHubDB"
     connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AppHubDB;Integrated Security=True"
     providerName="System.Data.SqlClient" />
```

สำหรับ SQL Express ให้เปลี่ยนเป็น:
```
Data Source=.\SQLEXPRESS;Initial Catalog=AppHubDB;Integrated Security=True
```

---

## ไฟล์ทดสอบ Excel

อยู่ในโฟลเดอร์ `TestData\`:

| ไฟล์                       | ใช้ทดสอบ                          |
|---------------------------|-----------------------------------|
| test_customers.xlsx        | Import ปกติ 10 แถว                |
| test_customers_dup.xlsx    | Import พร้อมรายการซ้ำ (3 ซ้ำ)     |
| test_empty.xlsx            | ไฟล์ว่าง — ไม่มีแถวข้อมูล          |
| test_wrong_header.xlsx     | Header ผิด — ต้องแสดง error        |

---

## Architecture Notes

- **MDI Pattern:** `MainForm` เป็น MDI container, แต่ละโมดูลเปิดเป็น MDI child
- **Session:** `AppSession` (static class) เก็บ user/permissions ใช้ร่วมกันทุก project
- **Connection String:** module projects ใช้ `ConfigurationManager` ดึงจาก `AppHub.Launcher\App.config` โดยอัตโนมัติ
- **Permission:** Admin เห็นทุกโมดูล, User เห็นเฉพาะที่ได้รับสิทธิ์
- **Password:** SHA-256 hash, ไม่เก็บ plain text
