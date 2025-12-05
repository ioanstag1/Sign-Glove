import serial
import json
import numpy as np
import joblib

# Load trained model
clf = joblib.load("gesture_model.pkl")

ser = serial.Serial("COM11", 9600)
ser.reset_input_buffer()

print("Real-time gesture prediction started...\n")

while True:
    try:
        line = ser.readline().decode(errors="ignore").strip()
        if not line.startswith("{"):
            continue

        data = json.loads(line)

        index_val = data["index"]
        middle_val = data["middle"]

        # Estimated features for real-time (approximation)
        features = np.array([
            index_val, 0, index_val, index_val,
            middle_val, 0, middle_val, middle_val
        ]).reshape(1, -1)

        pred = clf.predict(features)[0]
        print("👉 Gesture:", pred)

    except KeyboardInterrupt:
        print("\nStopped.")
        break
