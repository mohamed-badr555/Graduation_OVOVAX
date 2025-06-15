// OVOVAX API Test Script - Complete Testing Suite
// Open this in a browser console to test your HTTPS API connection
// Updated to match the consolidated OVOVAX Integration Guide

async function testOvovaxAPI() {
  const API_BASE_URL = 'https://localhost:7268/api';
  
  console.log('🚀 OVOVAX API Complete Test Suite Starting...');
  console.log('📡 Base URL:', API_BASE_URL);
  
  const results = {
    injection: { tested: false, working: false },
    scanner: { tested: false, working: false },
    movement: { tested: false, working: false }
  };
  
  try {
    // Test Injection Endpoints
    console.log('\n🔹 Testing Injection Controller...');
    
    // Test Injection History (GET)
    try {
      const injectionResponse = await fetch(`${API_BASE_URL}/Injection/history`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
      });
      results.injection.tested = true;
      results.injection.working = injectionResponse.ok;
      console.log(`  ✓ /Injection/history - Status: ${injectionResponse.status}`);
      
      if (injectionResponse.ok) {
        const data = await injectionResponse.json();
        console.log(`  📊 Data:`, data);
      }
    } catch (err) {
      console.log(`  ❌ /Injection/history failed:`, err.message);
    }
    
    // Test Injection Status (GET)
    try {
      const statusResponse = await fetch(`${API_BASE_URL}/Injection/status`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
      });
      console.log(`  ✓ /Injection/status - Status: ${statusResponse.status}`);
    } catch (err) {
      console.log(`  ❌ /Injection/status failed:`, err.message);
    }
      // Test Scanner Endpoints
    console.log('\n🔹 Testing Scanner Controller...');
    
    let scanId = null;
    
    // Test Start Scan (POST)
    try {
      const startScanResponse = await fetch(`${API_BASE_URL}/Scanner/start`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
          scanType: 'test',
          targetArea: 'test area'
        })
      });
      console.log(`  ✓ /Scanner/start - Status: ${startScanResponse.status}`);
      
      if (startScanResponse.ok) {
        const startData = await startScanResponse.json();
        console.log(`  📊 Start Scan Data:`, startData);
        scanId = startData.scanId;
        console.log(`  🆔 Scan ID for testing: ${scanId}`);
      }
    } catch (err) {
      console.log(`  ❌ /Scanner/start failed:`, err.message);
    }
    
    // Test Scanner History (GET)
    try {
      const scannerResponse = await fetch(`${API_BASE_URL}/Scanner/history`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
      });
      results.scanner.tested = true;
      results.scanner.working = scannerResponse.ok;
      console.log(`  ✓ /Scanner/history - Status: ${scannerResponse.status}`);
      
      if (scannerResponse.ok) {
        const data = await scannerResponse.json();
        console.log(`  📊 History Data:`, data);
      }
    } catch (err) {
      console.log(`  ❌ /Scanner/history failed:`, err.message);
    }
    
    // Test Stop Scan (POST) - only if we have a scan ID
    if (scanId) {
      try {
        const stopScanResponse = await fetch(`${API_BASE_URL}/Scanner/stop`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          credentials: 'include',
          body: JSON.stringify({
            scanId: scanId,
            depthMeasurement: 15.75 // Test depth measurement in mm
          })
        });
        console.log(`  ✓ /Scanner/stop - Status: ${stopScanResponse.status}`);
        
        if (stopScanResponse.ok) {
          const stopData = await stopScanResponse.json();
          console.log(`  📊 Stop Scan Data:`, stopData);
          console.log(`  🎯 Final Status: ${stopData.status}`);
          console.log(`  📏 Depth Measurement: ${stopData.depthMeasurement}mm`);
        }
      } catch (err) {
        console.log(`  ❌ /Scanner/stop failed:`, err.message);
      }
    } else {
      console.log(`  ⚠️ Skipping /Scanner/stop test - no scan ID available`);
    }
    
    // Test Movement Endpoints
    console.log('\n🔹 Testing Movement Controller...');
    
    try {
      const movementResponse = await fetch(`${API_BASE_URL}/Movement/history`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
      });
      results.movement.tested = true;
      results.movement.working = movementResponse.ok;
      console.log(`  ✓ /Movement/history - Status: ${movementResponse.status}`);
      
      if (movementResponse.ok) {
        const data = await movementResponse.json();
        console.log(`  📊 Data:`, data);
      }
    } catch (err) {
      console.log(`  ❌ /Movement/history failed:`, err.message);
    }
    
    // Test Current Position
    try {
      const positionResponse = await fetch(`${API_BASE_URL}/Movement/position`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
      });
      console.log(`  ✓ /Movement/position - Status: ${positionResponse.status}`);
    } catch (err) {
      console.log(`  ❌ /Movement/position failed:`, err.message);
    }
    
    // Summary
    console.log('\n📊 TEST RESULTS SUMMARY:');
    console.log('========================');
    
    const workingEndpoints = Object.values(results).filter(r => r.tested && r.working).length;
    const totalTested = Object.values(results).filter(r => r.tested).length;
    
    console.log(`✅ Working Controllers: ${workingEndpoints}/${totalTested}`);
    console.log(`🔹 Injection: ${results.injection.working ? '✅ Working' : '❌ Failed'}`);
    console.log(`🔹 Scanner: ${results.scanner.working ? '✅ Working' : '❌ Failed'}`);
    console.log(`🔹 Movement: ${results.movement.working ? '✅ Working' : '❌ Failed'}`);
    
    if (workingEndpoints === totalTested && totalTested > 0) {
      console.log('\n🎉 SUCCESS: All OVOVAX API endpoints are working correctly!');
      console.log('🚀 Your frontend can now integrate with the API using the consolidated guide.');
    } else {
      console.log('\n⚠️ Some endpoints may need attention. Check the individual results above.');
    }
    
    return workingEndpoints === totalTested;
    
  } catch (error) {
    console.error('\n❌ API Test Suite Failed:', error);
    
    if (error.message.includes('CORS')) {
      console.log('🔧 CORS Issue - Check backend CORS configuration');
    } else if (error.message.includes('fetch')) {
      console.log('🔧 Network Issue - Check if API is running at https://localhost:7268');
      console.log('💡 Make sure to:');
      console.log('   1. Run "dotnet run" in the OVOVAX.API project');
      console.log('   2. Accept the HTTPS certificate when prompted');
      console.log('   3. Visit https://localhost:7268 and accept security warning');
    }
    
    return false;
  }
}

// Auto-run the test suite
console.log('🚀 Starting OVOVAX API Test Suite...');
testOvovaxAPI();

// Quick reference for manual testing
console.log('\n📋 OVOVAX API Endpoints Reference:');
console.log('==================================');
console.log('📊 Scanner:');
console.log('  POST /api/Scanner/start    - Start scan (status: InProgress)');
console.log('  POST /api/Scanner/stop     - Stop scan with depth measurement (status: Success)');
console.log('  GET  /api/Scanner/history  - Get scan history');
console.log('💉 Injection:');
console.log('  GET  /api/Injection/history');
console.log('  GET  /api/Injection/status');
console.log('  POST /api/Injection/start');
console.log('  POST /api/Injection/stop');
console.log('🤖 Movement:');
console.log('  GET  /api/Movement/history');
console.log('  GET  /api/Movement/position');
console.log('  POST /api/Movement/move');
console.log('  POST /api/Movement/home');
console.log('\n🌐 Swagger UI: https://localhost:7268/swagger/index.html');
console.log('\n💡 Scanner Workflow:');
console.log('  1. POST /Scanner/start → Returns scanId, status: "InProgress"');
console.log('  2. POST /Scanner/stop + {scanId, depthMeasurement} → status: "Success"');
