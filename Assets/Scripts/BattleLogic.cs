#nullable disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogic : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private string knotName;
    [SerializeField] private QuestionDictionary questionDict;
    [SerializeField] private GameObject Admin;

    private QuestionData testQuestion;
    public string difficulty;

    // Start is called before the first frame update
    void Start()
    {
        if (Admin == null)
            {
                Debug.LogError("Admin GameObject is not assigned in the Inspector!");
                return;
            }
            
            questionDict = Admin.GetComponent<QuestionDictionary>();
            
            if (questionDict == null)
            {
                Debug.LogError("QuestionDictionary component not found on Admin GameObject!");
                return;
            }
        

        //Admin = GameObject.FindGameObjectWithTag("Admin");
        //questionDict = Admin.GetComponent<QuestionDictionary>();
        difficulty = "easy";
        //knotName = "TestDialogue";
    }

    // Update is called once per frame
    void Update()
    {
        if (!DialogueManager.Instance.isPlaying)
        {
            Debug.Log("Dialogue is triggered by scene start");
            DialogueManager.Instance.StartQuestionDialogue(inkJSON, knotName);
            GetQuestion();
        }
    }
    
    void GetQuestion()
    {
        if (questionDict == null)
        {
            Debug.LogError("questionDict is null");
            return;
        }
        
        Debug.Log("A random question is called!");
        testQuestion = questionDict.GetRandomQuestion(difficulty);
        

        /*
        if (QuestionDictionary.Instance != null)
        {
            QuestionData testQuestion = QuestionDictionary.Instance.GetRandomQuestion(difficulty);
            Debug.Log($"Got a question: {testQuestion.Question}");
        }
        */
    }
}