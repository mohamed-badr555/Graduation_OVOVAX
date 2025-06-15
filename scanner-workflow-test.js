// OVOVAX Scanner Workflow Test
// Test the new Start/Stop scanner functionality

async function testScannerWorkflow() {
  const API_BASE_URL = 'https://localhost:7268/api';
  
  console.log('🔬 Testing OVOVAX Scanner Workflow...\n');
  
  try {
    // Step 1: Start a scan
    console.log('1️⃣ Starting scan...');
    const startResponse = await fetch(`${API_BASE_URL}/Scanner/start`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({}) // Empty body as per ScanRequestDto
    });
    
    if (!startResponse.ok) {
      throw new Error(`Start scan failed: ${startResponse.status}`);
    }
    
    const startData = await startResponse.json();
    console.log('✅ Scan started successfully!');
    console.log(`   Scan ID: ${startData.scanId}`);
    console.log(`   Status: ${startData.status || 'InProgress'}`);
    console.log(`   Message: ${startData.message}\n`);
    
    // Step 2: Check scan history
    console.log('2️⃣ Checking scan history...');
    const historyResponse = await fetch(`${API_BASE_URL}/Scanner/history`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include'
    });
    
    if (historyResponse.ok) {
      const historyData = await historyResponse.json();
      console.log('✅ History retrieved successfully!');
      console.log(`   Found ${historyData.length} scan(s)`);
      
      // Find our current scan
      const currentScan = historyData.find(scan => scan.id === startData.scanId);
      if (currentScan) {
        console.log(`   Current scan status: ${currentScan.status}`);
        console.log(`   Current depth: ${currentScan.depthMeasurement}mm\n`);
      }
    }
    
    // Step 3: Stop the scan with depth measurement
    console.log('3️⃣ Stopping scan with depth measurement...');
    const depthMeasurement = 23.45; // Test depth in mm
    
    const stopResponse = await fetch(`${API_BASE_URL}/Scanner/stop`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({
        scanId: startData.scanId,
        depthMeasurement: depthMeasurement
      })
    });
    
    if (!stopResponse.ok) {
      const errorText = await stopResponse.text();
      throw new Error(`Stop scan failed: ${stopResponse.status} - ${errorText}`);
    }
    
    const stopData = await stopResponse.json();
    console.log('✅ Scan stopped successfully!');
    console.log(`   Scan ID: ${stopData.scanId}`);
    console.log(`   Final Status: ${stopData.status}`);
    console.log(`   Depth Measurement: ${stopData.depthMeasurement}mm`);
    console.log(`   Message: ${stopData.message}\n`);
    
    // Step 4: Verify the final state in history
    console.log('4️⃣ Verifying final state...');
    const finalHistoryResponse = await fetch(`${API_BASE_URL}/Scanner/history`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include'
    });
    
    if (finalHistoryResponse.ok) {
      const finalHistoryData = await finalHistoryResponse.json();
      const finalScan = finalHistoryData.find(scan => scan.id === startData.scanId);
      
      if (finalScan) {
        console.log('✅ Final verification successful!');
        console.log(`   Final Status: ${finalScan.status}`);
        console.log(`   Final Depth: ${finalScan.depthMeasurement}mm`);
        console.log(`   Scan Time: ${finalScan.scanTime}`);
      }
    }
    
    console.log('\n🎉 Scanner workflow test completed successfully!');
    console.log('📊 Summary:');
    console.log(`   • Scan ID: ${startData.scanId}`);
    console.log(`   • Initial Status: InProgress`);
    console.log(`   • Final Status: ${stopData.status}`);
    console.log(`   • Depth Measurement: ${stopData.depthMeasurement}mm`);
    
    return true;
    
  } catch (error) {
    console.error('❌ Scanner workflow test failed:', error);
    return false;
  }
}

// Test error scenarios
async function testScannerErrorScenarios() {
  const API_BASE_URL = 'https://localhost:7268/api';
  
  console.log('\n🧪 Testing Scanner Error Scenarios...\n');
  
  try {
    // Test 1: Stop non-existent scan
    console.log('1️⃣ Testing stop with invalid scan ID...');
    const invalidStopResponse = await fetch(`${API_BASE_URL}/Scanner/stop`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({
        scanId: 99999,
        depthMeasurement: 10.5
      })
    });
    
    if (invalidStopResponse.status === 404) {
      console.log('✅ Correctly returned 404 for non-existent scan');
    } else {
      console.log(`⚠️ Expected 404, got ${invalidStopResponse.status}`);
    }
    
    // Test 2: Try to stop already completed scan
    console.log('\n2️⃣ Testing stop on already completed scan...');
    
    // First start a scan
    const startResponse = await fetch(`${API_BASE_URL}/Scanner/start`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({})
    });
    
    if (startResponse.ok) {
      const startData = await startResponse.json();
      
      // Stop it once
      await fetch(`${API_BASE_URL}/Scanner/stop`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
          scanId: startData.scanId,
          depthMeasurement: 15.0
        })
      });
      
      // Try to stop it again
      const doubleStopResponse = await fetch(`${API_BASE_URL}/Scanner/stop`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
          scanId: startData.scanId,
          depthMeasurement: 20.0
        })
      });
      
      if (doubleStopResponse.status === 400) {
        console.log('✅ Correctly prevented stopping already completed scan');
      } else {
        console.log(`⚠️ Expected 400, got ${doubleStopResponse.status}`);
      }
    }
    
    console.log('\n✅ Error scenario testing completed!');
    
  } catch (error) {
    console.error('❌ Error scenario testing failed:', error);
  }
}

// Run both tests
console.log('🚀 Starting OVOVAX Scanner Tests...');
testScannerWorkflow().then(success => {
  if (success) {
    testScannerErrorScenarios();
  }
});
