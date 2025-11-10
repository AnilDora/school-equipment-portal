# Project Creation Prompt - School Equipment Portal

## Project Overview Prompt

```
Create a full-stack School Equipment Management Portal with the following specifications:

PROJECT NAME: School Equipment Portal
TECH STACK: 
- Backend: ASP.NET Core 7 Web API
- Frontend: React 19 with React Bootstrap
- Database: MongoDB with File-based fallback
- Authentication: JWT Bearer Token

PURPOSE: A web application for managing school equipment with role-based access control for students, staff, and administrators.
```

---

## Part 1: Backend Development (.NET 7 Web API)

### Initial Setup Prompt

```
Create a .NET 7 Web API project with the following structure:

PROJECT SETUP:
1. Create a new ASP.NET Core Web API project named "SchoolEquipmentApi"
2. Target framework: .NET 7.0
3. Enable CORS for React frontend
4. Configure port: 5010
5. Add MongoDB.Driver NuGet package (version 3.5.0)

COMMANDS:
dotnet new webapi -n SchoolEquipmentApi
cd SchoolEquipmentApi
dotnet add package MongoDB.Driver --version 3.5.0
```

### Folder Structure Prompt

```
Organize the backend project with the following folder structure:

FOLDERS TO CREATE:
- Controllers/     # API controllers for routing
- DTOs/           # Data Transfer Objects and models
- Modules/        # Data storage implementations

ORGANIZATION PRINCIPLES:
- Separate concerns (MVC pattern)
- Interface-based design for data storage
- Clear separation between models, controllers, and business logic
```

### Data Models (DTOs) Prompt

```
Create the following data models in the DTOs folder:

FILE: DTOs/Models.cs
MODELS NEEDED:
1. User class:
   - Properties: Id (int), Username (string), Password (string, hashed), Role (string: student/staff/admin)
   
2. Equipment class:
   - Properties: Id (int), Name (string), Category (string), Condition (string), Quantity (int), Available (bool)
   
3. Request class:
   - Properties: Id (int), UserId (int), Username (string), EquipmentId (int), EquipmentName (string), 
                 StartDate (DateTime), EndDate (DateTime), Status (string: pending/approved/rejected), RequestDate (DateTime)

FILE: DTOs/Data.cs
- Create a container class that holds:
  - List<User> Users
  - List<Equipment> Equipment  
  - List<Request> Requests

FILE: DTOs/RequestDtos.cs
- LoginRequest: username, password
- LoginResponse: user, token
- SignupRequest: username, password, role
- CreateEquipmentRequest: name, category, condition, quantity, available
- UpdateEquipmentRequest: name, category, condition, quantity, available
- CreateRequestDto: equipmentId, startDate, endDate
- UpdateRequestStatusDto: status

FILE: DTOs/Helpers.cs
- Authentication helper methods
- Password hashing (simple hash for demo)
- Token generation (simple GUID-based token)
```

### Data Storage (Modules) Prompt

```
Create a flexible data storage system with multiple implementations:

FILE: Modules/IDataStore.cs
- Interface with methods: Read(), Write(Data data)

FILE: Modules/FileDataStore.cs
- Implement IDataStore interface
- Use JSON file storage (data.json)
- Features:
  - Atomic file operations with lock mechanism
  - Automatic backup creation before write
  - Error handling with detailed logging
  - Thread-safe operations

FILE: Modules/MongoDataStore.cs
- Implement IDataStore interface
- Use MongoDB as storage backend
- Features:
  - Automatic index creation (username, equipmentId, userId)
  - Connection pooling
  - Error handling and retry logic
  - Database statistics tracking
- Collections: Users, Equipment, Requests

CONFIGURATION:
- Use appsettings.json for connection strings
- Auto-detect MongoDB availability and fallback to file storage
- Support both development and production configurations
```

### Controllers Prompt

