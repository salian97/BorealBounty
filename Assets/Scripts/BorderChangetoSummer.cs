using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderChangeScene : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScreenBorder"))
        {
            // Load next scene by name (replace with your scene's name)
            SceneManager.LoadScene("Summer");
        }
    }
}
