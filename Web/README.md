# Laundromat Management System (LMS)

A comprehensive **enterprise-grade laundromat management web system** designed to support multi-location laundromat operations with full admin control, payment integration, IoT device management, and reporting capabilities.

---

## 🚀 Project Overview

The **Laundromat Management System (LMS)** is a full-stack solution built to help laundromat operators manage everything from user payments to remote device control and tax reporting.

Key highlights include:

- 💳 **Multi-Payment Processing**: Support for cash, mobile money, and card transactions
- 📡 **IoT Integration**: Connects to machines via RS485 for real-time status & control
- ☁️ **Remote Administration**: Cloud-based dashboard for pricing, promotions & reporting
- 📊 **Offline Support**: Local operations with automated sync to cloud
- 🌍 **Multi-Language Support**: Interface localized in English, Kinyarwanda, and French
- 🧾 **Tax Compliance**: RRA EBM real-time tax reporting (with offline fallback)
- 🔐 **Role-Based Access Control**: Fine-grained permissions for different user types

---

## 💼 Admin Privileges

Admin users have elevated access to manage and maintain critical platform functionality:

### 🔑 Authentication

Admins must authenticate using secure login credentials with role-based access.

### 🛠 Core Admin Features

**Global Dashboard**

- View high-level analytics and machine usage
- Monitor revenue in real time

**User & Role Management**

- Add, edit, or deactivate user accounts
- Assign roles (e.g., Admin, Staff, Technician)
- Control access to sensitive features

**Machine & IoT Controls**

- Register machine devices
- Restart/Stop specific IoT equipment remotely
- View machine status and health metrics

**Pricing & Services**

- Update service price lists
- Set promotional pricing
- Configure pricing per location

**Reporting**

- Generate tax-compliant reports
- Export CSV/PDF financial summaries
- Track usage and trends over time

**System Configuration**

- Set global settings via the cloud portal
- Manage integrations (payment, tax engines, monitoring)

---

## 🗂 Architecture & Tech Stack

LMS is architected with modular components for scalability and resilience:

| Component            | Technology           |
| -------------------- | -------------------- |
| POS Application      | MAUI (.NET 9+, MVVM) |
| API Server           | Web API              |
| Cloud Portal         | Typescript (Local)   |
| Database             | SQLite               |
| Device Communication | RS485 / Serial       |

> The system supports both **local deployment** and **remote VPN for accessing data**.

---

## 📘 Documentation & Resources

- 📄 **API Documentation** — In the `docs/` folder
- 📖 **User Manual** — `docs/UserManual.md`
- 📊 **Admin Guide** — `docs/AdminGuide.md`

---

## 📜 License (_later_)

This project is proprietary. See the `LICENSE` file for details.
