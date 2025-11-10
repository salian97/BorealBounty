using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderChangeToEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScreenBorder"))
        {
            SceneManager.LoadScene("EndCutScene"); // end lore
        }
    }
}
