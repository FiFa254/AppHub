# AppHub — Test Plan & Test Report

**เวอร์ชัน:** 1.0  
**วันที่:** 2026-09-20  
**ผู้จัดทำ:** ตรีวิทย์ บุญหนุน  
**ระบบ:** AppHub (Multi-Module WinForms Application)

---

## 1. ขอบเขตการทดสอบ (Scope)

| โมดูล | รายละเอียด |
|---|---|
| **Login / Auth** | เข้าสู่ระบบ, ตรวจสอบรหัสผ่าน, โหลด permission |
| **User Management** | สร้าง/แก้ไข/ปิดใช้งาน user, กำหนดสิทธิ์โมดูล |
| **CRUD** | จัดการข้อมูล Customer (เพิ่ม / แก้ / ลบ / ค้นหา) |
| **Import Excel** | นำเข้าข้อมูล Customer จากไฟล์ .xlsx |
| **Report** | กรองและแสดงรายงาน Customer |
| **Scan / Input** | บันทึกข้อมูล ScanLog ทีละรายการ |

---

## 2. Workflow Steps (ขั้นตอนการใช้งานระบบ)

```
[ผู้ใช้เปิดโปรแกรม]
        │
        ▼
[LoginForm]  ──── ผิด ──── MessageBox "Username หรือ Password ไม่ถูกต้อง"
        │ ถูก
        ▼
[โหลด AppSession + Permissions จาก DB]
        │
        ▼
[MainForm MDI]
        │
        ├─── Admin ──────► [User Management Form]
        │                        ├── เพิ่ม User ใหม่
        │                        ├── กำหนดสิทธิ์ (checkbox โมดูล)
        │                        └── เปิด/ปิดใช้งาน User
        │
        ├─── มีสิทธิ์ CRUD  ──► [Customer CRUD Form]
        │                        ├── ค้นหา / กรอง
        │                        ├── เพิ่ม Customer
        │                        ├── แก้ไข Customer
        │                        └── ลบ Customer (ยืนยันก่อน)
        │
        ├─── มีสิทธิ์ IMPORT ► [Import Form]
        │                        ├── เลือกไฟล์ .xlsx
        │                        ├── Preview ข้อมูลก่อน import
        │                        └── กด Import → บันทึกลง DB
        │
        ├─── มีสิทธิ์ REPORT ► [Report Form]
        │                        ├── กรองตามวันที่ / ชื่อ
        │                        ├── แสดงผลใน Grid
        │                        └── Export CSV
        │
        └─── มีสิทธิ์ SCAN  ──► [Scan Form]
                                 ├── กรอก Customer Code
                                 ├── กรอก Note
                                 └── บันทึก ScanLog
```

---

## 3. Test Cases

### 3.1 TC-LOGIN — Login / Authentication

| TC# | ชื่อ Test Case | Input | Expected Result | Priority |
|---|---|---|---|---|
| TC-L-01 | Login สำเร็จ (Admin) | user: `admin` / pw: `admin1234` | เข้า MainForm, เห็นปุ่มทุกโมดูล + User Management | High |
| TC-L-02 | Login สำเร็จ (User จำกัดสิทธิ์) | user: `user1` / pw: `user1234` | เข้า MainForm, เห็นเฉพาะ CRUD + REPORT (ปุ่มอื่น disabled) | High |
| TC-L-03 | Password ผิด | user: `admin` / pw: `wrongpass` | MessageBox "Username หรือ Password ไม่ถูกต้อง" | High |
| TC-L-04 | Username ไม่มีในระบบ | user: `ghost` / pw: `anything` | MessageBox error เดิม | High |
| TC-L-05 | กด Login โดยไม่กรอก | username/password ว่าง | MessageBox "กรุณากรอก..." | Medium |
| TC-L-06 | กด Logout | เข้าระบบแล้วกด Logout | กลับ LoginForm, AppSession ถูก Clear | High |
| TC-L-07 | User ที่ Is_active = 0 | user ที่ถูกปิดใช้งาน | Login ไม่ผ่าน | Medium |

---

### 3.2 TC-USRMGMT — User Management (Admin only)

