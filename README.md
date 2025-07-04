# ☁️ Cloud File Storage App

A Google Drive–inspired **cloud file manager** built with **ASP.NET Core MVC**, **Entity Framework**, and **SQL Server**. Upload, organize, preview, and share files — all with user authentication, dark mode, and modern UI.

---

## 🌟 Features

- 🔐 **User Authentication** (ASP.NET Identity)
- 📤 **File Upload & Drag-and-Drop**
- 📁 **Folders & Subfolder Organization**
- 📂 **Trash & Restore Deleted Files**
- 🌗 **Light/Dark Mode Toggle**
- 🔗 **Public/Shareable Links** (Token-based)
- ✏️ **Rename Files**
- 📊 **Recent Uploads View**
- 📸 **File Preview Modal**
- 🎨 Clean UI with [Inter](https://fonts.google.com/specimen/Inter) font and Font Awesome icons

---


## 🛠 Tech Stack

- **Frontend:** HTML5, Bootstrap 5, Razor Views, Font Awesome
- **Backend:** ASP.NET Core MVC (.NET 6+)
- **Database:** SQL Server with Entity Framework Core
- **Auth:** ASP.NET Identity
- **Cloud Features:** Shareable file links, drag & drop, dynamic folder routing

---

## 🚀 How to Run Locally

1. **Clone the Repo**
   ```bash
   git clone https://github.com/yourusername/cloud-file-storage-app.git
   cd cloud-file-storage-app 

2. **Setup the Database**

- Configure appsettings.json with your SQL Server connection string.
- Run migrations:

`bash
dotnet ef database update` 

3. **Run the App**

`bash
dotnet run`

4. Visit

`arduino
https://localhost:5001`

---

## 🧑‍💻 For Recruiters
This project demonstrates:

- ✅ Strong grasp of ASP.NET Core MVC and Identity
- ✅ UI/UX implementation (light/dark mode, drag-drop, modal previews)
- ✅ Full-stack skills with data storage and authentication
- ✅ Git/GitHub project structure and documentation

---

## 📁 Folder Structure

📦 wwwroot/uploads/         # Saved user files
📦 Views/File/              # Main UI (Razor pages)
📄 Controllers/FileController.cs
📄 Models/StoredFile.cs
📄 Data/ApplicationDbContext.cs

---

## 🙌 Acknowledgements

- Font Awesome
- Google Fonts – Inter
- Bootstrap 5

---

## 📬 Contact

📧 Shubham Awchare
📍 Manchester, UK
💼 https://www.linkedin.com/in/shubham-awchare/

Made with ❤️ using ASP.NET Core MVC and SQL Server