```
Create the following API controllers in the Controllers folder:

FILE: Controllers/UsersController.cs
ROUTE: /api/auth
ENDPOINTS:
- POST /signup: Register new user (username, password, role)
- POST /login: Authenticate user, return JWT token
- GET /me: Get current user info (requires Bearer token)

AUTHENTICATION:
- Use custom JWT token generation (GUID-based for simplicity)
- Store active sessions in memory
- Validate Bearer token on protected endpoints

FILE: Controllers/EquipmentController.cs
ROUTE: /api/equipment
ENDPOINTS:
- GET /: List all equipment (authenticated users)
- GET /{id}: Get equipment by ID
- POST /: Create equipment (admin only)
- PUT /{id}: Update equipment (admin only)
- DELETE /{id}: Delete equipment (admin only)

AUTHORIZATION:
- Check user role from token
- Enforce admin-only operations

FILE: Controllers/RequestsController.cs
ROUTE: /api/requests
ENDPOINTS:
- GET /: Get all requests (staff/admin) or user's requests (students)
- GET /my: Get current user's requests
- POST /: Create new request (students)
- PUT /{id}: Update request status (staff/admin: approve/reject)
- DELETE /{id}: Cancel request (optional)

BUSINESS LOGIC:
- Validate date ranges
- Check equipment availability
- Prevent duplicate requests
- Role-based filtering
```

### Configuration Files Prompt

```
FILE: appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:8000",
    "DatabaseName": "SchoolEquipmentAIDatabase",
    "UsersCollectionName": "Users",
    "EquipmentCollectionName": "Equipment",
    "RequestsCollectionName": "Requests"
  }
}

FILE: appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:8000"
  }
}

FILE: Program.cs
CONFIGURATION:
- Enable CORS for React frontend (http://localhost:3000)
- Configure dependency injection for IDataStore
- Add controllers and Swagger
- Configure JSON serialization
- Set up authentication middleware
- Auto-select MongoDB or FileDataStore based on connection availability
```

### Startup Configuration Prompt

```
Configure Program.cs with the following:

CORS POLICY:
- Allow origin: http://localhost:3000
- Allow any method, header, and credentials

DEPENDENCY INJECTION:
- Try MongoDB connection first
- If MongoDB available: Register MongoDataStore
- If MongoDB fails: Fallback to FileDataStore with data.json
- Log which storage system is being used

MIDDLEWARE ORDER:
1. CORS
2. Authorization
3. Map Controllers
4. Swagger (development only)

LAUNCH SETTINGS:
- HTTP: http://localhost:5010
- HTTPS: https://localhost:7010 (optional)
```

---

## Part 2: Frontend Development (React)

### React App Setup Prompt

```
Create a React application with the following specifications:

INITIALIZATION:
npx create-react-app client
cd client

DEPENDENCIES TO INSTALL:
npm install react-router-dom@latest
npm install bootstrap@latest
npm install react-bootstrap@latest

CONFIGURATION:
- Base API URL: http://localhost:5010/api (configurable via .env)
- Port: 3000 (default)
```

### Folder Structure Prompt

```
Organize the React app with the following structure:

src/
├── components/      # Reusable components
│   ├── NavBar.js
│   └── PrivateRoute.js
├── context/        # Context providers
│   └── AuthContext.js
├── pages/          # Page components
│   ├── Login.js
│   ├── Signup.js
│   ├── Dashboard.js
│   ├── AdminDashboard.js
│   ├── EquipmentManagement.js
│   ├── StaffDashboard.js
│   ├── Requests.js
│   └── NotFound.js
├── App.js          # Main app with routing
├── App.css         # App styles
├── index.js        # Entry point
└── index.css       # Global styles
```

### Context Provider Prompt

```
FILE: src/context/AuthContext.js

Create an authentication context with the following features:

STATE MANAGEMENT:
- user: Current logged-in user object {id, username, role}
- loading: Boolean for initialization state

METHODS:
- signup(username, password, role): Register user via POST /api/auth/signup
- login(username, password): Authenticate via POST /api/auth/login
- logout(): Clear session and localStorage
- Initialize: Check localStorage for token, fetch user via GET /api/auth/me

LOCAL STORAGE:
- Store token in localStorage
- Store user object in localStorage
- Clear on logout

API INTEGRATION:
- Base URL: process.env.REACT_APP_API_BASE || 'http://localhost:5010/api'
- Handle errors gracefully
- Return {success: boolean, message: string, user: object}
```

