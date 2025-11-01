# School Equipment API (server-net)

This is a minimal ASP.NET Core (net7.0) Web API used as a mock backend for the School Equipment Portal.

Features
- Signup / Login (simple token-based session stored server-side in `data.json`)
- Equipment CRUD (admin only for create/update/delete)
- Borrowing requests: create, approve, reject, mark returned
- Prevents overlapping approved bookings for the same equipment

Run
1. Install .NET SDK 7+ from https://dotnet.microsoft.com/download
2. From this folder run:

```powershell
cd server-net
dotnet restore
dotnet run
```

The API will run on http://localhost:5000 by default (Kestrel chooses available port; check console output).

Notes
- Data is persisted to `data.json` next to the app (simple file-based store for demo only).
- Passwords are stored in plaintext in this demo — do NOT use this in production.
- Tokens are random GUID strings stored in `data.json` sessions map.

Endpoints (summary)
- POST /api/auth/signup { username, password, role }
- POST /api/auth/login { username, password }
- GET /api/auth/me (Authorization: Bearer <token>)
- GET /api/equipment
- POST /api/equipment (admin)
- PUT /api/equipment/{id} (admin)
- DELETE /api/equipment/{id} (admin)
- POST /api/requests { equipmentId, startDate, endDate }
- GET /api/requests
- PUT /api/requests/{id}/approve
- PUT /api/requests/{id}/reject
- PUT /api/requests/{id}/return

If you want, I can:
- Add Dockerfile for the API
- Add JWT authentication and password hashing (bcrypt)
- Add CORS configuration to allow the client on a different host

