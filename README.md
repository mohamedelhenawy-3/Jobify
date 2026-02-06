# Jobify 

### Modern Full-Stack Job Portal Solution

**Built with:** .NET 8 (Web API) | Angular 17 | SQL Server | Pure CSS

---

## 📖 Project Overview

**Jobify** is a comprehensive recruitment platform designed to bridge the gap between employers and job seekers. The project focuses on a decoupled architecture, high-performance data handling, and a modern reactive UI.

This project was built to demonstrate proficiency in:

* **Tiered Backend Architecture** (Clean Architecture).
* **Reactive Frontend State Management** (Angular Signals).
* **Relational Database Design** (Normalized SQL Server Schema).

---

## 🛠 Technical Architecture

### Backend: .NET 8 Web API

The backend is structured using **Clean Architecture** principles to ensure the code is testable and maintainable.

* **Domain Layer:** Contains core entities (Users, Jobs, Applications) and business logic.
* **Application Layer:** Implements the **CQRS Pattern** using MediatR for clean command/query separation.
* **Infrastructure Layer:** Handles SQL Server communication via **Entity Framework Core 8** and Repository Patterns.
* **API Layer:** RESTful endpoints secured with **JWT Authentication**.

### Frontend: Angular 17

A modern, standalone-component-based architecture.

* **Reactivity:** Uses **Angular Signals** for lightning-fast UI updates and efficient change detection.
* **Routing:** Implements **Lazy Loading** for all feature modules to minimize initial bundle size.
* **Styling:** 100% **Custom CSS**. No external UI libraries were used, showcasing deep knowledge of Flexbox, CSS Grid, and responsive design.

---

## 📊 Database Design

The database is fully normalized (3NF) based on enterprise standards for job portals.

* **Separated Profiles:** Uses a 1:1 relationship between `Users` and `Profiles` (JobSeeker/Employer) to optimize authentication speed.
* **Lookup Tables:** Global tables for `IndustryType`, `EmploymentType`, and `JobStatus` to ensure data consistency.
* **Audit Logging:** Every table includes automated timestamping for `CreatedAt` and `UpdatedAt`.

---

## ✨ Key Features

* **Dynamic Search:** Filter jobs by title, location, or industry in real-time.
* **Applicant Tracking:** Employers can review applications and leave internal private notes.
* **Resume Management:** Job seekers can upload and update their professional links and resumes.
* **Security:** Password hashing using BCrypt and secure token-based authorization.

---

## ⚙️ How to Run

### 1. Prerequisites

* .NET 8 SDK
* Node.js (v18+)
* SQL Server Management Studio (SSMS)

### 2. Backend Setup

```bash
cd Jobify.Api
# Update ConnectionString in appsettings.json
dotnet ef database update
dotnet run

```

### 3. Frontend Setup

```bash
cd Jobify.Client
npm install
ng serve -o

```

---

##  Author

**Your Name**

* **LinkedIn:** [https://www.linkedin.com/in/mohamed-el-henawy-759888209/]
* **Email:** [mohamed.henawii11@gmail.com]