### Components Prompt

```
FILE: src/components/NavBar.js
CREATE: Navigation bar using React Bootstrap
FEATURES:
- Brand: "Equipment Portal"
- Conditional links based on authentication and role:
  - Logged out: Login, Sign Up
  - Logged in: Dashboard, Requests
  - Admin: Manage Equipment
  - Staff: Staff Panel
- Display current user (username, role)
- Logout button

FILE: src/components/PrivateRoute.js
CREATE: Route protection wrapper
LOGIC:
- Check if user is authenticated
- Check if user's role is in allowedRoles array
- Show loading state during auth check
- Redirect to /login if not authenticated
- Show "Access Denied" if role not allowed
- Render <Outlet /> if authorized
```

### Pages Prompt

```
FILE: src/pages/Login.js
FORM FIELDS:
- Username (text input)
- Password (password input)
ACTIONS:
- Submit → call AuthContext.login()
- On success → navigate to /
- On error → display error message
STYLING: Use React Bootstrap Card, Form, Button, Alert

FILE: src/pages/Signup.js
FORM FIELDS:
- Username (text input)
- Password (password input)
- Role (select: student/staff/admin)
ACTIONS:
- Submit → call AuthContext.signup()
- On success → navigate to /
- On error → display error message

FILE: src/pages/Dashboard.js
PURPOSE: Equipment catalog for all users
FEATURES:
- Fetch equipment list via GET /api/equipment
- Search by name (client-side filter)
- Filter by category (dynamic from data)
- Filter by availability (available/unavailable/all)
- Display equipment table
STUDENT-SPECIFIC:
- "Request" button for available equipment
- Modal with datetime inputs (start/end dates)
- Submit request via POST /api/requests

FILE: src/pages/AdminDashboard.js
PURPOSE: Admin landing page
CONTENT:
- Welcome message
- Quick links to admin functions
- Link to /admin/equipment

FILE: src/pages/EquipmentManagement.js
PURPOSE: CRUD operations for equipment (admin only)
FEATURES:
- Display equipment table
- Add Equipment button → Modal form
- Edit button per row → Modal with pre-filled form
- Delete button per row → Confirmation
- Create: POST /api/equipment
- Update: PUT /api/equipment/:id
- Delete: DELETE /api/equipment/:id
FORM FIELDS:
- Name, Category, Condition, Quantity, Available (checkbox)

FILE: src/pages/StaffDashboard.js
PURPOSE: Staff request management
FEATURES:
- Fetch all requests via GET /api/requests
- Display table with: student, equipment, dates, status
- Approve/Reject buttons for pending requests
- Update status via PUT /api/requests/:id

FILE: src/pages/Requests.js
PURPOSE: View user's own requests
FEATURES:
- Fetch user's requests via GET /api/requests/my
- Display table: equipment, dates, status
- Color-coded status (pending/approved/rejected)
- Optional: Cancel button for pending requests

FILE: src/pages/NotFound.js
PURPOSE: 404 page
CONTENT:
- "Page Not Found" message
- Link back to dashboard
```

### Routing Configuration Prompt

```
FILE: src/App.js

SETUP:
- Wrap app in BrowserRouter
- Wrap app in AuthProvider
- Include NavBar (always visible)

ROUTES:
- /login → Login (public)
- /signup → Signup (public)
- / → Dashboard (protected: student, staff, admin)
- /admin → AdminDashboard (protected: admin)
- /admin/equipment → EquipmentManagement (protected: admin)
- /staff → StaffDashboard (protected: staff, admin)
- /requests → Requests (protected: student, staff, admin)
- * → NotFound (fallback)

PROTECTION:
Use PrivateRoute wrapper with allowedRoles prop for each protected route
```

### Environment Configuration Prompt

