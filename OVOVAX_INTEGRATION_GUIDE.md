# OVOVAX Frontend Integration Guide - Complete Documentation

## 🚀 Overview

This is the complete integration guide for connecting your React frontend to the OVOVAX API. This guide consolidates all previous documentation and provides the definitive solution for frontend-backend communication.

### System Architecture
- **Frontend (React)**: `http://localhost:5173/GraduationProject/#/`
- **Backend API (HTTPS)**: `https://localhost:7268`
- **Backend API (HTTP)**: `http://localhost:5243`
- **Swagger UI**: `https://localhost:7268/swagger/index.html`

---

## 🔧 Frontend Configuration

### 1. Environment Variables (.env)

Create or update your `.env` file in your React project root:

```env
# Vite environment variables
VITE_APP_TITLE=Egg Injection System

# API Configuration - Use HTTPS for production-ready setup
VITE_API_BASE_URL=https://localhost:7268/api

# Frontend Configuration
VITE_APP_NAME="OvoVax - Automated In-Ovo Vaccination System"
VITE_APP_VERSION="1.0.0"

# Development Settings
VITE_DEV_MODE=true
```

### 2. API Manager Implementation

Create or replace your `ApiManager.js` with this complete implementation:

```javascript
// OVOVAX API Manager - Complete Implementation
// Handles all API communication with proper error handling and logging

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7268/api';

console.log('🚀 ApiManager initialized with base URL:', API_BASE_URL);

// Helper function to get headers
const getHeaders = (token = null, contentType = 'application/json') => {
  const headers = {
    'Content-Type': contentType,
  };
  
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }
  
  return headers;
};

// Helper function for API requests with enhanced logging and error handling
const apiRequest = async (endpoint, options = {}) => {
  const url = `${API_BASE_URL}${endpoint}`;
  
  const config = {
    credentials: 'include',
    headers: getHeaders(options.token, options.contentType),
    ...options,
  };

  console.log(`🌐 API Request: ${config.method || 'GET'} ${url}`);

  try {
    const response = await fetch(url, config);
    
    console.log(`📡 Response Status: ${response.status} for ${endpoint}`);

    if (!response.ok) {
      const errorText = await response.text();
      console.error(`❌ API Error: ${response.status} - ${errorText}`);
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const data = await response.json();
    console.log(`✅ API Success for ${endpoint}:`, data);
    return data;

  } catch (error) {
    console.error(`❌ API Request Failed for ${endpoint}:`, error);
    
    if (error.name === 'TypeError' && error.message.includes('fetch')) {
      console.error('🔧 Network Error - Check if API server is running');
    }
    
    throw error;
  }
};

export default class ApiManager {
    // Scanner API Methods
  static async performScan(scanData = {}) {
    return apiRequest('/Scanner/start', {
      method: 'POST',
      body: JSON.stringify({
        scanType: scanData.scanType || 'default',
        targetArea: scanData.targetArea || 'default',
        ...scanData
      }),
    });
  }

  static async stopScan(scanId, depthMeasurement) {
    return apiRequest('/Scanner/stop', {
      method: 'POST',
      body: JSON.stringify({
        scanId: scanId,
        depthMeasurement: depthMeasurement
      }),
    });
  }

  static async getScanResults() {
    return apiRequest('/Scanner/history');
  }

  static async getScanResult(id) {
    return apiRequest(`/Scanner/results/${id}`);
  }

  // Injection API Methods
  static async startInjection(injectionData) {
    return apiRequest('/Injection/start', {
      method: 'POST',
      body: JSON.stringify({
        dosage: injectionData.dosage,
        medicationType: injectionData.medicationType,
        targetLocation: injectionData.targetLocation,
        ...injectionData
      }),
    });
  }

  static async stopInjection() {
    return apiRequest('/Injection/stop', {
      method: 'POST',
    });
  }

  static async getInjectionHistory() {
    return apiRequest('/Injection/history');
  }

  static async getInjectionStatus() {
    return apiRequest('/Injection/status');
  }

  // Movement API Methods
  static async executeMovement(movementData) {
    return apiRequest('/Movement/move', {
      method: 'POST',
      body: JSON.stringify({
        direction: movementData.direction,
        distance: movementData.distance,
        speed: movementData.speed || 1.0,
        ...movementData
      }),
    });
  }

  static async homeAxes() {
    return apiRequest('/Movement/home', {
      method: 'POST',
    });
  }

  static async getMovementHistory() {
    return apiRequest('/Movement/history');
  }

  static async getCurrentPosition() {
    return apiRequest('/Movement/position');
  }

  // Utility Methods
  static async testConnection() {
    try {
      await apiRequest('/Injection/history');
      console.log('✅ API Connection Test: SUCCESS');
      return true;
    } catch (error) {
      console.error('❌ API Connection Test: FAILED', error);
      return false;
    }
  }
}

// Export for easy importing
export { ApiManager };
```

