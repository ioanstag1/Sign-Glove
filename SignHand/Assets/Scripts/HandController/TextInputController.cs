using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInputController : MonoBehaviour
{
    public GestureController gestureController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))   // when user presses Enter
        {
            string input = currentInput.ToLower();

            if (input == "rest")
                gestureController.DoRest();
            else if (input == "fist")
                gestureController.DoFist();
            else if (input == "index" || input == "index_point")
                gestureController.DoIndexPoint();
            else if (input == "middle" || input == "middle_point")
                gestureController.DoMiddlePoint();
            else
                Debug.Log("Unknown command: " + input);

            currentInput = "";
        }
    }

    private string currentInput = "";

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Type a gesture (rest, fist, index, middle):");
        currentInput = GUI.TextField(new Rect(10, 30, 200, 20), currentInput);
    }
}
