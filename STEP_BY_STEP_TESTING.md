# 🚀 Step-by-Step API Testing Guide

## Step 1: Start the API Server

Open your terminal and run:
```bash
cd "d:\4T2\Graduation\OVOVAX  Solution\OVOVAX.API"
dotnet run
```

The API should start on: **http://localhost:5243**

## Step 2: Open Swagger UI

Open your browser and go to: **http://localhost:5243/swagger**

You should see:
- 🔒 **Authorize** button (top-right corner)
- All API endpoints listed
- Auth endpoints: register, login, validate-token, etc.

## Step 3: Create Account (Register)

### 3.1 Using Swagger UI:
1. Find `POST /api/auth/register`
2. Click "Try it out"
3. Use this JSON:

```json
{
  "firstName": "Mohamed",
  "lastName": "Badr",
  "email": "mohamed@test.com",
  "password": "SecurePass123!"
}
```

4. Click "Execute"

### 3.2 Expected Response:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "12345-guid",
    "firstName": "Mohamed",
    "lastName": "Badr",
    "email": "mohamed@test.com",
    "lastLogin": "2025-06-18T..."
  }
}
```

**✅ Copy the `token` value - you'll need it!**

## Step 4: Test Login

### 4.1 Using Swagger UI:
1. Find `POST /api/auth/login`
2. Click "Try it out"
3. Use this JSON:

```json
{
  "email": "mohamed@test.com",
  "password": "SecurePass123!"
}
```

4. Click "Execute"

### 4.2 Expected Response:
Should return the same format with a new token.

## Step 5: Authorize in Swagger

### 5.1 Set Authorization:
1. Click the 🔒 **Authorize** button (top-right)
2. In the popup, enter: `Bearer YOUR_TOKEN_HERE`
   - Replace `YOUR_TOKEN_HERE` with the actual token from Step 3
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Click **Authorize**
4. Click **Close**

## Step 6: Test Protected Endpoint

### 6.1 Validate Token:
1. Find `GET /api/auth/validate-token`
2. Click "Try it out"
3. Click "Execute"

### 6.2 Expected Response:
```json
{
  "success": true,
  "token": "your-token",
  "user": {
    "id": "12345-guid",
    "firstName": "Mohamed",
    "lastName": "Badr",
    "email": "mohamed@test.com",
    "lastLogin": "2025-06-18T..."
  }
}
```

## Step 7: Test Password Validation

### 7.1 Test Strong Password:
1. Find `POST /api/auth/validate-password`
2. Click "Try it out"
3. Use this JSON:

```json
{
  "password": "StrongPass123!"
}
```

### 7.2 Expected Response:
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

### 7.3 Test Weak Password:
Use this JSON:
```json
{
  "password": "weak"
}
```

Expected response - all requirements should be `false`.

## Step 8: Test Email Check

### 8.1 Check Existing Email:
1. Find `GET /api/auth/emailexists`
2. Click "Try it out"
3. Enter email: `mohamed@test.com`
4. Click "Execute"

Expected response: `true`

### 8.2 Check Non-existing Email:
Enter email: `nonexistent@test.com`
Expected response: `false`

## Step 9: Test Forgot Password (Email)

### 9.1 Send Reset Email:
1. Find `POST /api/auth/forgot-password`
2. Click "Try it out"
3. Use this JSON:

```json
{
  "email": "mohamedb.555dr@gmail.com"
}
```

4. Click "Execute"

### 9.2 Expected Response:
```json
{
  "success": true,
  "message": "Password reset email sent successfully"
}
```

**✅ Check your Gmail inbox for the password reset email!**

## Step 10: Test Error Cases

### 10.1 Register with Existing Email:
Try to register again with the same email - should fail.

### 10.2 Login with Wrong Password:
```json
{
  "email": "mohamed@test.com",
  "password": "WrongPassword123!"
}
```

### 10.3 Test Without Authorization:
1. Click 🔒 **Authorize** and click **Logout**
2. Try `GET /api/auth/validate-token` again
3. Should get `401 Unauthorized`

---

## ✅ Complete Testing Checklist

- [ ] ✅ API server started successfully
- [ ] ✅ Swagger UI opens and shows 🔒 Authorize button
- [ ] ✅ Register new user - get token
- [ ] ✅ Login with same credentials - get token
- [ ] ✅ Authorize in Swagger with Bearer token
- [ ] ✅ Validate token endpoint works
- [ ] ✅ Password validation - strong password
- [ ] ✅ Password validation - weak password
- [ ] ✅ Email exists check - existing email
- [ ] ✅ Email exists check - non-existing email
- [ ] ✅ Forgot password - email sent
- [ ] ✅ Test error cases (wrong password, existing email, etc.)

---

## 🎯 Key Points to Verify

1. **🔒 Authorize Button**: Must be visible in Swagger
2. **JWT Token**: Should be long string starting with `eyJ`
3. **Password Requirements**: All 5 requirements working
4. **Email Integration**: Real email sent to Gmail
5. **Error Handling**: Proper error messages for failures
6. **CORS**: No CORS errors in browser console

**Once you complete all steps, your authentication system is fully validated!** 🚀
