# 🧪 OVOVAX API Testing Guide

## 🚀 Starting the API Server

1. **Open Terminal in VS Code**
2. **Navigate to API project:**
   ```bash
   cd "OVOVAX.API"
   ```
3. **Run the API:**
   ```bash
   dotnet run
   ```
4. **API will start at:** `https://localhost:5243`

## 📊 Testing All Endpoints

### 1. **🔓 Password Validation (No Auth Required)**
```http
POST https://localhost:5243/api/auth/validate-password
Content-Type: application/json

{
  "password": "TestPass123!"
}
```

**Expected Response:**
```json
{
  "isValid": true,
  "requirements": {
    "length": true,
    "uppercase": true,
    "lowercase": true,
    "number": true,
    "special": true
  }
}
```

### 2. **👤 User Registration**
```http
POST https://localhost:5243/api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

**Expected Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "user-guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "lastLogin": "2025-06-18T10:30:00Z"
  }
}
```

### 3. **🔑 User Login**
```http
POST https://localhost:5243/api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

**Expected Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "user-guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "lastLogin": "2025-06-18T10:35:00Z"
  }
}
```

### 4. **📧 Forgot Password (Email Test)**
```http
POST https://localhost:5243/api/auth/forgot-password
Content-Type: application/json

{
  "email": "john.doe@example.com"
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Password reset email sent successfully"
}
```

**Check Your Gmail:** `mohamedb.555dr@gmail.com` - You should receive a password reset email!

### 5. **✅ Check Email Exists**
```http
GET https://localhost:5243/api/auth/emailexists?email=john.doe@example.com
```

**Expected Response:**
```json
true
```

### 6. **🔒 Token Validation (Protected Endpoint)**

**First, copy the JWT token from login/register response, then:**

```http
GET https://localhost:5243/api/auth/validate-token
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Expected Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "user-guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "lastLogin": "2025-06-18T10:35:00Z"
  }
}
```

## 🧪 Testing Methods

### **Option 1: Swagger UI (Recommended)**
1. **Start API:** `dotnet run`
2. **Open:** `https://localhost:5243/swagger`
3. **Use the Authorize button** 🔐 for protected endpoints
4. **Test each endpoint interactively**

### **Option 2: HTTP Files (VS Code)**
Create a file named `api-tests.http`:

```http
### Test Password Validation
POST https://localhost:5243/api/auth/validate-password
Content-Type: application/json

{
  "password": "TestPass123!"
}

### Test Registration
POST https://localhost:5243/api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}

### Test Login
POST https://localhost:5243/api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}

### Test Forgot Password
POST https://localhost:5243/api/auth/forgot-password
Content-Type: application/json

{
  "email": "john.doe@example.com"
}

### Test Email Exists
GET https://localhost:5243/api/auth/emailexists?email=john.doe@example.com

### Test Token Validation (Replace with actual token)
GET https://localhost:5243/api/auth/validate-token
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

### **Option 3: Postman Collection**
Import the requests above into Postman and test systematically.

## 🎯 Expected Test Results

✅ **Password Validation**: Returns detailed requirements check
✅ **Registration**: Creates user + sends welcome email (optional)
✅ **Login**: Returns JWT token and user info
✅ **Forgot Password**: Sends reset email to your Gmail
✅ **Email Check**: Returns true/false for email existence
✅ **Token Validation**: Validates JWT and returns user info

## 🐛 Common Issues & Solutions

### **Database Issues:**
```bash
# Update database if migrations are pending
dotnet ef database update --project OVOVAX.Repository
```

### **Email Issues:**
- Check your Gmail app password: `extqcppykczfzipa`
- Verify SMTP settings in `appsettings.json`

### **JWT Issues:**
- Token expires in 7 days (configurable)
- Make sure to include "Bearer " prefix in Authorization header

## 🏆 Success Criteria

🎯 **All 6 endpoints working**
🎯 **JWT authentication functional**
🎯 **Email sending operational**
🎯 **Password validation enforced**
🎯 **Local database connected**
🎯 **Swagger UI accessible**

---

**Ready to test! Start the API and begin with Swagger UI for the best experience.** 🚀
