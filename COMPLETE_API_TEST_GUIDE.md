# 🚀 OVOVAX API Testing Guide

## ✅ Complete Authentication System Ready!

Your OVOVAX API now includes **JWT Authentication** with **Swagger Authorization** button.

---

## 🔧 How to Start & Test

### 1. **Start the API Server**
```bash
cd "D:\4T2\Graduation\OVOVAX  Solution\OVOVAX.API"
dotnet run --urls "http://localhost:5243"
```

### 2. **Open Swagger UI**
Navigate to: **http://localhost:5243/swagger**

You'll now see the **🔒 Authorize** button in the top-right corner!

---

## 📱 API Endpoints to Test

### **🔐 Authentication Endpoints**

#### 1. **Register User**
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Expected Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "user-guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "lastLogin": "2025-06-18T10:30:00Z"
  }
}
```

#### 2. **Login User**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

#### 3. **Password Validation (Frontend Feature)**
```http
POST /api/auth/validate-password
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

#### 4. **Forgot Password (Email Integration)**
```http
POST /api/auth/forgot-password
Content-Type: application/json

{
  "email": "john@example.com"
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Password reset email sent successfully"
}
```

#### 5. **Check Email Exists**
```http
GET /api/auth/emailexists?email=john@example.com
```

**Expected Response:**
```json
true
```

#### 6. **Validate Token (Protected Endpoint)**
```http
GET /api/auth/validate-token
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

---

## 🔒 How to Use Swagger Authorization

### Step 1: Register/Login to Get Token
1. Use `/api/auth/register` or `/api/auth/login`
2. Copy the `token` from the response

### Step 2: Authorize in Swagger
1. Click the **🔒 Authorize** button (top-right)
2. Enter: `Bearer YOUR_TOKEN_HERE`
3. Click **Authorize**

### Step 3: Test Protected Endpoints
- All endpoints marked with 🔒 can now be tested
- The authorization header will be automatically added

---

## ✅ Password Requirements Testing

Your password validation follows these exact requirements:
- ✅ **Length**: At least 8 characters
- ✅ **Uppercase**: At least one uppercase letter
- ✅ **Lowercase**: At least one lowercase letter  
- ✅ **Number**: At least one digit
- ✅ **Special**: At least one special character `!@#$%^&*(),.?":{}|<>`

### Test Cases:
| Password | Valid | Missing |
|----------|-------|---------|
| `password` | ❌ | Uppercase, Number, Special |
| `Password` | ❌ | Number, Special |
| `Password1` | ❌ | Special |
| `Password1!` | ✅ | All requirements met |
| `SecurePass123!` | ✅ | All requirements met |

---

## 📧 Email Testing (Gmail Integration)

Your email system is configured with:
- **SMTP**: `smtp.gmail.com:587`
- **From**: `mohamedb.555dr@gmail.com`
- **App Password**: `extqcppykczfzipa` ✅

### Test Email Sending:
1. Use `/api/auth/forgot-password`
2. Check your Gmail inbox for password reset email
3. Beautiful HTML template with OVOVAX branding

---

## 🎯 Complete Testing Checklist

### ✅ Basic Authentication
- [ ] Register new user
- [ ] Login with correct credentials
- [ ] Login with wrong credentials (should fail)
- [ ] Check email exists
- [ ] Password validation

### ✅ JWT Authorization
- [ ] Get token from login/register
- [ ] Authorize in Swagger
- [ ] Test protected endpoints
- [ ] Test without token (should get 401)

### ✅ Email Integration
- [ ] Send forgot password email
- [ ] Check Gmail inbox
- [ ] Verify HTML template

### ✅ Frontend Requirements
- [ ] Real-time password validation
- [ ] Detailed error messages
- [ ] CORS enabled for React frontend

---

## 🚀 Ready for Frontend Integration!

Your API is now complete with:
- ✅ **JWT Authentication** with Swagger authorize button
- ✅ **Password validation** with your exact requirements
- ✅ **Email integration** with Gmail SMTP
- ✅ **Clean Architecture** following Talabat pattern
- ✅ **CORS enabled** for React frontend
- ✅ **Local database** for safe testing

**All endpoints are ready for your frontend application!** 🎉
