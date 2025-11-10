using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
//using Unity.EventSystems;
using Unity.UI;
using System;


public class DialogueManager : MonoBehaviour
{
    //Singleton pattern
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    private PlayerController playerController;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject dialogueChoices;

    [SerializeField] private GameObject textObject;
    public TextMeshProUGUI dialogueText;

    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    // Comes form ink, based on an inkJSON file
    private Story currentStory;

    // Is the system waiting for a dialogue choice to be made?
    public bool isWaiting = false;
    // Is the dialogue system currently playing?
    public bool isPlaying = false;
    // canClick exists because when exiting choices, both makechoice and update want to
    //public bool canClick = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();


        dialogueBox.SetActive(false);
        dialogueChoices.SetActive(false);
        dialogueText.text = string.Empty;

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying)
        {
            return;
        }
        if (isPlaying && !isWaiting && playerController.clickAction.WasPressedThisFrame())
        {
            ContinueStory();
        }
    }

    public void StartQuestionDialogue(TextAsset inkJSON, string knotName)
    {
        dialogueBox.SetActive(true);
        currentStory = new Story(inkJSON.text);
        currentStory.ChoosePathString(knotName);
        isPlaying = true;

        ContinueStory();
    }

    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayAnswers();
        }
        else
        {
            EndQuestionDialogue();
        }
    }

    void EndQuestionDialogue()
    {
        dialogueBox.SetActive(false);
        isPlaying = false;
        dialogueText.text = "";
    }

    void DisplayAnswers()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count <= 0)
        {
            // There are no choices to display
            return;
        }
        else if (currentChoices.Count > choices.Length)
        {
            Debug.Log("There are more answeres given than the UI can handle (> 4)");
        }

        textObject.SetActive(false);
        dialogueChoices.SetActive(true);
        isWaiting = true;

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            // Make the question in the UI visible
            choices[index].gameObject.SetActive(true);
            // Set text of corresponding choice
            choicesText[index].text = choice.text;
            index++;
        }

        if (index < 4)
        {
            while (index < 4)
            {
                choices[index].gameObject.SetActive(false);
                choicesText[index].text = string.Empty;
                index++;
            }
        }
        
    }
    public void MakeChoice(int choiceIndex)
    {
        Debug.Log("A choice has been made");
        dialogueChoices.SetActive(false);
        currentStory.ChooseChoiceIndex(choiceIndex);
        isWaiting = false;
        textObject.SetActive(true);
        ContinueStory();
    }

    public void SetVar(string var, bool value)
    {
        // Sets a variable inside ink from unity
        currentStory.variablesState[var] = (object)value;
    }
    
}