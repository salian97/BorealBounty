using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button buttonObj;
    public bool interactable = true;
    //private bool cursorHovering = false;
    public int buttonIndex;

    [SerializeField] private GameObject player;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
        playerController = player.GetComponent<PlayerController>();
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!!!");
        DialogueManager.Instance.MakeChoice(buttonIndex);
    }


    // Update is called once per frame
    void Update()
    {
        /*
        if (playerController.clickAction.WasPressedThisFrame())
        {
            Debug.Log("Button has been pressed!");
            if (cursorHovering)
            {
                Debug.Log("Cursor is hovering");
                DialogueManager.Instance.MakeChoice(buttonIndex);
            }
            */
    }
}


/*
public Button yourButton;

	void Start () {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		Debug.Log ("You have clicked the button!");
	}
}
*/