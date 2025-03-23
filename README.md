# Cumulative1_n01649272

## 📌 Project Overview
This repository contains structured data for courses and instructors, representing an academic system. The dataset includes details about courses, their instructors, and relevant timelines. The purpose of this repository is to organize and present the information in a structured format for analysis and development.

## 📂 Repository Information
- **GitHub Repository:** [Cumulative1_n01649272](https://github.com/Oyemahak/Cumulative1_n01649272)  
- **Author:** Mahak Patel  
- **Database Structure:** Includes course details and teacher information  

---

## 📚 Course Data
The following table provides details about the available courses, including their duration and assigned instructors.

| Course ID | Course Code | Teacher ID | Start Date  | End Date    | Course Name                         |
|-----------|------------|------------|------------|------------|-------------------------------------|
| 1         | HTTP5101   | 1          | 2018-09-04 | 2018-12-14 | Web Application Development        |
| 2         | HTTP5102   | 2          | 2018-09-04 | 2018-12-14 | Project Management                 |
| 3         | HTTP5103   | 5          | 2018-09-04 | 2018-12-14 | Web Programming                    |
| 4         | HTTP5104   | 7          | 2018-09-04 | 2018-12-14 | Digital Design                     |
| 5         | HTTP5105   | 8          | 2018-09-04 | 2018-12-14 | Database Development               |
| 6         | HTTP5201   | 2          | 2019-01-08 | 2019-04-27 | Security & Quality Assurance       |
| 7         | HTTP5202   | 3          | 2019-01-08 | 2019-04-27 | Web Application Development 2      |
| 8         | HTTP5203   | 4          | 2019-01-08 | 2019-04-27 | XML and Web Services               |
| 9         | HTTP5204   | 5          | 2019-01-08 | 2019-04-27 | Mobile Development                 |
| 10        | HTTP5205   | 6          | 2019-01-08 | 2019-04-27 | Career Connections                 |
| 11        | HTTP5206   | 8          | 2019-01-08 | 2019-04-27 | Web Information Architecture       |
| 12        | PHYS2203   | 10         | 2019-09-04 | 2019-12-14 | Massage Therapy                    |

---

## 👨‍🏫 Teacher Data
The following table provides details about instructors, including their employee numbers, hire dates, and salaries.

| Teacher ID | First Name | Last Name  | Employee Number | Hire Date  | Salary  |
|------------|-----------|------------|---------------|------------|--------|
| 1          | Alexander | Bennett    | T378         | 2016-08-05 | 55.30  |
| 2          | Caitlin   | Cummings   | T381         | 2014-06-10 | 62.77  |
| 3          | Linda     | Chan       | T382         | 2015-08-22 | 60.22  |
| 4          | Lauren    | Smith      | T385         | 2014-06-22 | 74.20  |
| 5          | Jessica   | Morris     | T389         | 2012-06-04 | 48.62  |
| 6          | Thomas    | Hawkins    | T393         | 2016-08-10 | 54.45  |
| 7          | Shannon   | Barton     | T397         | 2013-08-04 | 64.70  |
| 8          | Dana      | Ford       | T401         | 2014-06-26 | 71.15  |
| 9          | Cody      | Holland    | T403         | 2016-06-13 | 43.20  |
| 10         | John      | Taram      | T505         | 2015-10-23 | 79.63  |

---

## 📂 Project Structure

## Backend (Web API)

| Component                   | Description                               |
|-----------------------------|-------------------------------------------|
| `SchoolDbContext.cs`         | Database context for MySQL connection.    |
| `TeacherAPIController.cs`    | API Controller for retrieving teacher data. |
| `Teacher.cs`                 | Model representing teacher details.       |

## Frontend (MVC Views)

| Component                   | Description                               |
|-----------------------------|-------------------------------------------|
| `TeacherPageController.cs`   | Handles dynamic page routing for teachers. |
| `List.cshtml`                | Displays a list of all teachers.          |
| `Show.cshtml`                | Displays details of a specific teacher.   |


## 💻 How to Use This Repository
1. **Clone the Repository**  
   ```bash
   git clone https://github.com/Oyemahak/Cumulative1_n01649272.git

## Author
**Mahak Patel** - Developed as part of **Christine Bittle’s C# Cumulative Project** at Humber College.
