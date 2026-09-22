# AppHub — CLAUDE.md

> Context file for Claude Code CLI. อ่านก่อนแก้ไข code ทุกครั้ง

---

## Project Overview

**AppHub** คือ WinForms MDI application (.NET Framework 4.7.2, C#) สำหรับจัดการข้อมูลลูกค้า
ประกอบด้วย 6 projects + 1 test project ใน Visual Studio Solution เดียว

---

## Solution Structure

```
AppHub\
├── AppHub.sln
├── setup.sql                          ← สร้าง DB + tables + seed data
├── README.md
├── CLAUDE.md                          ← ไฟล์นี้
│
├── AppHub.Core\                       ← Shared library (ไม่มี UI form)
│   ├── AppHub.Core.csproj
│   ├── Data\
│   │   └── SQL.cs                     ← Static DB helper (หัวใจสำคัญ)
│   ├── Security\
│   │   ├── AccountRules.cs            ← กฎ username / รหัสผ่าน, SHA-256, ค่าล็อกบัญชี
│   │   └── AppSession.cs              ← Static session state
│   └── UI\
│       ├── AppHubForm.cs              ← Base form (ShowError, ShowModal, ShowDialogChild ...)
│       ├── AppHubCRUDForm.cs          ← Base CRUD form (DGV ในการ์ด, LblTitle, LblCount, PnlFooter)
│       ├── Theme.cs                   ← Design system: สี, ฟอนต์, ไอคอน, สไตล์ปุ่ม/ตาราง
│       ├── DialogChrome.cs            ← กรอบ dialog แบบใหม่ (หัว + ปุ่มปิด + ลากย้ายได้)
│       └── UiServices.cs              ← จุดเปิด MessageBox / dialog / file picker (test แทนที่ได้)
│
├── AppHub.Launcher\                   ← Startup project (EXE)
│   ├── AppHub.Launcher.csproj
│   ├── App.config                     ← Connection string อยู่ที่นี่
│   ├── Program.cs
│   ├── Forms\
│   │   ├── MainForm.cs / .Designer.cs          ← MDI container + sidebar
│   │   ├── LoginForm.cs / .Designer.cs
│   │   ├── RegisterForm.cs / .Designer.cs      ← สมัครสมาชิก (เปิดจากลิงก์ในหน้า Login)
│   │   └── UserManagementForm.cs / .Designer.cs
│   ├── Dialogs\
│   │   ├── UserEditDialog.cs
│   │   ├── PermissionDialog.cs        ← + ModuleCatalog (อ่าน t_Modules)
│   │   └── ChangePasswordDialog.cs    ← เปลี่ยนรหัสผ่านเอง / Admin รีเซ็ต
│   └── Controls\
│       ├── BrandPanel.cs              ← แผงแบรนด์ซ้ายของหน้า Login / สมัครสมาชิก
│       └── PasswordRulesView.cs       ← checklist เงื่อนไขรหัสผ่าน (✔ / ✘)
│
├── AppHub.CRUD\                       ← Module: จัดการลูกค้า
│   ├── AppHub.CRUD.csproj
│   ├── Forms\CustomerCRUDForm.cs / .Designer.cs
│   └── Dialogs\CustomerEditDialog.cs
│
├── AppHub.Import\                     ← Module: นำเข้า Excel
│   ├── AppHub.Import.csproj
│   ├── packages.config                ← EPPlus 5.8.14
│   └── Forms\ImportForm.cs / .Designer.cs
│
├── AppHub.Report\                     ← Module: รายงาน + Export CSV
│   ├── AppHub.Report.csproj
│   └── Forms\ReportForm.cs / .Designer.cs
│
├── AppHub.Scan\                       ← Module: Scan บัตร/QR
│   ├── AppHub.Scan.csproj
│   └── Forms\ScanForm.cs / .Designer.cs
│
└── AppHub.Tests\                      ← MSTest — ครอบคลุมทุก TC ใน AppHub_TestPlan.md
    ├── AppHub.Tests.csproj
    ├── App.config                     ← connection string ของ AppHubDB_Test
    ├── AppHub.runsettings             ← บังคับ STA thread (WinForms)
    ├── Infrastructure\                ← TestDb, Ui (reflection), UiTestBase
    └── Tests\*Tests.cs                ← 1 ไฟล์ต่อ module (TestCategory = TC-xx-nn)
```

### Folder convention (Folder-by-Type)

ทุก project จัดไฟล์ตาม **ประเภท** — ไฟล์ใหม่ต้องลงโฟลเดอร์ตามนี้:

| โฟลเดอร์ | ใส่อะไร |
|---|---|
| `Forms\` | หน้าจอหลัก (form + `.Designer.cs` คู่กัน) |
| `Dialogs\` | dialog ที่เปิดจาก form (เพิ่ม/แก้ไข/ยืนยันข้อมูล) |
| `Controls\` | UserControl / control วาดเอง ที่ใช้ซ้ำ |
| `Data\` · `Security\` · `UI\` | เฉพาะ `AppHub.Core` — DB, บัญชี/สิทธิ์, base UI + theme |
| `Infrastructure\` · `Tests\` | เฉพาะ `AppHub.Tests` — ตัวช่วย test / test class |

- **namespace ไม่เปลี่ยนตามโฟลเดอร์** — ใช้ชื่อ project (`AppHub.Launcher`, `AppHub.CRUD` ...) เหมือนเดิม ยกเว้น `AppHub.Core.UI`
- ย้าย/เพิ่มไฟล์แล้วต้องแก้ `<Compile Include="Forms\X.cs">` ใน `.csproj` (old-style csproj ไม่ include อัตโนมัติ)

### Project GUIDs

| Project | GUID |
|---|---|
| AppHub.Core | `{A1B2C3D4-0001-0001-0001-000000000001}` |
| AppHub.Launcher | `{A1B2C3D4-0002-0002-0002-000000000002}` |
| AppHub.Import | `{A1B2C3D4-0003-0003-0003-000000000003}` |
| AppHub.Report | `{A1B2C3D4-0004-0004-0004-000000000004}` |
| AppHub.Scan | `{A1B2C3D4-0005-0005-0005-000000000005}` |
| AppHub.CRUD | `{A1B2C3D4-0006-0006-0006-000000000006}` |
| AppHub.Tests | `{A1B2C3D4-0007-0007-0007-000000000007}` |

---

## Architecture Patterns — อ่านให้ขึ้นใจก่อนแก้ code

### 1. Static SQL Pattern (บังคับใช้ทุก form)

```csharp
// ✅ CORRECT
try
{
    SQL.Connect();           // ← ต้องอยู่ใน try เสมอ
    var dt = SQL.ExecuteQuery(sql, parameters);
    // ... ใช้ข้อมูล
}
catch (Exception ex)
{
    ShowError("ข้อความ:\n" + ex.Message);
    return;                  // ← return หลัง ShowError ใน catch
}
finally
{
    SQL.Disconnect();        // ← ต้องอยู่ใน finally เสมอ ไม่มีข้อยกเว้น
}

// ❌ WRONG — ห้ามเด็ดขาด
SQL.Connect();
try { ... }
finally { SQL.Disconnect(); }
```

**SQL Methods:**
- `SQL.ExecuteQuery(sql, dict)` → `DataTable`
- `SQL.ExecuteCommand(sql, dict)` → `int` (rows affected)
- `SQL.ExecuteScalar(sql, dict)` → `object`
- Parameters: `new Dictionary<string, object> { { "@param", value } }`

### 2. Form Inheritance

```csharp
// Form ทั่วไป
public partial class MyForm : AppHubForm { }

// Form ที่มี DataGridView + Title + Count
public partial class MyForm : AppHubCRUDForm { }
```

**AppHubForm helpers:**
- `ShowError(message, title)` — MessageBox Error
- `ShowWarning(message, title)` — MessageBox Warning
- `ShowInfo(message, title)` — MessageBox Information (แจ้งผลสำเร็จ)
- `Confirm(message, title)` → `bool` — MessageBox YesNo
- `ShowDialogChild(dialog, onOk)` — **เปิด dialog เป็นหน้าต่างลูกใน MainForm (MDI)** กลางจอ, ล็อก form เจ้าของระหว่างเปิด, เรียก `onOk` หลังกด OK แล้ว dialog ปิด — ใช้กับทุก dialog ที่เปิดจาก form ใน MainForm
- `ShowModal(dialog)` → `DialogResult` — modal เฉพาะ form ที่ยังไม่มี MainForm (หน้า Login → สมัครสมาชิก)
- `PickOpenFile(filter, title)` / `PickSaveFile(filter, fileName)` → path หรือ `null` — ใช้แทน OpenFileDialog / SaveFileDialog
- `SetPlaceholder(TextBox, text)` — ข้อความจางใน TextBox (.NET Framework ไม่มี `PlaceholderText`)

**AppHubCRUDForm extras:**
- `LblTitle` — Label หัวข้อ
- `LblCount` — Label แสดงจำนวน record (อยู่ใน `PnlFooter`)
- `DGV` — DataGridView หลัก — ใช้ตัวนี้เสมอ **ห้ามสร้าง grid / title / count ซ้ำใน Designer**
- `PnlFooter` — panel ล่าง, ปุ่มที่เพิ่มให้ใช้ `Dock = Right`
- `UpdateCount(int)` — อัปเดต LblCount
- `HideColumn(string)` — ซ่อน column ใน DGV
- Designer ของ form ลูกเพิ่มแค่ toolbar/filter panel (`Dock = Top`) — base จัดลำดับ dock ให้ใน `OnLoad`
- โหลดข้อมูลใน `override OnLoad` (เรียก `base.OnLoad(e)` ก่อน) ไม่ใช่ใน constructor

### 3. AppSession (Static)

```csharp
AppSession.UserId    // int
AppSession.Username  // string
AppSession.FullName  // string
AppSession.IsAdmin   // bool
AppSession.HasPermission("CRUD")   // bool — ตรวจ module permission
AppSession.HasAnyModule             // bool — มีสิทธิ์อย่างน้อย 1 module (Admin = true)
AppSession.SetPermissions(List<string> codes)
```

### 4. MDI Pattern

- `MainForm` คือ MDI container (`IsMdiContainer = true`)
- แต่ละ module form ถูก show แบบ MDI child:
  ```csharp
  var form = new CustomerCRUDForm();
  form.MdiParent = this;
  form.Show();
  ```
- เปิดได้จาก menu items ใน MainForm
- Dialog (เพิ่ม/แก้ไข, กำหนดสิทธิ์, รหัสผ่าน) ก็เป็น MDI child ด้วย — เปิดผ่าน `ShowDialogChild`:
  ```csharp
  var dlg = new CustomerEditDialog("เพิ่ม Customer ใหม่");
  ShowDialogChild(dlg, () => AddCustomer(dlg));   // โค้ดบันทึกแยกเป็น method
  ```
  ปุ่มที่ตั้ง `DialogResult` (ยกเลิก / Esc) ถูกสั่งปิดให้อัตโนมัติ — ปุ่ม OK ที่ validate ต้อง `DialogResult = OK; Close();` เอง

### 5. Design System (`AppHub.Core\UI\Theme.cs`)

`AppHubForm.OnLoad` เรียก `Theme.ApplyForm(this)` ให้อัตโนมัติ — **ห้ามตั้งสีปุ่ม / grid / panel ใน Designer**

- **สี:** Slate + Blue — `Theme.Background` (พื้นหน้าจอ), `Surface` (การ์ด/dialog), `Primary`, `Danger`, `Sidebar` ...
- **บทบาทปุ่ม** (ตั้งที่ `Button.Tag`): `primary` · `secondary` (ค่าเริ่มต้น) · `danger` · `ghost` · `icon` · `nav`
  - `AcceptButton` ของ form เป็น `primary` อัตโนมัติ
- **ไอคอน:** `Theme.SetIcon(btnAdd, Theme.Icons.Add)` ใน constructor — ใช้ฟอนต์ Segoe MDL2 Assets, สีตามบทบาทปุ่ม
  - **ห้ามใส่ emoji ในข้อความ** — GDI วาดเป็นกล่อง ▯ (มี test `NoEmojiInAnyScreen` ตรวจ) ใช้ได้แค่ ✔ ✘
  - ไอคอนใหม่: ใส่ code point ใน `Theme.Icons` เป็น escape `''`
- **ตาราง:** หัวคอลัมน์ภาษาไทยจาก `Theme.ColumnCaptions` (เพิ่มคอลัมน์ใหม่ที่นี่), วันที่ `dd/MM/yyyy HH:mm`
- **Title:** Label ชื่อ `lblTitle` / `LblTitle` ได้สไตล์หัวข้ออัตโนมัติ, panel `Dock = Top` ที่ไม่มี Tag = toolbar สีขาว
- **MainForm:** sidebar ซ้าย — module เปิดแบบไร้กรอบเต็มพื้นที่ มีปุ่มปิดที่หัวข้อ; dialog ใช้ `DialogChrome` (หัว + ปุ่มปิด)

---

## Database

### Connection String (App.config ของ Launcher)

```xml
<connectionStrings>
  <add name="AppHubDB"
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AppHubDB;Integrated Security=True"
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

### Tables

```sql
dbo.t_Users         -- User_id, Username, Password_hash (SHA-256), Full_name, Is_admin, Is_active, Created_date,
                    --   Failed_login_count, Locked_until, Last_login_date
dbo.t_Modules       -- Module_code (PK), Module_name, Sort_order
dbo.t_UserModule    -- User_id, Module_code (FK → t_Modules)
dbo.t_Customers     -- Customer_id, Customer_code (PK), Full_name, Phone, Email, Address, Created_date, Updated_date
dbo.t_ScanLog       -- Log_id, Customer_code, Scanned_by, Scan_date, Note
```

### Module Codes (Permission)

| Code | Module |
|---|---|
| `CRUD` | จัดการลูกค้า (AppHub.CRUD) |
| `IMPORT` | นำเข้า Excel (AppHub.Import) |
| `REPORT` | รายงาน (AppHub.Report) |
| `SCAN` | Scan บัตร (AppHub.Scan) |

### Demo Accounts

| Username | Password | Role |
|---|---|---|
| `admin` | `admin1234` | Admin (เข้าถึงทุก module + User Management) |
| `user1` | `user1234` | User (CRUD, REPORT) |

> รหัส demo ตั้งไว้ก่อนมี password policy — login ได้ปกติ แต่รหัสใหม่ต้องผ่านเงื่อนไข

### Account Security

- ใส่รหัสผิดครบ `AccountRules.MaxFailedLogins` (5) ครั้ง → ล็อก `LockoutMinutes` (15) นาที
- Admin ปลดล็อกได้ด้วย "รีเซ็ตรหัสผ่าน"
- ผู้ที่สมัครเอง = active แต่ไม่มีสิทธิ์ module จนกว่า Admin จะกำหนด

---

## Key Dependencies

| Library | Version | ใช้ใน |
|---|---|---|
| EPPlus | 5.8.14 | AppHub.Import — อ่าน Excel |
| System.Data.SqlClient | (built-in) | AppHub.Core — SQL Server |
| System.Configuration | (built-in) | AppHub.Core — ConnectionString |
| System.Windows.Forms | (built-in) | ทุก project |

---

## Build & Setup

1. เปิด `AppHub.sln` ใน Visual Studio 2022
2. Restore NuGet: `Tools → NuGet Package Manager → Restore`
3. รัน `setup.sql` ใน SQL Server LocalDB
4. ตั้ง `AppHub.Launcher` เป็น Startup Project
5. `F5` เพื่อ build และ run

### Run tests

- Visual Studio: `Test → Run All Tests` (ใช้ `AppHub.Tests\AppHub.runsettings` อัตโนมัติ)
- Command line (หลัง build):
  ```
  vstest.console.exe AppHub.Tests\bin\Debug\AppHub.Tests.dll /Settings:AppHub.Tests\AppHub.runsettings /TestAdapterPath:packages\MSTest.TestAdapter.2.2.10\build\_common
  ```
- Test ลบและสร้าง `AppHubDB_Test` ใหม่จาก `setup.sql` ทุกครั้ง — ไม่แตะ `AppHubDB`

---

## Coding Conventions

- **ภาษา:** Comments และ UI text เป็นภาษาไทย, code เป็น English
- **Naming:** PascalCase สำหรับ methods/properties, camelCase สำหรับ local variables
- **Prefix controls:** `txt` = TextBox, `btn` = Button, `lbl` = Label, `dgv` = DataGridView, `chk` = CheckBox, `dtp` = DateTimePicker
- **Error handling:** ใช้ `ShowError()` เสมอ ไม่ใช้ `MessageBox.Show()` โดยตรง
- **Modal UI:** ห้ามเรียก `MessageBox.Show` / `ShowDialog` / `OpenFileDialog` ตรงๆ — ใช้ helper ของ `AppHubForm` เพื่อให้ test ดักได้
- **Dialog:** form ใน MainForm เปิด dialog ด้วย `ShowDialogChild` (MDI) เสมอ — ห้ามใช้ `ShowModal` ยกเว้นหน้าที่ยังไม่มี MainForm
- **Test:** แก้ behavior แล้วต้องเพิ่ม/แก้ test ใน `AppHub.Tests` และรันให้ผ่านทั้งหมด
- **Password:** SHA-256 hash เสมอ ห้าม store plain text — ใช้ `AccountRules.HashPassword()` ห้ามเขียน SHA-256 เอง
- **Password policy:** รหัสใหม่ทุกที่ (สมัคร, Admin เพิ่ม/รีเซ็ต, เปลี่ยนเอง) ต้องผ่าน `AccountRules.ValidatePassword()` — ≥ 8 ตัว, a-z, A-Z, อักขระพิเศษ
- **Username:** ผ่าน `AccountRules.ValidateUsername()` — 3-50 ตัว, a-z A-Z 0-9 _ .

---

## NDA / Security Constraint

> ⚠️ **สำคัญมาก**: AppHub ได้รับแรงบันดาลใจจาก architecture ของ CSP-Card_Production
> แต่ **ห้ามใช้ code, ข้อมูล, หรือโครงสร้างใดๆ จาก CSP-Card_Production** โดยเด็ดขาด
> เนื่องจากเป็นทรัพย์สินของบริษัท
>
> Code ทุกบรรทัดใน AppHub ต้องเขียนขึ้นใหม่ทั้งหมด

---

## Common Tasks for Claude Code

### เพิ่ม Module ใหม่

1. สร้าง project ใหม่ใน `AppHub.NewModule\` — form ไว้ใน `Forms\`, dialog ไว้ใน `Dialogs\`
2. Reference `AppHub.Core`
3. สร้าง Form ที่ inherit `AppHubForm` หรือ `AppHubCRUDForm` (title label ชื่อ `lblTitle`, ไอคอนปุ่มด้วย `Theme.SetIcon`)
4. เพิ่ม Module_code ใหม่ใน `dbo.t_Modules` (seed ใน `setup.sql`) — หน้ากำหนดสิทธิ์จะแสดงเอง
5. ใน `MainForm`: เพิ่มปุ่ม `navXxx` (Tag `nav`) ใน sidebar + menu item, ผูกไอคอน, ใส่ใน `_navByForm`,
   และตรวจ `AppSession.HasPermission("NEWMODULE")` ใน `ApplyPermissions`

### แก้ไข SQL Query

- ใช้ parameterized query เสมอ (`@param`)
- ห้าม string concatenation ใน SQL
- ห้ามลืม `finally { SQL.Disconnect(); }`

### เพิ่ม Column ใน Grid

```csharp
// หลัง DataSource = dt;
if (dgvXxx.Columns["ColName"] != null)
    dgvXxx.Columns["ColName"].Visible = false;
```
