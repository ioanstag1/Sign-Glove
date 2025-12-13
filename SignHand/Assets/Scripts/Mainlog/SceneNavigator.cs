using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Φρόντισε τα ονόματα να ταιριάζουν με τα πραγματικά Scene names
    private const string SCENE_MAIN_MENU = "MainMenu";
    private const string SCENE_GESTURE   = "SignerScene";    // USE GLOVE
    private const string SCENE_CHAT      = "ListenerScene";  // USE TEXT / VOICE
    private const string SCENE_LEARN     = "LearnScene";     // LEARN SIGNS
    private const string SCENE_TEST      = "TestScene";

    public void GoToMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene(SCENE_MAIN_MENU);
    }

    public void GoToGestureMode()
    {
        Debug.Log("Loading Gesture Mode...");
        SceneManager.LoadScene(SCENE_GESTURE);
    }

    public void GoToChatMode()
    {
        Debug.Log("Loading Chat Mode...");
        SceneManager.LoadScene(SCENE_CHAT);
    }

    public void GoToLearnMode()
    {
        Debug.Log("Loading Learn Mode...");
        SceneManager.LoadScene(SCENE_LEARN);
    }

    public void GoToTestMode()
    {
        Debug.Log("Loading Test Mode...");
        SceneManager.LoadScene(SCENE_TEST);
    }

    public void QuitApp()
    {
        Debug.Log("Quitting Application...");
        Application.Quit();
    }
}
