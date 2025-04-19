# Cumulative_n01649272

## 📌 Project Overview
This ASP.NET Core MVC/WebAPI project provides full CRUD functionality for Teachers and Students in a school database system. The application includes comprehensive error handling, validation, and initiative features across all three parts of the cumulative project.

## 📂 Repository Information
- **GitHub Repository:** [Cumulative_n01649272](https://github.com/Oyemahak/Cumulative_n01649272)  
- **Author:** Mahak Patel  
- **Database:** MySQL School Database with Teachers, Students, and Courses tables  

---

## 🚀 Project Features

### Part 1: Read Functionality (Completed)
#### Teacher Features
- List all teachers with search/filter capabilities
- View detailed teacher information
- Display courses taught by each teacher

#### Student Features
- List all students with search functionality
- View detailed student information
- Display enrolled courses

### Part 2: Add/Delete Functionality
#### Teacher Features
- Add new teachers with validation
- Delete teachers with confirmation
- Error handling for invalid inputs

#### Student Features
- Add new students with validation
- Delete students with confirmation
- Comprehensive error handling

### Part 3: Update Functionality (New)
#### Teacher Features
- Update teacher information
- Client and server-side validation
- Error handling for invalid updates

#### Student Features
- Update student records
- Full validation suite

---

## 📚 Database Structure

### Teachers Table
| Teacher ID | First Name | Last Name  | Employee Number | Hire Date  | Salary  |
|------------|-----------|------------|---------------|------------|--------|
| 1          | Alexander | Bennett    | T378         | 2016-08-05 | 55.30  |
| 2          | Caitlin   | Cummings   | T381         | 2014-06-10 | 62.77  |
| ...        | ...       | ...        | ...          | ...        | ...    |

### Students Table
| Student ID | First Name | Last Name | Student Number | Enrollment Date | Balance |
|------------|-----------|------------|---------------|-----------------|---------|
| 1          | John      | Smith      | S10001       | 2020-09-01      | 500.00  |
| 2          | Sarah     | Johnson    | S10002       | 2021-01-05      | 750.00  |
| ...        | ...       | ...        | ...          | ...             | ...     |

---

## 📂 Project Structure

### Shared Components
| File | Description |
|------|-------------|
| `SchoolDbContext.cs` | MySQL database context |
| `Shared/_Layout.cshtml` | Master layout page |

### Teacher Components
| Component | Type | Description |
|-----------|------|-------------|
| `TeacherAPIController.cs` | WebAPI | All CRUD operations |
| `TeacherPageController.cs` | MVC | Routing and views |
| `Teacher.cs` | Model | Teacher data structure |
| `List.cshtml` | View | Teacher listing |
| `Show.cshtml` | View | Teacher details |
| `New.cshtml` | View | Add teacher form |
| `Edit.cshtml` | View | Update teacher form |
| `DeleteConfirm.cshtml` | View | Delete confirmation |

### Student Components
| Component | Type | Description |
|-----------|------|-------------|
| `StudentAPIController.cs` | WebAPI | All CRUD operations |
| `StudentPageController.cs` | MVC | Routing and views |
| `Student.cs` | Model | Student data structure |
| `List.cshtml` | View | Student listing |
| `Show.cshtml` | View | Student details |
| `New.cshtml` | View | Add student form |
| `Edit.cshtml` | View | Update student form |
| `DeleteConfirm.cshtml` | View | Delete confirmation |

---

## 🛠️ Technical Implementation

### Key Features Added in Part 3
1. **Update Functionality**
   - Full teacher/student update capabilities
   - Comprehensive validation (client and server-side)
   - Error handling for invalid updates

2. **Enhanced API Endpoints**
   - `PUT /api/TeacherPage/UpdateTeacher/{id}`
   - `PUT /api/StudentPage/UpdateStudent/{id}`
   - Robust error responses

3. **User Interface Improvements**
   - Edit forms for teachers and students
   - Responsive design enhancements

4. **Initiative Features Completed**
   - Full error handling for all operations
   - Client-side validation with JavaScript
   - Cross-feature implementation (Teachers/Students/Courses)

---

## 💻 How to Use This Repository

1. **Clone the Repository**  
   ```bash
   git clone https://github.com/Oyemahak/Cumulative_n01649272.git
