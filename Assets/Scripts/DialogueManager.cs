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
using UnityEngine.AI;


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

    [SerializeField] private BattleLogic BattleController;

    private QuestionData testQuestion;



    private GameObject currentInstance;
    public GameObject prefab;


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
    public bool battleComplete;
    private int indexChoice;
    private int mindfullness;
    private int correctAnswer = 2;

    private EEGDataAnalyzer instanceScript;

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
        battleComplete = false;

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
        //Debug.Log($"{isPlaying} {isWaiting} {battleComplete}");
        if (isPlaying && !isWaiting && playerController.clickAction.WasPressedThisFrame())
        {
            if (!battleComplete)
            {
                ContinueStory();
            }
            else
            {
                isPlaying = false;
            }
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
            StartQuestion();
        }
        else if (currentStory.canContinue)
        {
            if (currentSpeaker == "Boss")
            {
                bossDialogueBox.SetActive(true);
                bossTextObject.SetActive(true);
                string newText = currentStory.Continue();
                if (correctAnswer == 1)
                {
                    // 1 is true
                    Debug.Log("Incorrect answer!! :DD");
                    bossDialogueText.text = "The correct answer was: " + testQuestion.CorrectAnswer;
                }
                else
                {
                    bossDialogueText.text = newText;
                }
                dialogueBox.SetActive(false);
                textObject.SetActive(false);
                if (correctAnswer == 0) { correctAnswer = 1; }
                else { correctAnswer = 3; }
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
            EndQuestionDialogue();
        }
    }

    void EndQuestionDialogue()
    {
        Debug.Log("End");
        dialogueBox.SetActive(false);
        dialogueText.text = "";
        battleComplete = true;
    }

    void DisplayAnswers(bool isAnswer)
    {
        // This is here the question is called, when isAnswer is true. If isAnswer is false, this is dialogue choices and not a question.
        // Put functionality to record brain stuff here, but put it under a if (isAnswer){};
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count <= 0)
        {
            // There are no choices to display
            //if (isAnswer){ ContinueStory(); }
            return;
        }
        else if (currentChoices.Count > choices.Length)
        {
            Debug.Log("There are more answeres given than the UI can handle (> 4)");
        }
        bossTextObject.SetActive(isAnswer);

        /*
        if (isAnswer)
        {
            QuestionData question = BattleController.GetQuestion();
            FitQuestion(question);
        }
        */

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

        if (isAnswer)
        {
            testQuestion = BattleController.GetQuestion("easy");
            FitQuestion(testQuestion);
            SpawnTracker();
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        dialogueChoices.SetActive(false);
        currentStory.ChooseChoiceIndex(choiceIndex);
        if (currentlyAsking)
        {
            currentStory.Continue();
            indexChoice = GetIntVar("chosenIndex");
            Debug.Log(indexChoice);
            CompareAnswer(testQuestion);
            dialogueBox.SetActive(false);
            mindfullness = DestroyTracker();
            Debug.Log($"Mindfullness for this question: {mindfullness}");
        }
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
        return varValue;
    }

    public int GetIntVar(string varName)
    {
        int varValue = (int)currentStory.variablesState[varName];
        return varValue;
    }

    public void StartQuestion()
    {
        dialogueBox.SetActive(true);
        textObject.SetActive(true);

        bossDialogueBox.SetActive(true);
        bossTextObject.SetActive(true);
        bossDialogueText.text = currentStory.Continue();

        DisplayAnswers(true);

    }

    public void FitQuestion(QuestionData question)
    {
        if (question == null) { Debug.Log("Question is null"); }
        else if (question.Question == null) { Debug.Log("Question title is null"); }
        Debug.Log(question.Question);
        bossDialogueText.text = question.Question;

        choicesText[0].text = question.Answer1;
        choicesText[1].text = question.Answer2;
        if (question.Answer3 != null)
        {
            choicesText[2].text = question.Answer3;
            if (question.Answer4 != null)
            {
                choicesText[3].text = question.Answer4;
            }
        }
    }

    public void CompareAnswer(QuestionData question)
    {
        string toCompare = "";
        switch (indexChoice)
        {
            case 0:
                toCompare = question.Answer1;
                break;
            case 1:
                toCompare = question.Answer2;
                break;
            case 2:
                toCompare = question.Answer3;
                break;
            case 3:
                toCompare = question.Answer4;
                break;
        }
        if (toCompare == question.CorrectAnswer)
        {
            Debug.Log("Correct Answer!");
            SetVar("correctAnswer", true);
            correctAnswer = 2;
        }
        else
        {
            Debug.Log("Incorrect Answer");
            SetVar("correctAnswer", false);
            correctAnswer = 0;
        }
    }

    void SpawnTracker()
    {
        // Destroy existing instance if any
        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        // Instantiate new instance and store reference
        currentInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        Debug.Log("Prefab spawned");
    }

    public int DestroyTracker()
    {
        if (currentInstance != null)
        {
            instanceScript = currentInstance.GetComponent<EEGDataAnalyzer>();

            if (instanceScript == null)
            {
                Debug.LogError("script not found on the prefab instance");
                mindfullness = 0;
            }
            else
            {
                int mindfullness = instanceScript.GetMindfulnessScore();
            }

            Destroy(currentInstance);
            currentInstance = null; // clear the reference
            instanceScript = null;
            Debug.Log("Prefab destroyed");
        }
        return mindfullness;
    }
}