| TC# | ชื่อ Test Case | Input | Expected Result | Priority |
|---|---|---|---|---|
| TC-U-01 | เปิดหน้า User Management | Login เป็น Admin | โหลดรายการ Users ทั้งหมดใน Grid | High |
| TC-U-02 | เพิ่ม User ใหม่ | username: `user2`, pw: `test1234`, สิทธิ์: CRUD, IMPORT | User ปรากฏใน Grid | High |
| TC-U-03 | เพิ่ม User ซ้ำ username | username ซ้ำกับที่มีอยู่ | DB error — แสดง MessageBox แจ้งเตือน | High |
| TC-U-04 | เพิ่ม User โดยไม่กรอก Username | Username ว่าง | MessageBox "กรุณากรอก Username และ Password" | Medium |
| TC-U-05 | กำหนดสิทธิ์ | เลือก user1 → กด "กำหนดสิทธิ์" → เพิ่ม SCAN | user1 login แล้วเห็นปุ่ม SCAN เพิ่ม | High |
| TC-U-06 | ถอนสิทธิ์ | เลือก user1 → ถอน CRUD | user1 login แล้วปุ่ม CRUD disabled | High |
| TC-U-07 | ปิดใช้งาน User | เลือก user → กด "เปิด/ปิด" → ยืนยัน Yes | Is_active = 0 ใน DB, user login ไม่ได้ | High |
| TC-U-08 | ยกเลิกปิดใช้งาน | กด "เปิด/ปิด" → ยืนยัน No | ไม่มีการเปลี่ยนแปลง | Low |
| TC-U-09 | User ปกติเข้า User Management | Login เป็น user1 | ปุ่ม "User Management" ไม่ปรากฏในเมนู | High |

---

### 3.3 TC-CRUD — Customer Management

| TC# | ชื่อ Test Case | Input | Expected Result | Priority |
|---|---|---|---|---|
| TC-C-01 | เปิดหน้า CRUD | กด "จัดการข้อมูล" | Grid แสดง Customer ทั้งหมดจาก DB | High |
| TC-C-02 | เพิ่ม Customer ใหม่ | code: `C099`, name: `ทดสอบ`, phone: `080-000` | แถวใหม่ปรากฏใน Grid | High |
| TC-C-03 | เพิ่ม Customer code ซ้ำ | code: `C001` (ซ้ำ) | MessageBox แจ้ง duplicate | High |
| TC-C-04 | เพิ่มโดยไม่กรอก Required field | Customer_code หรือ Full_name ว่าง | Validate error ก่อน save | High |
| TC-C-05 | แก้ไข Customer | เลือกแถว → แก้ phone → บันทึก | Updated_date อัปเดต, ข้อมูลเปลี่ยน | High |
| TC-C-06 | ลบ Customer | เลือกแถว → ลบ → ยืนยัน Yes | แถวหายออกจาก Grid และ DB | High |
| TC-C-07 | ยกเลิกลบ | กด ลบ → ยืนยัน No | ไม่มีการเปลี่ยนแปลง | Medium |
| TC-C-08 | ค้นหา/กรอง | พิมพ์ชื่อบางส่วน | Grid กรองเฉพาะแถวที่ตรง | Medium |

---

### 3.4 TC-IMPORT — Import Excel

| TC# | ชื่อ Test Case | Input | Expected Result | Priority |
|---|---|---|---|---|
| TC-I-01 | เลือกไฟล์ .xlsx ถูกต้อง | ไฟล์ Excel มี col: Customer_code, Full_name, Phone, Email, Address | Preview แสดงข้อมูลใน Grid | High |
| TC-I-02 | ไฟล์ที่ไม่ใช่ Excel | เลือก .txt หรือ .pdf | MessageBox "กรุณาเลือกไฟล์ .xlsx" | High |
| TC-I-03 | ไฟล์ Excel header ผิด | header column ไม่ตรง | MessageBox แจ้ง column ที่หาไม่พบ | Medium |
| TC-I-04 | Import สำเร็จ | กด Import หลัง preview | ข้อมูลเพิ่มลง t_Customers, แสดง "Import สำเร็จ X รายการ" | High |
| TC-I-05 | Import มี code ซ้ำ | บางแถวมี Customer_code ซ้ำกับใน DB | ข้ามแถวซ้ำ, import เฉพาะแถวใหม่, แจ้งจำนวน skipped | High |
| TC-I-06 | ไฟล์ว่าง / ไม่มีข้อมูล | Excel มีแค่ header | MessageBox "ไม่พบข้อมูลในไฟล์" | Medium |
| TC-I-07 | Import ไฟล์ใหญ่ | > 100 แถว | Import ครบ ไม่ crash | Low |