```
FILE: .env (create in client folder)
REACT_APP_API_BASE=http://localhost:5010/api

FILE: .env.production
REACT_APP_API_BASE=https://your-api-domain.com/api

USAGE IN CODE:
const API_BASE = process.env.REACT_APP_API_BASE || 'http://localhost:5010/api';
```

### Styling Prompt

```
FILE: src/index.css
IMPORTS:
import 'bootstrap/dist/css/bootstrap.min.css';

GLOBAL STYLES:
- Set default font family
- Add spacing utilities
- Style tables, forms, buttons consistently

FILE: src/App.css
APP-SPECIFIC STYLES:
- Container padding
- Card styling
- Custom navbar colors (optional)
- Responsive design adjustments
```

---

## Part 3: Integration & Testing

### API-Frontend Integration Prompt

```
ENSURE THE FOLLOWING:

1. CORS Configuration:
   - Backend allows http://localhost:3000
   - Frontend sends credentials with requests

2. Authentication Flow:
   - Frontend stores JWT token in localStorage
   - All API requests include: Authorization: Bearer ${token}
   - Token validated on backend for protected endpoints

3. Error Handling:
   - Backend returns consistent error format: {message: "error description"}
   - Frontend displays errors to user (alerts/toasts)
   - Handle network errors gracefully

4. Data Synchronization:
   - Refresh data after create/update/delete operations
   - Update UI optimistically where appropriate
   - Show loading states during API calls
```

### Testing Commands Prompt

```
BACKEND TESTING:
cd server-net
dotnet build                                    # Verify compilation
dotnet run --urls="http://localhost:5010"      # Start server

TEST ENDPOINTS:
# Signup
curl -X POST http://localhost:5010/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123","role":"admin"}'

# Login
curl -X POST http://localhost:5010/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# Get Equipment (with token)
curl http://localhost:5010/api/equipment \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"

FRONTEND TESTING:
cd client
npm start                    # Start React dev server
# Open http://localhost:3000
# Test all user flows manually

BUILD FOR PRODUCTION:
cd client
npm run build               # Creates optimized build in build/ folder
```

---

## Part 4: Database Setup

### MongoDB Setup Prompt (Optional)

```
OPTION 1: Local MongoDB Installation
1. Download MongoDB Community Server
2. Install and start MongoDB service
3. Default port: 27017 (change to 8000 in config if needed)
4. No authentication required for development

OPTION 2: MongoDB Atlas (Cloud)
1. Create free account at mongodb.com/atlas
2. Create cluster
3. Get connection string
4. Update appsettings.json with connection string

OPTION 3: Docker MongoDB
docker run -d -p 8000:27017 --name mongodb mongo:latest

CONNECTION STRING FORMAT:
mongodb://localhost:8000
mongodb://username:password@localhost:8000
mongodb+srv://cluster.mongodb.net/database

FILE-BASED FALLBACK:
- If MongoDB unavailable, system automatically uses data.json
- No additional setup required
- Data persists in JSON format
```

---

## Part 5: Documentation

### README Files Prompt

```
CREATE DOCUMENTATION:

FILE: README.md (root)
- Project overview
- Tech stack
- Features list
- Setup instructions
- Running instructions
- Project structure

FILE: server-net/README.md
- Backend-specific documentation
- API endpoints
- Authentication flow
- Database configuration
- Development guide

FILE: server-net/PROJECT-STRUCTURE.md
- Detailed folder structure
- File descriptions
- Architecture explanation
- Design patterns used

FILE: server-net/Modules/README.md
- Data storage documentation
- IDataStore interface
- FileDataStore vs MongoDataStore
- Configuration guide

FILE: server-net/Modules/README-MongoDB.md
- MongoDB-specific setup
- Connection string examples
- Troubleshooting guide

FILE: client/COMPONENT-HIERARCHY.md
- Complete component tree
- Component documentation
- Props and state
- Data flow diagrams
- API integration points
```

---

## Part 6: Complete Setup Commands

### Full Project Setup (Step by Step)

