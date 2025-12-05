import serial
import json
import csv
import time
import os
from pathlib import Path

# Settings
duration = 5   # seconds to record each trial
num_trials = 10
prep_time = 3  # seconds countdown before each trial
rest_between = 2  # seconds between trials

base_path = Path("D:/ΕΜΜΒΙΟΜΕ/3rd Semester - IASI/Assistive Devices/dataset/Nasser")

# Open serial
ser = serial.Serial("COM11", 9600)
ser.reset_input_buffer()

gesture = input("Enter gesture name (rest, index_point, middle_point, fist): ").strip()
gesture_folder = base_path / gesture
gesture_folder.mkdir(parents=True, exist_ok=True)

print(f"\nPreparing to record gesture: {gesture}")
print(f"Total trials: {num_trials}")
print("---------------------------------------")

for trial in range(1, num_trials + 1):

    # Create file path
    file_path = gesture_folder / f"{gesture}_{trial:02d}.csv"

    # Open CSV for this trial
    csv_file = open(file_path, "w", newline="")
    writer = csv.writer(csv_file)
    writer.writerow(["index", "middle"])

    # Countdown before starting trial
    print(f"\nTrial {trial}/{num_trials} starting in...")
    for s in range(prep_time, 0, -1):
        print(s, "...")
        time.sleep(1)

    print(f">>> Recording {gesture} | Trial {trial} for {duration} seconds...")

    start_time = time.time()

    # Recording loop for this trial
    while True:
        if time.time() - start_time >= duration:
            print(f"Trial {trial} complete. Saved: {file_path}")
            csv_file.close()
            break

        line = ser.readline().decode(errors="ignore").strip()
        if not line.startswith("{"):
            continue

        data = json.loads(line)

        index_val = data["index"]
        middle_val = data["middle"]

        print(f"Index: {index_val}   Middle: {middle_val}")
        writer.writerow([index_val, middle_val])

    print(f"Resting {rest_between} seconds before next trial...\n")
    time.sleep(rest_between)

print("\n---------------------------------------")
print("ALL TRIALS COMPLETE!")
print(f"Saved under: {gesture_folder}")