---

### 3.5 TC-REPORT — Report

| TC# | ชื่อ Test Case | Input | Expected Result | Priority |
|---|---|---|---|---|
| TC-R-01 | เปิดหน้า Report | กด "รายงาน" | Grid แสดง Customer ทั้งหมด | High |
| TC-R-02 | กรองตามชื่อ | พิมพ์ "สมชาย" | Grid แสดงเฉพาะ Customer ที่ชื่อ match | High |
| TC-R-03 | กรองตามวันที่ From | วันที่เริ่ม | แสดงเฉพาะ Created_date >= วันที่ที่เลือก | High |
| TC-R-04 | กรองตามวันที่ To | วันที่สิ้นสุด | แสดงเฉพาะ Created_date <= วันที่ที่เลือก | High |
| TC-R-05 | กรองไม่พบข้อมูล | ชื่อที่ไม่มีใน DB | Grid ว่าง, MessageBox "ไม่พบข้อมูล" | Medium |
| TC-R-06 | Export CSV | กด "Export CSV" | บันทึกไฟล์ .csv ที่เลือก path, เปิดได้ด้วย Excel | Medium |
| TC-R-07 | Clear filter | กด "ล้าง" หลังกรอง | Grid แสดงทั้งหมดอีกครั้ง | Low |

---

### 3.6 TC-SCAN — Scan / Input

| TC# | ชื่อ Test Case | Input | Expected Result | Priority |
|---|---|---|---|---|
| TC-S-01 | บันทึก ScanLog สำเร็จ | code: `C001`, note: `รับของแล้ว` | แถวใหม่ใน t_ScanLog, MessageBox "บันทึกสำเร็จ" | High |
| TC-S-02 | บันทึกโดยไม่กรอก Code | Customer_code ว่าง | MessageBox "กรุณากรอก Customer Code" | High |
| TC-S-03 | บันทึกซ้ำ (same code) | scan code เดิมอีกครั้ง | บันทึกได้ (ScanLog ไม่ unique constraint) | Medium |
| TC-S-04 | กด Clear | กรอกข้อมูลแล้วกด Clear | ล้างทุก field กลับ empty | Low |
| TC-S-05 | กด Enter บน field | กด Enter ใน txtCode หรือ txtNote | trigger บันทึก (AcceptButton) | Low |

---

## 4. แผนการทดสอบ SIT / UAT

### 4.1 SIT — System Integration Testing

**วัตถุประสงค์:** ทดสอบว่าทุก module ทำงานร่วมกันได้ถูกต้องตาม spec

**ผู้ทดสอบ:** Developer (ตรีวิทย์)  
**Environment:** เครื่อง Developer + LocalDB  
**ระยะเวลา:** 1 วัน

| Phase | กลุ่ม Test Case | เป้าหมาย |
|---|---|---|
| SIT-1 | TC-L-01 ถึง TC-L-07 | Login flow ทุก path |
| SIT-2 | TC-U-01 ถึง TC-U-09 | User Management + Permission |
| SIT-3 | TC-C-01 ถึง TC-C-08 | CRUD flow ทั้งหมด |
| SIT-4 | TC-I-01 ถึง TC-I-07 | Import Excel flow |
| SIT-5 | TC-R-01 ถึง TC-R-07 | Report + Export |
| SIT-6 | TC-S-01 ถึง TC-S-05 | Scan / Input |

**เกณฑ์ผ่าน SIT:** Test case Priority High ผ่านทั้งหมด, Medium ≥ 80%

---

### 4.2 UAT — User Acceptance Testing

**วัตถุประสงค์:** ทดสอบว่า end user ใช้งานได้จริงตามที่คาดหวัง

**ผู้ทดสอบ:** User จริง (ผู้ใช้งานระบบ)  
**Environment:** Test environment (ไม่ใช่ Production)  
**ระยะเวลา:** 2 วัน

#### UAT Scenario 1 — Admin จัดการผู้ใช้

```
Pre-condition: Login เป็น admin / admin1234

Steps:
1. เปิดโปรแกรม → Login
2. กด "จัดการผู้ใช้" ในเมนู
3. กด "เพิ่มผู้ใช้" → กรอก username: uat_user1, password: uat1234, ชื่อ: UAT User One
4. ติ๊ก checkbox CRUD และ REPORT → บันทึก
5. Logout
6. Login ด้วย uat_user1 / uat1234
7. ตรวจว่าเห็นปุ่ม CRUD, REPORT เท่านั้น

Expected: ผ่านทุกขั้นตอน ปุ่มโมดูลอื่น disabled
```