```bash
# 1. CREATE PROJECT ROOT
mkdir school-equipment-portal
cd school-equipment-portal

# 2. SETUP BACKEND
mkdir server-net
cd server-net
dotnet new webapi -n SchoolEquipmentApi
cd SchoolEquipmentApi
dotnet add package MongoDB.Driver --version 3.5.0

# Create folder structure
mkdir Controllers DTOs Modules

# Create all files as per prompts above
# ... (create Controllers, DTOs, Modules files)

# Update Program.cs with configuration
# Update appsettings.json

# Build and run
dotnet build
dotnet run --urls="http://localhost:5010"

# 3. SETUP FRONTEND
cd ../../
npx create-react-app client
cd client

# Install dependencies
npm install react-router-dom bootstrap react-bootstrap

# Create folder structure
mkdir src/components src/context src/pages

# Create all files as per prompts above
# ... (create components, context, pages)

# Update App.js with routing
# Update index.css with Bootstrap import

# Start development server
npm start

# 4. TEST THE APPLICATION
# Backend: http://localhost:5010
# Frontend: http://localhost:3000
# Create admin user via signup
# Login and test all features
```

---

## Part 7: Key Features to Implement

### Feature Checklist

```
AUTHENTICATION & AUTHORIZATION:
☐ User signup with role selection
☐ User login with JWT token
☐ Token-based session management
☐ Role-based access control (student, staff, admin)
☐ Protected routes in React
☐ Authorization checks in API

EQUIPMENT MANAGEMENT:
☐ View equipment catalog (all users)
☐ Search equipment by name
☐ Filter by category and availability
☐ CRUD operations (admin only)
☐ Equipment availability tracking

REQUEST MANAGEMENT:
☐ Students can request equipment
☐ Date/time range selection
☐ View own requests with status
☐ Staff can approve/reject requests
☐ Request status tracking (pending/approved/rejected)
☐ Prevent duplicate requests

UI/UX:
☐ Responsive design with Bootstrap
☐ Navigation bar with role-based links
☐ Modal forms for data entry
☐ Loading states for async operations
☐ Error messages and validation
☐ Success confirmations

DATA PERSISTENCE:
☐ MongoDB integration with automatic failover
☐ File-based storage fallback
☐ Atomic write operations
☐ Data backup mechanism
☐ Thread-safe operations
```

---

## Part 8: Customization & Extensions

### Enhancement Ideas

```
ADDITIONAL FEATURES TO CONSIDER:

1. Advanced Equipment Management:
   - Equipment images
   - Equipment categories management
   - Maintenance schedules
   - Equipment history/audit log

2. Enhanced Request System:
   - Email notifications
   - Real-time status updates (WebSocket)
   - Request comments/notes
   - Request modification before approval
   - Bulk operations

3. Reporting & Analytics:
   - Equipment usage statistics
   - Popular equipment report
   - Request trends over time
   - User activity logs

4. User Management:
   - Admin can manage users
   - Password reset functionality
   - User profiles
   - Role modification

5. UI Improvements:
   - Dark mode
   - Custom themes
   - Advanced search with multiple filters
   - Pagination for large datasets
   - Export to CSV/PDF

6. Security Enhancements:
   - Password strength validation
   - Rate limiting
   - CSRF protection
   - HTTP-only cookies for tokens
   - Refresh token mechanism

7. Testing:
   - Unit tests for backend controllers
   - Integration tests for API
   - React component tests
   - E2E tests with Playwright/Cypress
```

---

## Part 9: Deployment Prompt

### Production Deployment