---

## 📋 API Endpoints Reference

### Scanner Controller (`/api/Scanner`)
- **POST** `/Scanner/start` - Start a scan operation (sets status to InProgress)
  ```json
  {
    "scanType": "full",
    "targetArea": "injection_site"
  }
  ```
  **Response:**
  ```json
  {
    "success": true,
    "message": "Scan started successfully",
    "scanId": 123,
    "status": "InProgress"
  }
  ```

- **POST** `/Scanner/stop` - Stop a scan and provide depth measurement (changes status to Success)
  ```json
  {
    "scanId": 123,
    "depthMeasurement": 15.5
  }
  ```
  **Response:**
  ```json
  {
    "success": true,
    "message": "Scan stopped successfully",
    "scanId": 123,
    "depthMeasurement": 15.5,
    "status": "Success"
  }
  ```

- **GET** `/Scanner/history` - Get all scan results

### Injection Controller (`/api/Injection`)
- **POST** `/Injection/start` - Start injection process
  ```json
  {
    "dosage": 0.1,
    "medicationType": "vaccine",
    "targetLocation": { "x": 10.5, "y": 15.2, "z": 8.0 }
  }
  ```

- **POST** `/Injection/stop` - Stop injection process
- **GET** `/Injection/history` - Get injection history
- **GET** `/Injection/status` - Get current injection status

### Movement Controller (`/api/Movement`)
- **POST** `/Movement/move` - Execute movement command
  ```json
  {
    "direction": "x",
    "distance": 10.5,
    "speed": 1.5
  }
  ```

- **POST** `/Movement/home` - Return to home position
- **GET** `/Movement/history` - Get movement history
- **GET** `/Movement/position` - Get current position

---

## 🧪 Testing Your Integration

### Step 1: Start the Backend API
Open PowerShell and navigate to your API project:

```powershell
cd "d:\4T2\Graduation\OVOVAX  Solution\OVOVAX.API"
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7268`
- HTTP: `http://localhost:5243`
- Swagger: `https://localhost:7268/swagger/index.html`

### Step 2: Accept HTTPS Certificate
1. Open your browser and go to `https://localhost:7268`
2. If you see a security warning, click "Advanced"
3. Click "Proceed to localhost (unsafe)"
4. You should see the API response or Swagger page

### Step 3: Test API Connection in Browser Console

Open your React app at `http://localhost:5173/GraduationProject/#/` and run this test script in the browser console:

```javascript
// OVOVAX API Connection Test
async function testOvovaxAPI() {
  const API_BASE_URL = 'https://localhost:7268/api';
  
  console.log('🔄 Testing OVOVAX API Connection...');
  
  try {
    // Test basic connectivity
    const response = await fetch(`${API_BASE_URL}/Injection/history`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include'
    });
    
    console.log('✅ Response Status:', response.status);
    
    if (response.ok) {
      const data = await response.json();
      console.log('✅ API Data:', data);
      console.log('🎉 SUCCESS: API is working correctly!');
      return true;
    } else {
      console.log('⚠️ API responded with error status:', response.status);
      return false;
    }
    
  } catch (error) {
    console.error('❌ API Connection Failed:', error);
    
    if (error.message.includes('CORS')) {
      console.log('🔧 CORS Issue - Check backend CORS configuration');
    } else if (error.message.includes('fetch')) {
      console.log('🔧 Network Issue - Check if API is running at https://localhost:7268');
    }
    
    return false;
  }
}

// Run the test
testOvovaxAPI();
```

### Step 4: React Hook Example

Create a custom hook for easy API integration:

```javascript
import { useState, useEffect } from 'react';
import { ApiManager } from './ApiManager';

export const useOvovaxApi = () => {
  const [isConnected, setIsConnected] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const testConnection = async () => {
      const connected = await ApiManager.testConnection();
      setIsConnected(connected);
    };
    
    testConnection();
  }, []);

  const executeApiCall = async (apiFunction, ...args) => {
    setLoading(true);
    setError(null);
    
    try {
      const result = await apiFunction(...args);
      setLoading(false);
      return result;
    } catch (err) {
      setError(err.message);
      setLoading(false);
      throw err;
    }
  };

  return {
    isConnected,
    loading,
    error,
    executeApiCall,
      // Scanner methods
    performScan: (data) => executeApiCall(ApiManager.performScan, data),
    stopScan: (scanId, depthMeasurement) => executeApiCall(ApiManager.stopScan, scanId, depthMeasurement),
    getScanResults: () => executeApiCall(ApiManager.getScanResults),
    
    // Injection methods
    startInjection: (data) => executeApiCall(ApiManager.startInjection, data),
    stopInjection: () => executeApiCall(ApiManager.stopInjection),
    getInjectionHistory: () => executeApiCall(ApiManager.getInjectionHistory),
    getInjectionStatus: () => executeApiCall(ApiManager.getInjectionStatus),
    
    // Movement methods
    executeMovement: (data) => executeApiCall(ApiManager.executeMovement, data),
    homeAxes: () => executeApiCall(ApiManager.homeAxes),
    getMovementHistory: () => executeApiCall(ApiManager.getMovementHistory),
    getCurrentPosition: () => executeApiCall(ApiManager.getCurrentPosition),
  };
};
```

