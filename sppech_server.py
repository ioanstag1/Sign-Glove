import serial
import json
import numpy as np
import joblib
import socket
from collections import deque
import time

# -------------------- SERIAL --------------------
ser = serial.Serial("COM11", 9600)
ser.reset_input_buffer()

# -------------------- LOAD MODEL --------------------
clf = joblib.load("best_model.joblib")

# -------------------- UDP TO UNITY --------------------
UDP_IP = "127.0.0.1"
UDP_PORT = 5005
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# sliding windows
win_index = deque(maxlen=30)
win_middle = deque(maxlen=30)

# prediction smoothing
prediction_buffer = deque(maxlen=5)

print("Real-time gesture recognition started.\n")

def extract_features(values):
    arr = np.array(values)
    return [arr.mean(), arr.std(), arr.min(), arr.max()]

# ---- LIMIT PREDICTIONS ----
PREDICTION_INTERVAL = 0.2
last_pred_time = time.time()

# ---- STORE LAST VALID SENSOR VALUES ----
prev_idx = None
prev_mid = None

while True:
    try:
        line = ser.readline().decode(errors="ignore").strip()
        if not line.startswith("{"):
            continue
        
        data = json.loads(line)

        raw_idx = data["index"]
        raw_mid = data["middle"]

        # Replace 1023 with previous values
        idx = raw_idx if raw_idx != 1023 else prev_idx
        mid = raw_mid if raw_mid != 1023 else prev_mid

        if idx is None or mid is None:
            continue

        prev_idx = idx
        prev_mid = mid

        win_index.append(idx)
        win_middle.append(mid)

        if len(win_index) < 10:
            continue

        # Limit prediction rate
        if time.time() - last_pred_time < PREDICTION_INTERVAL:
            continue
        last_pred_time = time.time()

        feat = [
            *extract_features(win_index),
            *extract_features(win_middle)
        ]

        X = np.array(feat).reshape(1, -1)
        pred = clf.predict(X)[0]

        print("Predicted:", pred)

        # ------------------------
        # MAJORITY VOTE BUFFER
        # ------------------------
        prediction_buffer.append(pred)

        if len(prediction_buffer) == 5:
            most_common = max(set(prediction_buffer), key=prediction_buffer.count)
            count = prediction_buffer.count(most_common)

            if count >= 3:
                print(">>> Gesture Activated:", most_common)
                sock.sendto(most_common.encode(), (UDP_IP, UDP_PORT))

            prediction_buffer.clear()
            print("-" * 40)

    except KeyboardInterrupt:
        print("Stopped.")
        break
