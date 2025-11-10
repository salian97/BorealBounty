#nullable disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
//using Unity.EventSystems;
using Unity.UI;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class DialogueManager : MonoBehaviour
{
    //Singleton pattern
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    private PlayerController playerController;

    [Header("Main Dialogue Box")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject dialogueChoices;

    [SerializeField] private GameObject textObject;
    public TextMeshProUGUI dialogueText;

    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Boss Dialogue Box")]
    [SerializeField] private GameObject bossDialogueBox;
    [SerializeField] private GameObject bossTextObject;
    public TextMeshProUGUI bossDialogueText;


    // Comes form ink, based on an inkJSON file
    private Story currentStory;

    // Is the system waiting for a dialogue choice to be made?
    public bool isWaiting = false;
    // Is the dialogue system currently playing?
    public bool isPlaying = false;
    // canClick exists because when exiting choices, both makechoice and update want to
    //public bool canClick = false;
    public string currentSpeaker;
    private bool currentlyAsking;

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
        currentlyAsking = false;

        dialogueBox.SetActive(false);
        dialogueChoices.SetActive(false);
        dialogueText.text = string.Empty;

        bossDialogueBox.SetActive(false);
        bossDialogueText.text = string.Empty;

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
            Debug.Log("Update can be called");
            ContinueStory();
        }
    }

    public void StartQuestionDialogue(TextAsset inkJSON, string knotName)
    {
        currentStory = new Story(inkJSON.text);
        currentStory.ChoosePathString(knotName);
        currentSpeaker = GetStringVar("nextSpeaker");
        currentlyAsking = GetBoolVar("asking");
        if (!currentlyAsking)
        {
            if (currentSpeaker == "Squirrel")
            {
                dialogueBox.SetActive(true);
            }
            else if (currentSpeaker == "Boss")
            {
                bossDialogueBox.SetActive(true);
            }

            isPlaying = true;

            ContinueStory();
        }
        else
        {
            StartQuestion();
        }

    }

    void ContinueStory()
    {
        if (currentlyAsking)
        {
            Debug.Log("Currently asking!");
            StartQuestion();
        }
        else if (currentStory.canContinue)
        {
            Debug.Log($"Current speaker is {currentSpeaker}");
            if (currentSpeaker == "Boss")
            {
                bossDialogueBox.SetActive(true);
                bossTextObject.SetActive(true);
                bossDialogueText.text = currentStory.Continue();
                dialogueBox.SetActive(false);
                textObject.SetActive(false);
            }
            else if (currentSpeaker == "Squirrel")
            {
                dialogueBox.SetActive(true);
                textObject.SetActive(true);
                dialogueText.text = currentStory.Continue();
                bossDialogueBox.SetActive(false);
                bossTextObject.SetActive(false);
            }
            currentSpeaker = GetStringVar("nextSpeaker");

            currentlyAsking = GetBoolVar("asking");
            DisplayAnswers(currentlyAsking);
        }
        else
        {
            Debug.Log("Calling end QuesstionDialogue");
            currentlyAsking = GetBoolVar("asking");
            EndQuestionDialogue();
        }
    }

    void EndQuestionDialogue()
    {
        dialogueBox.SetActive(false);
        isPlaying = false;
        dialogueText.text = "";
        SceneManager.LoadScene(BattleReturnData.lastScene);
    }

    void DisplayAnswers(bool isAnswer)
    {
        // This is here the question is called, when isAnswer is true. If isAnswer is false, this is dialogue choices and not a question.
        // Put functionality to record brain stuff here, but put it under a if (isAnswer){};

        Debug.Log($"isAnswer: {isAnswer}");
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

        if (!isAnswer)
        {
            bossTextObject.SetActive(false);
        }
        else
        {
            bossTextObject.SetActive(true);
        }

        textObject.SetActive(false);
        dialogueBox.SetActive(true);
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
        if (currentlyAsking)
        {
            // End brain sensor
        }

        Debug.Log("A choice has been made");
        dialogueChoices.SetActive(false);
        currentStory.ChooseChoiceIndex(choiceIndex);
        isWaiting = false;
        currentlyAsking = false;
        textObject.SetActive(true);
        ContinueStory();
    }

    public void SetVar(string var, bool value)
    {
        // Sets a variable inside ink from unity
        currentStory.variablesState[var] = (object)value;
    }

    public string GetStringVar(string varName)
    {
        string varValue = (string)currentStory.variablesState[varName];
        return varValue;
    }

    public bool GetBoolVar(string varName)
    {
        bool varValue = (bool)currentStory.variablesState[varName];
        Debug.Log($"Currently aking {currentlyAsking}");
        return varValue;
    }

    public void StartQuestion()
    {
        Debug.Log("Start question bit");
        dialogueBox.SetActive(true);
        textObject.SetActive(true);

        bossDialogueBox.SetActive(true);
        bossTextObject.SetActive(true);
        bossDialogueText.text = currentStory.Continue();

        DisplayAnswers(true);
        


        /*
        if (currentStory.canContinue)
        {
            currentSpeaker = GetStringVar("currentSpeaker");
            if (currentSpeaker == "Boss")
            {
                bossDialogueBox.SetActive(true);
                bossDialogueText.text = currentStory.Continue();
                dialogueBox.SetActive(false);
            }
            else if (currentSpeaker == "Squirrel")
            {
                dialogueBox.SetActive(true);
                dialogueText.text = currentStory.Continue();
                bossDialogueBox.SetActive(false);
            }

            DisplayAnswers();
        }
        else
        {
            EndQuestionDialogue();
        };
        */
    }
    
}