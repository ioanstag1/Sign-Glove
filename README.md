# Sign Glove — Real-Time Bidirectional Gesture Communication System

A real-time two-way communication prototype that translates hand gestures 
into text and speech, and displays a shared 3D hand avatar for visual feedback.

Built as a team project during the EMMBIOME Erasmus Mundus Master's Programme (January 2025).

## What it does

- Reads sensor data from a glove (Arduino) and classifies hand gestures in real time
- Recognises 5 gestures with ~98% accuracy
- Converts recognised gestures to text and speech via a Unity chat application
- Displays a shared 3D hand avatar for both users simultaneously

## System Architecture
```
Glove Sensors (Arduino) → Python classifier → Speech server → Unity Chat App
                                                         ↕
                                               3D Hand Avatar (real-time)
```

## Stack

| Component | Technology |
|---|---|
| Gesture classifier | Python, scikit-learn |
| Real-time prediction | rt_predict.py |
| Communication server | sppech_server.py |
| 3D application | Unity (C#) |
| Hardware | Arduino + glove sensors |

## Results

- 5-class gesture recognition
- ~95% classification accuracy on collected dataset
- End-to-end latency suitable for real-time use

## Team

Built by Ioanna Stagona and Sayyidah Humairah as part of the EMMBIOME Joint Master's programme.
