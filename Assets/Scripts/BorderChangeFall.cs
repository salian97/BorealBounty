using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderChangeSceneToFall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScreenBorder"))
        {
            SceneManager.LoadScene("Fall"); // Loads the scene named "Fall"
        }
    }
}
