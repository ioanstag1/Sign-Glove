import pandas as pd
import matplotlib.pyplot as plt
import glob
from scipy.signal import medfilt

gesture = input("Enter gesture folder name (e.g. REST, fist): ")

folder = f"D:/ΕΜΜΒΙΟΜΕ/3rd Semester - IASI/Assistive Devices/dataset/{gesture}/*.csv"
files = glob.glob(folder)

plt.figure(figsize=(12, 6))

# Plot INDEX in blue (smoothed)
for f in files:
    df = pd.read_csv(f)
    index_smoothed = medfilt(df["index"], kernel_size=5)
    plt.plot(index_smoothed, alpha=0.5, color="blue",
             label="Index (smoothed)" if f == files[0] else "")

# Plot MIDDLE in red (smoothed)
for f in files:
    df = pd.read_csv(f)
    middle_smoothed = medfilt(df["middle"], kernel_size=5)
    plt.plot(middle_smoothed, alpha=0.5, color="red",
             label="Middle (smoothed)" if f == files[0] else "")

plt.title(f"All trials for gesture (smoothed): {gesture}")
plt.xlabel("Samples")
plt.ylabel("Sensor Value")
plt.legend()
plt.tight_layout()
plt.show()
