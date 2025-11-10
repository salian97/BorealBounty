using UnityEngine;

public class SquirrelNPCInteract : MonoBehaviour
{
    public GameObject dialogueUI; // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC")) {
            dialogueUI.SetActive(true); // Show dialogue UI
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC")) {
            dialogueUI.SetActive(false); // Hide dialogue UI
        }
    }
}
