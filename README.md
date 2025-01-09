# Employee Management System 🏢

A simple Employee Management System built with a clean architecture approach, providing efficient CRUD operations, searching, filtering, and pagination.

---

## 🚀 Tech Stack

### Backend

- **Language**: C#
- **Framework**: .NET 8, Minimal API
- **Database**: SQL Server
- **ORM**: EF Core 9
- **Key Features**:
  - Soft Deletion
  - Auditable Entities
  - Clean and Modular Code
  - Global Exception Handling Middleware
  - Column Indexing for Frequently Used Columns
  - Normalized Database with Proper Relations

### Frontend

- **Technologies**: HTML, CSS, JavaScript, jQuery, Bootstrap
- **Library**: DataTables.js for interactive and paginated data display.
- **Features**:
  - Efficient Searching and Filtering
  - Seamless Integration with Backend

---

## ⚙️ Features

- **CRUD Operations**: Manage employee records with ease.
- **Search & Filter**: Find employees with advanced filtering options.
- **Pagination**: Navigate large datasets efficiently using DataTables.
- **Error Handling**: Robust global exception handling for API stability.
- **Performance**: Optimized data loading with database indexing and clean code.
- **Scalability**: Built with a modular and clean architecture, making it easy to scale and maintain.

---

## 🛠️ Project Setup

Follow these steps to set up the project locally:

### Backend Setup

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/FarhanRafi/EmployeeManagementSystem.git
   ```

2. **Build the Solution**:
   Open the project in Visual Studio or any .NET-supported IDE.
   Build the solution to check for missing dependencies.
   If dependencies are missing, restore them as per the **`.csproj`** file.
   Configuration:

Add a development **`appsettings.json`** file in the API project directory.
Define the connection string as:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Connection-String-Here"
  }
}
```

3. **Set Startup Project**:
   Set the **`EMS.API`** project as the startup project in Visual Studio.
   Handle CORS:

If your frontend runs on a port other than `http://127.0.0.1:5500/`, update the CORS configuration in **`Program.cs`**:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

3. **Run the Backend**:
   Start the API project. The backend is now ready! ✅

## Frontend Setup:

1. **Navigate to the Frontend Folder**:
   Open the folder containing the static frontend files where _`index.html`_ file is located.

2. **Install Dependencies**:
   Run the following command to install required packages:

```bash
npm install
```

3. **Serve the Frontend**:

Open _`index.html`_ in your browser or serve it using a simple _`HTTP server`_.

## Database Scripts

Database creation, indexing, and check constraint scripts are located in the sql-script folder.

- DB Creation Script: This script sets up the database structure.
- Indexing Script: This script adds indexes to improve query performance.
- Check Constraints Script: This script ensures data integrity by adding check constraints.

You can execute these scripts against your database to set up and maintain the required schema.

## 🎯 Notes for viewers

This project demonstrates:

- Adherence to clean architecture principles.
- Effective use of minimal APIs for simplicity and performance.
- Implementation of best practices like column indexing, data - normalization, and error handling.
- Efficient searching and filtering in a paginated manner using DataTables.js.
- Built with scalability and maintainability in mind.
