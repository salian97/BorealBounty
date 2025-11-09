using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Ink.Runtime;

public class PlayerController : MonoBehaviour
{
    public InputAction clickAction;
    public InputAction mouseAction;
    [SerializeField] private TextAsset inkJSON;
    private string knotName;

    // Start is called before the first frame update
    void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        mouseAction = InputSystem.actions.FindAction("Point");
        knotName = "TestDialogue";
    }

    void Update()
    {
        if (!DialogueManager.Instance.isPlaying && clickAction.WasPressedThisFrame())
        {
            // DialogueManager is currently not running and does not exist, and the player has clicked the screen
            DialogueManager.Instance.StartQuestionDialogue(inkJSON, knotName);
        }
    }

    // Update is called once per frame
    /*
    void Update()
    {
      if (interactAction.WasPerformedThisFrame())
        {
            // your code to respond to the first frame that the Interact action is held for enough time
        }

        if (interactAction.WasCompletedThisFrame())
        {
            // your code to respond to the frame that the Interact action is released after being held for enough time
        }  
    }
    */

}
