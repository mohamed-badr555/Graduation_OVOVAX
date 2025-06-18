Write-Host "🧪 Testing OVOVAX API Endpoints"
Write-Host "================================"
Write-Host ""

# Wait for API to be ready
Write-Host "⏳ Waiting for API to start..."
Start-Sleep 5

# Test 1: Simple endpoint test
Write-Host "📋 Test 1: Email exists check"
try {
    $result = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/emailexists?email=test@example.com" -Method GET
    Write-Host "✅ API is responding. Result: $result"
} catch {
    Write-Host "❌ API not responding: $($_.Exception.Message)"
    exit 1
}

# Test 2: Register Account
Write-Host ""
Write-Host "📋 Test 2: Register muhammed.adel455@gmail.com"
$registerBody = @{
    firstName = "Muhammed"
    lastName = "Adel"  
    email = "muhammed.adel455@gmail.com"
    password = "SecurePass123!"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"
    Write-Host "✅ Registration successful!"
    Write-Host "User ID: $($registerResponse.user.id)"
    Write-Host "Email: $($registerResponse.user.email)"
    Write-Host "Token (first 20 chars): $($registerResponse.token.Substring(0,20))..."
    $token = $registerResponse.token
} catch {
    Write-Host "❌ Registration failed: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $errorDetails = $reader.ReadToEnd()
        Write-Host "Error details: $errorDetails"
    }
}

# Test 3: Login
if ($token) {
    Write-Host ""
    Write-Host "📋 Test 3: Login with same credentials"
    $loginBody = @{
        email = "muhammed.adel455@gmail.com"
        password = "SecurePass123!"
    } | ConvertTo-Json

    try {
        $loginResponse = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
        Write-Host "✅ Login successful!"
        Write-Host "Token (first 20 chars): $($loginResponse.token.Substring(0,20))..."
    } catch {
        Write-Host "❌ Login failed: $($_.Exception.Message)"
    }

    # Test 4: Validate Token
    Write-Host ""
    Write-Host "📋 Test 4: Validate Token (Protected Endpoint)"
    $headers = @{ Authorization = "Bearer $token" }
    
    try {
        $validateResponse = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/validate-token" -Method GET -Headers $headers
        Write-Host "✅ Token validation successful!"
        Write-Host "User: $($validateResponse.user.firstName) $($validateResponse.user.lastName)"
    } catch {
        Write-Host "❌ Token validation failed: $($_.Exception.Message)"
    }

    # Test 5: Password Validation
    Write-Host ""
    Write-Host "📋 Test 5: Password Validation"
    $passwordBody = @{
        password = "SecurePass123!"
    } | ConvertTo-Json

    try {
        $passwordResponse = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/validate-password" -Method POST -Body $passwordBody -ContentType "application/json"
        Write-Host "✅ Password validation successful!"
        Write-Host "Is Valid: $($passwordResponse.isValid)"
        Write-Host "Requirements met:"
        Write-Host "  - Length (≥8): $($passwordResponse.requirements.length)"
        Write-Host "  - Uppercase: $($passwordResponse.requirements.uppercase)"
        Write-Host "  - Lowercase: $($passwordResponse.requirements.lowercase)"
        Write-Host "  - Number: $($passwordResponse.requirements.number)"
        Write-Host "  - Special: $($passwordResponse.requirements.special)"
    } catch {
        Write-Host "❌ Password validation failed: $($_.Exception.Message)"
    }

    # Test 6: Forgot Password (Email Test)
    Write-Host ""
    Write-Host "📋 Test 6: Forgot Password (Email Test)"
    $forgotBody = @{
        email = "muhammed.adel455@gmail.com"
    } | ConvertTo-Json

    try {
        $forgotResponse = Invoke-RestMethod -Uri "http://localhost:5243/api/auth/forgot-password" -Method POST -Body $forgotBody -ContentType "application/json"
        Write-Host "✅ Forgot password successful!"
        Write-Host "Message: $($forgotResponse.message)"
        Write-Host "📧 Check your email: muhammed.adel455@gmail.com"
    } catch {
        Write-Host "❌ Forgot password failed: $($_.Exception.Message)"
    }
}

Write-Host ""
Write-Host "🎉 All tests completed!"
Write-Host ""
Read-Host "Press Enter to close"