#### UAT Scenario 2 — User นำเข้าและดูรายงาน

```
Pre-condition: Login เป็น uat_user1 (มีสิทธิ์ IMPORT + REPORT)

Steps:
1. กด "Import Excel" → เลือกไฟล์ test_customers.xlsx
2. ตรวจ Preview ว่าข้อมูลถูกต้อง
3. กด Import → ตรวจ MessageBox แจ้งผล
4. ปิด Import Form
5. กด "รายงาน" → ตรวจว่าข้อมูลที่ import ปรากฏ
6. กรองตามชื่อ → ตรวจผล

Expected: Import สำเร็จ, Report แสดงข้อมูลใหม่
```

#### UAT Scenario 3 — User จัดการข้อมูล

```
Pre-condition: Login เป็น uat_user1 (มีสิทธิ์ CRUD)

Steps:
1. กด "จัดการข้อมูล"
2. เพิ่ม Customer ใหม่: code UAT001, ชื่อ "ผู้ทดสอบ UAT"
3. แก้ไข phone → บันทึก
4. ค้นหา "ผู้ทดสอบ" → ตรวจว่าพบ
5. ลบ Customer UAT001 → ยืนยัน

Expected: ทุกขั้นตอนทำงานถูกต้อง
```

**เกณฑ์ผ่าน UAT:** Scenario ผ่านทั้ง 3 ข้อ โดยไม่มี critical bug

---

## 5. ไฟล์ทดสอบที่ต้องเตรียม

| ไฟล์ | รายละเอียด | ใช้ใน |
|---|---|---|
| `test_customers.xlsx` | ข้อมูล Customer ตัวอย่าง 10 แถว (column: Customer_code, Full_name, Phone, Email, Address) | TC-I-01, TC-I-04, UAT-2 |
| `test_customers_dup.xlsx` | ข้อมูลที่มี code ซ้ำกับ DB 3 แถว | TC-I-05 |
| `test_empty.xlsx` | ไฟล์ Excel มีแค่ header row | TC-I-06 |
| `test_wrong_header.xlsx` | header ผิด column name | TC-I-03 |

---

## 6. รายงานผลการทดสอบ (Test Report)

**วันที่ทดสอบ:** ___________  
**ผู้ทดสอบ:** ___________  
**Version โปรแกรม:** 1.0  
**Environment:** LocalDB / SQLExpress

---

### 6.1 สรุปผล SIT

| Phase | จำนวน TC | ผ่าน | ไม่ผ่าน | Skip | % ผ่าน |
|---|---|---|---|---|---|
| SIT-1 (Login) | 7 | | | | |
| SIT-2 (User Mgmt) | 9 | | | | |
| SIT-3 (CRUD) | 8 | | | | |
| SIT-4 (Import) | 7 | | | | |
| SIT-5 (Report) | 7 | | | | |
| SIT-6 (Scan) | 5 | | | | |
| **รวม** | **43** | | | | |

---

### 6.2 รายละเอียดผลการทดสอบ

