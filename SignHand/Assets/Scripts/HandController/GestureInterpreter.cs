using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInterpreter : MonoBehaviour
{
    public GestureController gestures;

    // public UnityWebSync webSync;   // ❌ Disabled (Web removed)

    string currentText = "";

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentText.Length > 0)
                    currentText = currentText.Substring(0, currentText.Length - 1);
            }
            else if (c != '\n' && c != '\r')
            {
                currentText += c;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Interpret(currentText.ToLower());
            currentText = "";
        }
    }

    void Interpret(string cmd)
    {
        Debug.Log("Command: " + cmd);

        if (cmd == "rest")
        {
            gestures.DoRest();
            // webSync.SendGestureToWeb("rest");   // ❌ Removed
        }
        else if (cmd == "fist")
        {
            gestures.DoFist();
            // webSync.SendGestureToWeb("fist");   // ❌ Removed
        }
        else if (cmd == "index" || cmd == "index_point")
        {
            gestures.DoIndexPoint();
            // webSync.SendGestureToWeb("index");  // ❌ Removed
        }
        else if (cmd == "middle" || cmd == "middle_point")
        {
            gestures.DoMiddlePoint();
            // webSync.SendGestureToWeb("middle"); // ❌ Removed
        }
        else
        {
            Debug.Log("Unknown command: " + cmd);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 30), "Type: rest, fist, index, middle + ENTER");
        GUI.Label(new Rect(10, 40, 500, 30), "Input: " + currentText);
    }
}
