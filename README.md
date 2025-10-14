# TaskFlow - A Trello-like Project Management Tool

TaskFlow is a modern, full-stack web application designed to help users manage their projects and tasks in a visual, Kanban-style workflow. It is built with a clean, decoupled architecture using an ASP.NET Core backend and a React frontend.

![TaskFlow Screenshot](https://github.com/user-attachments/assets/f553ec6e-c0a7-449c-b994-cd0b5d488d94)

---

## ✨ Features

The application currently supports a robust set of features for a complete project management experience.

### Core Functionality
- **Board Management**: Create new project boards.
- **List Management**: Add, rename (inline editing), and delete lists (e.g., "To Do", "In Progress", "Done") within a board.
- **Card Management**: Create, edit (title and description), delete, and reorder task cards within each list.
- **Drag-and-Drop**: Smoothly move cards between lists and reorder them within a list, with all changes saved persistently.

### Authentication & Authorization
- **User Accounts**: Secure user registration and login system using ASP.NET Core Identity.
- **JWT Authentication**: Stateless and secure API communication using JSON Web Tokens (JWT).
- **Resource-Based Authorization**: A robust security model ensuring that each user can only access and view their own boards or boards that have been explicitly shared with them.

### Collaboration
- **Board Sharing**: Board owners can invite other registered users to collaborate on their boards.
- **Member Management**: Board owners can view all members of a board and remove them.

---

## 🛠️ Tech Stack

This project leverages a modern and powerful technology stack, demonstrating best practices in both backend and frontend development.

### Backend
- **Framework**: ASP.NET Core 8
- **API Style**: RESTful API
- **Database**: Entity Framework Core 8 with SQL Server (LocalDB)
- **Authentication**: ASP.NET Core Identity with JWT Bearer Tokens
- **Architecture**:
    - Clean, Layered Architecture (Controllers, Services, Data Access)
    - Dependency Injection for loose coupling
    - DTOs (Data Transfer Objects) for clean API contracts

### Frontend
- **Framework**: React 18 (with Vite)
- **Routing**: React Router v6 for client-side routing
- **State Management**: React Context API
- **API Communication**: Axios with interceptors for automatic JWT attachment
- **Drag & Drop**: `@dnd-kit` for a modern, accessible, and performant drag-and-drop experience
- **UI Components**: `react-modal` for accessible popup windows

---

## 🚀 Getting Started

To run this project locally, you will need .NET 8 SDK and Node.js installed.

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/z-moradipour/TaskFlow.git](https://github.com/z-moradipour/TaskFlow.git)
    cd TaskFlow
    ```

2.  **Configure the Backend:**
    - Open the solution `TaskFlow.sln` in Visual Studio.
    - Ensure the connection string in `TaskFlow.Api/appsettings.json` is configured for your local database.
    - Run the database migrations:
      ```powershell
      # In the Package Manager Console
      Update-Database
      ```
    - Run the backend project (F5).

3.  **Configure the Frontend:**
    - Navigate to the frontend directory: `cd frontend`
    - Install dependencies: `npm install`
    - Start the development server: `npm run dev`

The frontend will be available at `http://localhost:5173`.

---

## 🔮 Future Work & Roadmap

This project is a solid foundation, and the following professional-grade features are planned for future implementation. I will be updating this README as each feature is completed.

- **✅ Automated Tests (Coming Soon)**
  - Write unit tests for the backend service layer using xUnit or NUnit to ensure code quality and reliability.
- **✅ Activity Log (Coming Soon)**
  - Implement a system to track and display a history of all actions performed on a board (e.g., "User X moved card Y").
- **✅ Real-time Collaboration with SignalR (Coming Soon)**
  - Integrate ASP.NET Core SignalR to enable live, real-time updates across all connected clients. When one user moves a card, it will move instantly for all other users viewing the same board.
- **✅ Responsive Frontend Design (Coming Soon)**
  - Improve the CSS to ensure the application is fully usable and looks great on mobile devices and tablets.