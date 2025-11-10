using UnityEngine;
using UnityEngine.SceneManagement;

public class SquirrelBattlePortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save position and scene name
            BattleReturnData.lastPosition = other.transform.position;
            BattleReturnData.lastScene = SceneManager.GetActiveScene().name;
            // Switch to battle scene
            SceneManager.LoadScene("BattlePhase"); // Replace "BattleScene" with your battle scene name
        }
    }
}
