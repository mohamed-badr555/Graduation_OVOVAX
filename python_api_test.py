from flask import Flask, request, jsonify
import cv2
import numpy as np
import time
import logging
import socket

app = Flask(__name__)  # Fixed: __name__ instead of name
logging.basicConfig(level=logging.INFO)

@app.route('/api/track/detect', methods=['POST'])
def detect_track():
    try:
        print("Track detection request received")
        
        detected_texts = []
        
        detected_texts.append({
            'text': "egg_ade",
            'confidence': 1.0
        })
        print(f"Detected text: '{detected_texts}'")
        
        # Combine into track ID
        track_id = " ".join([item['text'] for item in detected_texts]).strip()
        print(f"Combined Track ID: '{track_id}'")
        
        return jsonify({
            'success': True,
            'track_id': track_id,
            'detected_texts': detected_texts,
            'timestamp': time.time()
        })
        
    except Exception as e:
        print(f"Error in detect_track: {str(e)}")
        return jsonify({
            'success': False,
            'error': str(e),
            'timestamp': time.time()
        }), 500

@app.route('/api/health', methods=['GET'])
def health_check():
    return jsonify({
        'status': 'healthy',
        'hostname': socket.gethostname(),
        'timestamp': time.time()
    })

@app.route('/status', methods=['GET'])
def status():
    return jsonify({
        'status': 'running',
        'service': 'Track Detection API',
        'hostname': socket.gethostname(),
        'timestamp': time.time()
    })

@app.route('/', methods=['GET'])
def root():
    return jsonify({
        'message': 'Track Detection API is running',
        'endpoints': [
            '/api/track/detect (POST)',
            '/api/health (GET)',
            '/status (GET)'
        ],
        'timestamp': time.time()
    })

if __name__ == '__main__':
    print("Starting Flask API server...")
    print("API will be available at: http://raspberrypi.local:5001")
    app.run(host='0.0.0.0', port=5001, debug=True)  # Changed port to 5001
