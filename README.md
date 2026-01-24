# XYZ University Management System

A web application for managing university academic operations including students, instructors, courses, departments, and enrollments.

## Features

- **Role-Based Access Control**: Three user roles (Admin, Instructor, Student) with different permissions
- **Admin Dashboard**: Manage users, assign roles, and oversee all university data
- **Instructor Portal**: View assigned courses, grade students, and manage enrollments
- **Student Portal**: View enrolled courses, check GPA, and manage student card
- **Department & Course Management**: Full CRUD operations for academic structure
- **Student Card System**: Auto-generated student identification cards
- **API Integration**: Consumes external REST API for department/instructor data

## Tech Stack

- **Framework**: ASP.NET Core 10.0 MVC
- **Language**: C# 12
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views, Bootstrap 5, CSS3
- **API Communication**: JavaScript Fetch API

## Database Relationships

- **Inheritance**: Person (abstract) → Student, Instructor
- **One-to-Many**: Department → Instructors, Department → Courses, Instructor → Courses
- **Many-to-Many**: Student ↔ Course (via Enrollment bridge table with Mark property)
- **One-to-One**: Student ↔ StudentCard (shared primary key pattern)

## Default Credentials

- **Admin**: admin@xyz.edu.jo / Admin@123

## Running the Application

```bash
dotnet run
```

## Related Projects

- [UniAPI](https://github.com/HalaHabboub/UniAPI) - REST API providing department and instructor endpoints