| TC# | ชื่อ | ผล | หมายเหตุ / Bug Description |
|---|---|---|---|
| TC-L-01 | Login Admin | ☐ Pass ☐ Fail | |
| TC-L-02 | Login User จำกัดสิทธิ์ | ☐ Pass ☐ Fail | |
| TC-L-03 | Password ผิด | ☐ Pass ☐ Fail | |
| TC-L-04 | Username ไม่มีในระบบ | ☐ Pass ☐ Fail | |
| TC-L-05 | Login ไม่กรอก | ☐ Pass ☐ Fail | |
| TC-L-06 | Logout | ☐ Pass ☐ Fail | |
| TC-L-07 | User ปิดใช้งาน | ☐ Pass ☐ Fail | |
| TC-U-01 | เปิด User Mgmt | ☐ Pass ☐ Fail | |
| TC-U-02 | เพิ่ม User | ☐ Pass ☐ Fail | |
| TC-U-03 | Username ซ้ำ | ☐ Pass ☐ Fail | |
| TC-U-04 | Username ว่าง | ☐ Pass ☐ Fail | |
| TC-U-05 | กำหนดสิทธิ์ | ☐ Pass ☐ Fail | |
| TC-U-06 | ถอนสิทธิ์ | ☐ Pass ☐ Fail | |
| TC-U-07 | ปิดใช้งาน User | ☐ Pass ☐ Fail | |
| TC-U-08 | ยกเลิกปิดใช้งาน | ☐ Pass ☐ Fail | |
| TC-U-09 | User ปกติเข้า User Mgmt | ☐ Pass ☐ Fail | |
| TC-C-01 | เปิด CRUD | ☐ Pass ☐ Fail | |
| TC-C-02 | เพิ่ม Customer | ☐ Pass ☐ Fail | |
| TC-C-03 | Code ซ้ำ | ☐ Pass ☐ Fail | |
| TC-C-04 | Required field ว่าง | ☐ Pass ☐ Fail | |
| TC-C-05 | แก้ไข Customer | ☐ Pass ☐ Fail | |
| TC-C-06 | ลบ Customer | ☐ Pass ☐ Fail | |
| TC-C-07 | ยกเลิกลบ | ☐ Pass ☐ Fail | |
| TC-C-08 | ค้นหา | ☐ Pass ☐ Fail | |
| TC-I-01 | เลือกไฟล์ถูกต้อง | ☐ Pass ☐ Fail | |
| TC-I-02 | ไฟล์ไม่ใช่ Excel | ☐ Pass ☐ Fail | |
| TC-I-03 | Header ผิด | ☐ Pass ☐ Fail | |
| TC-I-04 | Import สำเร็จ | ☐ Pass ☐ Fail | |
| TC-I-05 | Code ซ้ำ | ☐ Pass ☐ Fail | |
| TC-I-06 | ไฟล์ว่าง | ☐ Pass ☐ Fail | |
| TC-I-07 | ไฟล์ใหญ่ | ☐ Pass ☐ Fail | |
| TC-R-01 | เปิด Report | ☐ Pass ☐ Fail | |
| TC-R-02 | กรองชื่อ | ☐ Pass ☐ Fail | |
| TC-R-03 | กรอง From | ☐ Pass ☐ Fail | |
| TC-R-04 | กรอง To | ☐ Pass ☐ Fail | |
| TC-R-05 | ไม่พบข้อมูล | ☐ Pass ☐ Fail | |
| TC-R-06 | Export CSV | ☐ Pass ☐ Fail | |
| TC-R-07 | Clear filter | ☐ Pass ☐ Fail | |
| TC-S-01 | บันทึก Scan | ☐ Pass ☐ Fail | |
| TC-S-02 | Code ว่าง | ☐ Pass ☐ Fail | |
| TC-S-03 | Scan ซ้ำ | ☐ Pass ☐ Fail | |
| TC-S-04 | กด Clear | ☐ Pass ☐ Fail | |
| TC-S-05 | กด Enter | ☐ Pass ☐ Fail | |

---

### 6.3 สรุปผล UAT

| Scenario | ผล | ผู้ทดสอบ | ลายเซ็น |
|---|---|---|---|
| UAT-1: Admin จัดการผู้ใช้ | ☐ Pass ☐ Fail | | |
| UAT-2: Import + Report | ☐ Pass ☐ Fail | | |
| UAT-3: CRUD | ☐ Pass ☐ Fail | | |

---

### 6.4 Bug Log

| Bug# | TC# | ความรุนแรง | อาการ | สถานะ |
|---|---|---|---|---|
| BUG-001 | | ☐ Critical ☐ Major ☐ Minor | | ☐ Open ☐ Fixed |
| BUG-002 | | ☐ Critical ☐ Major ☐ Minor | | ☐ Open ☐ Fixed |
| BUG-003 | | ☐ Critical ☐ Major ☐ Minor | | ☐ Open ☐ Fixed |

---

### 6.5 สรุปและข้อสรุปโดยรวม

**ผล SIT:** ☐ ผ่าน ☐ ไม่ผ่าน  
**ผล UAT:** ☐ ผ่าน ☐ ไม่ผ่าน  

**สรุปปัญหาที่พบ:**

```
(กรอกที่นี่)
```

**ข้อเสนอแนะ:**

```
(กรอกที่นี่)
```

**มติ:** ☐ อนุมัติให้ใช้งาน ☐ แก้ไขก่อน ☐ ทดสอบซ้ำ

---

*เอกสารนี้เป็นส่วนหนึ่งของโปรเจกต์ AppHub — Demo Multi-Module WinForms Application*
