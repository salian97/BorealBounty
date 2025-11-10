using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    // Name of the main menu scene (must match exactly what's in your Build Settings)
    public string menuSceneName = "Main Menu";

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