```
BACKEND DEPLOYMENT:

1. Build for production:
   dotnet publish -c Release -o publish

2. Deploy options:
   - Azure App Service
   - AWS Elastic Beanstalk
   - Docker container
   - IIS (Windows Server)

3. Environment variables:
   - Set MongoDB connection string
   - Configure CORS for production frontend URL
   - Set production logging levels

FRONTEND DEPLOYMENT:

1. Build for production:
   npm run build

2. Deploy options:
   - Netlify
   - Vercel
   - Azure Static Web Apps
   - AWS S3 + CloudFront
   - GitHub Pages

3. Configuration:
   - Update REACT_APP_API_BASE in .env.production
   - Configure routing (use HashRouter if needed)
   - Enable HTTPS

DOCKER DEPLOYMENT:

Create Dockerfile for backend:
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY publish/ .
EXPOSE 5010
ENTRYPOINT ["dotnet", "SchoolEquipmentApi.dll"]

Create Dockerfile for frontend:
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

Docker Compose:
version: '3.8'
services:
  backend:
    build: ./server-net
    ports:
      - "5010:5010"
    environment:
      - ConnectionStrings__MongoDB=mongodb://mongo:27017
  
  frontend:
    build: ./client
    ports:
      - "80:80"
    depends_on:
      - backend
  
  mongo:
    image: mongo:latest
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db

volumes:
  mongodb_data:
```

---

## Part 10: Quick Start Guide

### For New Developers

```
QUICK SETUP (Assuming prerequisites installed):

# 1. Clone/Download project
git clone <repository-url>
cd school-equipment-portal

# 2. Start Backend
cd server-net
dotnet restore
dotnet run --urls="http://localhost:5010"

# 3. Start Frontend (in new terminal)
cd ../client
npm install
npm start

# 4. Access Application
# Frontend: http://localhost:3000
# Backend API: http://localhost:5010
# Swagger: http://localhost:5010/swagger (if enabled)

# 5. Create First Admin User
# Go to http://localhost:3000/signup
# Username: admin
# Password: admin123
# Role: admin

# 6. Login and Explore
# Login with admin credentials
# Navigate through all sections
# Create sample equipment
# Test request flows with different roles
```

---

## Prerequisites

```
REQUIRED SOFTWARE:

Backend:
- .NET 7 SDK or later
- Visual Studio 2022 / VS Code / Rider (IDE)
- MongoDB (optional, falls back to file storage)

Frontend:
- Node.js 16+ and npm
- Modern web browser (Chrome, Firefox, Edge)

Development Tools:
- Git for version control
- Postman/Insomnia for API testing
- MongoDB Compass (optional, for MongoDB GUI)

Operating System:
- Windows 10/11
- macOS 10.15+
- Linux (Ubuntu 20.04+)
```

---

## Success Criteria

```
PROJECT IS COMPLETE WHEN:

✓ Backend API runs on http://localhost:5010
✓ Frontend app runs on http://localhost:3000
✓ User can signup with role selection
✓ User can login and receive JWT token
✓ Navigation bar shows role-based links
✓ Admin can create/edit/delete equipment
✓ Students can view equipment catalog
✓ Students can request available equipment
✓ Staff can approve/reject requests
✓ All roles can view their requests
✓ Data persists (MongoDB or file-based)
✓ CORS allows frontend-backend communication
✓ Protected routes work correctly
✓ Error handling is graceful
✓ UI is responsive and user-friendly
```

---

## Troubleshooting Guide

```
COMMON ISSUES:

1. CORS Error:
   - Check Program.cs has correct frontend URL
   - Verify frontend API_BASE is correct
   - Ensure WithCredentials is set if using cookies

2. MongoDB Connection Failed:
   - Check MongoDB is running
   - Verify connection string in appsettings.json
   - Check firewall settings
   - System will fallback to file storage automatically

3. Token Invalid:
   - Clear localStorage and login again
   - Check token is being sent in Authorization header
   - Verify token validation logic in backend

4. Routes Not Working:
   - Check PrivateRoute allowedRoles
   - Verify user role is set correctly
   - Check React Router setup in App.js

5. Build Errors:
   - Run: dotnet restore (backend)
   - Run: npm install (frontend)
   - Check .NET 7 SDK is installed
   - Check Node.js version (16+)

6. API Not Responding:
   - Verify backend is running on correct port
   - Check for compilation errors
   - Review Program.cs configuration
   - Check Windows Firewall / antivirus
```

---

*This prompt file provides complete instructions to recreate the School Equipment Portal project from scratch.*

**Version**: 1.0  
**Created**: November 9, 2025  
**Author**: School Equipment Portal Development Team