---

## 🔍 CORS Configuration

The backend is already configured with CORS to accept requests from your frontend:

```csharp
// Allowed Origins (already configured in backend)
"http://localhost:5173"
"http://localhost:3000"  
"https://localhost:5173"
"https://localhost:3000"

// Methods: All (GET, POST, PUT, DELETE, etc.)
// Headers: All
// Credentials: Enabled
```

---

## 🐛 Troubleshooting

### Common Issues and Solutions

#### ❌ "Failed to fetch" Error
**Causes:**
- HTTPS certificate not accepted
- API server not running
- Wrong API URL

**Solutions:**
1. Go to `https://localhost:7268` and accept the certificate
2. Ensure API is running with `dotnet run`
3. Check console for specific error messages

#### ❌ CORS Errors
**Causes:**
- Wrong origin in frontend
- Missing credentials in requests

**Solutions:**
1. Ensure frontend runs on `http://localhost:5173`
2. Use `credentials: 'include'` in all fetch requests
3. Check backend CORS configuration

#### ❌ "net::ERR_CERT_AUTHORITY_INVALID"
**Cause:** Development HTTPS certificate not trusted

**Solution:** 
1. Navigate to `https://localhost:7268` in browser
2. Click "Advanced" → "Proceed to localhost (unsafe)"

#### ❌ API Returns 404
**Causes:**
- Wrong endpoint URL
- Controller not registered

**Solutions:**
1. Check endpoint spelling and casing
2. Use correct controller names: `Scanner`, `Injection`, `Movement`
3. Verify API is running on correct port

---

## 🚀 Production Deployment

### Environment Configuration
```javascript
// Production API configuration
const getApiBaseUrl = () => {
  if (process.env.NODE_ENV === 'production') {
    return 'https://your-api-domain.com/api';
  }
  return 'https://localhost:7268/api';
};
```

### Updated CORS for Production
Update the backend CORS policy in `Program.cs`:
```csharp
policy.WithOrigins(
  "http://localhost:5173",
  "https://your-frontend-domain.com",
  "https://your-production-domain.com"
)
```

---

## 📊 API Data Models

### Scanner Models
```typescript
interface ScanRequest {
  scanType?: string;
  targetArea?: string;
}

interface StopScanRequest {
  scanId: number;
  depthMeasurement: number;
}

interface ScanResponse {
  success: boolean;
  message: string;
  scanId?: number;
  depthMeasurement?: number;
  status?: 'InProgress' | 'Success' | 'Failed';
}

interface ScanResult {
  id: number;
  scanTime: string;
  depthMeasurement: number;
  status: 'InProgress' | 'Success' | 'Failed';
}
```

### Injection Models
```typescript
interface InjectionRequest {
  dosage: number;
  medicationType: string;
  targetLocation: {
    x: number;
    y: number;
    z: number;
  };
}

interface InjectionStatus {
  isActive: boolean;
  currentDosage: number;
  remainingTime: number;
}
```

### Movement Models
```typescript
interface MovementRequest {
  direction: 'x' | 'y' | 'z';
  distance: number;
  speed: number;
}

interface Position {
  x: number;
  y: number;
  z: number;
}
```

---

## ✅ Quick Setup Checklist

- [ ] Update `.env` file with HTTPS API URL
- [ ] Replace `ApiManager.js` with the complete implementation
- [ ] Start backend API with `dotnet run`
- [ ] Accept HTTPS certificate in browser
- [ ] Test API connection in browser console
- [ ] Import and use `useOvovaxApi` hook in React components
- [ ] Check browser Network tab for successful API calls

---

*This guide consolidates all previous OVOVAX frontend integration documentation into a single, comprehensive resource.*

**Last Updated:** June 9, 2025  
**API Version:** HTTPS-enabled with proper CORS configuration  
**Frontend Compatibility:** React + Vite development server
