using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderChangeSceneToWinter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScreenBorder"))
        {
            SceneManager.LoadScene("Winter"); // Loads the scene named "Winter"
        }
    }
}
