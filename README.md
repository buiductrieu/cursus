
# CURSUS NET ONE

Many people nowadays do not have time to go to live courses due to various reasons. Therefore, this project is proposed to solve the learning problem of users as well as create more jobs for instructors.

This is a course management project where students can register for their desired courses and study online. Upon completion of the course, the system will award certificates to the students. Instructors can upload their courses and get paid based on the number of students enrolling.



## Authors

- [@NET_13_DaoTrongDuc](https://git.fa.edu.vn/NET_13_DaoTrongDuc)
- [@NET_13_NguyenBaMinhDuc](https://git.fa.edu.vn/NET_13_NguyenBaMinhDuc)
- [@NET_13_PhamAnhKiet](https://git.fa.edu.vn/NET_13_PhamAnhKiet)
- [@NET_13_NguyenMinhThanh](https://git.fa.edu.vn/tnqt375)
- [@NET_13_LeXuanPhuongNam](https://git.fa.edu.vn/NET_13_LeXuanPhuongNam)
- [@NET13_NguyenQuangSon](https://git.fa.edu.vn/NET13_NguyenQuangSon)
- [@NET_13_NguyenHuuQuocHung](https://git.fa.edu.vn/NET_13_NguyenHuuQuocHung)
- [@NET_13_BUIDUCTRIEU](https://git.fa.edu.vn/NET_13_BUIDUCTRIEU)
- [@NET_13_VoLeDucAnh](https://git.fa.edu.vn/NET_13_VoLeDucAnh)




## Demo

https://api.pak160404.click/index.html

## Tools and Technologies
- Frontend: Swagger UI  
- Backend: C#, AutoMapper, Code First  
- Web Server: Azure  
- Database: MSSQL Server


## The system has three modules. 
- User
- Instructor
- Admin

Users, mostly students, can view courses on the system, they have the right to register and buy those courses. During the learning process, they can add their favorite courses to bookmarks. If they like the courses, they have the right to rate and give feedback to the instructor so they can do better. After completing the courses well, they will receive a certificate as recognition of their excellent completion of the course.

Instructors, who are the ones who put their courses on the system. They will get profits based on the number of students who enroll in their courses. They can discuss with students based on the Chat feature. Their rating will be based on the number of reviews on the courses.

Admin, who manages this system, will have access to the highest authority of the system. They will be responsible for managing the system, issuing discount codes, and responsible for the system's income and expenditure.

## How to run
1. Download the project zip file
2. Extract the file and copy the folder
3. Open CusurNetOne
4. Create a database
5. Import demo.sql file (inside database folder)
6. Run the script 

# .NET 8 Project Setup Guide

This document provides instructions on how to set up and run a .NET 8 project, including the installation of .NET SDK, creating a new project, and running it on your local machine.

## System Requirements

To develop and run .NET 8 applications, make sure your system meets the following requirements:

### Requirements:
- **.NET 8 SDK**: You need the latest .NET SDK version. This guide assumes you are using .NET 8.
- **Visual Studio** or **Visual Studio Code**:
  - **Visual Studio**: For Windows, Visual Studio is highly recommended for .NET development.
  - **Visual Studio Code**: A lightweight editor available on all platforms (Windows, macOS, Linux).
- **Operating System**: .NET 8 works on Windows, macOS, and Linux.

## Installation

### I. Install .NET 8 SDK

1. **Download .NET SDK**:
   - Visit the [official .NET 8 download page](https://dotnet.microsoft.com/download/dotnet) and select the **.NET 8 SDK**.
   - Run the installer and follow the instructions to complete the installation.

2. **Verify Installation**:
   Open a command prompt (cmd) and run:
   ```bash
   dotnet --version
### II. Create Database

### 1. Add Migration 
    Add migrations "Name_of_Migration"

### 2.Update Database
    Drop-database(If you already have database)
    Update-Database

# User Roles and Functionality Table

| Function                                       | Guest | Student | Instructor | Admin |
|------------------------------------------------|-------|---------|------------|-------|
| **Authentication**                             |       |         |            |       |
| Register                                       | O     | O       | O          | X     |
| Login/Logout                                   | O     | O       | O          | O     |
| **HomePage/DashBoard**                         |       |         |            |       |
| View homepage as guest                         | O     | X       | X          | X     |
| View homepage as student                       | X     | O       | X          | X     |
| View Dashboard                                 | X     | O       | O          | O     |
| View Course Enrolled                           | X     | O       | X          | X     |
| View Total Of Student/Course                   | X     | X       | O          | X     |
| **Setting**                                    |       |         |            |       |
| View/Edit Profile                              | X     | O       | O          | X     |
| Change Password                                | X     | O       | O          | X     |
| Purchase courses                               | X     | O       | X          | X     |
| View/Add/Edit/Delete Cart                      | X     | O       | X          | X     |
| Proceed purchase course                        | O     | X       | X          | X     |
| **Enrol courses**                              |       |         |            |       |
| Enrol into a course                            | X     | O       | X          | X     |
| View tracking of the course                    | X     | O       | X          | X     |
| View Step/Step Content                         | X     | O       | X          | X     |
| Review course                                  | X     | O       | X          | X     |
| Report course                                  | X     | O       | X          | X     |
| Comment on course                              | X     | O       | X          | X     |
| Get a certificate after completing the course  | X     | O       | X          | X     |
| **Dashboard for Instructor**                   |       |         |            |       |
| View Course/ Student Analytics                 | X     | X       | O          | X     |
| View Wallet                                    | X     | X       | O          | X     |
| Request Payout From Wallet                     | X     | X       | O          | X     |
| **Manage Course**                              |       |         |            |       |
| View list course                               | X     | O       | O          | X     |
| View list step                                 | X     | O       | O          | X     |
| View list step content                         | X     | O       | O          | X     |
| Add/Update/Delete Step                         | X     | X       | O          | X     |
| Add/Update/Delete StepContent                  | X     | X       | O          | X     |
| Submit course for approval                     | X     | X       | O          | X     |
| Create Certification about Course              | X     | X       | O          | X     |
| **Money Earned**                               |       |         |            |       |
| View Earning Money                             | X     | X       | O          | X     |
| Request Payout                                 | X     | X       | O          | X     |
| **Dashboard for Admin**                        |       |         |            |       |
| View Dashboard                                 | X     | X       | X          | O     |
| View list of Instructor                        | X     | X       | X          | O     |
| View list of User                              | X     | X       | X          | O     |
| View list of Course                            | X     | X       | X          | O     |
| View Wallet of Instructor                      | X     | X       | X          | O     |
| **Manage User**                                |       |         |            |       |
| Approved/Reject Instructor register            | X     | X       | X          | O     |
| Approved/Reject Request Payout Instructor      | X     | X       | X          | O     |
| Approved/Reject Course                         | X     | X       | X          | O     |
| Block/Unblock Instructor                       | X     | X       | X          | O     |

**Legend:**
- **O**: Available
- **X**: Not Available
