import pandas as pd
import numpy as np
import glob
import os
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
import joblib

base = "D:/ΕΜΜΒΙΟΜΕ/3rd Semester - IASI/Assistive Devices/dataset/"

gestures = ["rest", "index_point", "middle_point", "fist"]

X = []
y = []

for participant in os.listdir(base):
    part_path = base + participant
    if not os.path.isdir(part_path):
        continue

    for gesture in gestures:
        folder = f"{part_path}/{gesture}/*.csv"
        files = glob.glob(folder)

        for f in files:
            df = pd.read_csv(f)

            # Features: simple descriptive statistics
            features = [
                df["index"].mean(),
                df["index"].std(),
                df["index"].min(),
                df["index"].max(),
                df["middle"].mean(),
                df["middle"].std(),
                df["middle"].min(),
                df["middle"].max()
            ]

            X.append(features)
            y.append(gesture)

X = np.array(X)
y = np.array(y)

# Train test split
X_train, X_test, y_train, y_test = train_test_split(
    X, y, test_size=0.2, random_state=42, stratify=y
)

clf = RandomForestClassifier(n_estimators=300, random_state=42)
clf.fit(X_train, y_train)

y_pred = clf.predict(X_test)
print(classification_report(y_test, y_pred))

# Save the model
joblib.dump(clf, "gesture_model.pkl")
print("Saved model to gesture_model.pkl")
