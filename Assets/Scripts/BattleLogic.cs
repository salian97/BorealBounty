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
        Admin = GameObject.FindGameObjectWithTag("Admin");
        questionDict = Admin.GetComponent<QuestionDictionary>();
        //inkJSON = 
        knotName = "Main";
        
        StartCoroutine(WaitForData());
    }

    IEnumerator WaitForData()
    {
        // Wait until questions are loaded
        while (Admin.GetComponent<ReadCSV>().QuestionsList == null || Admin.GetComponent<ReadCSV>().QuestionsList.Count == 0 || Admin.GetComponent<QuestionDictionary>().questionDict == null)
        {
            Debug.Log("Waiting for data to load...");
            yield return new WaitForSeconds(0.1f);
        }

        // Now the data is ready
        if (Admin == null)
            {
                Debug.LogError("Admin GameObject is not assigned in the Inspector!");
            }

        questionDict = Admin.GetComponent<QuestionDictionary>();

        if (questionDict == null)
        {
            Debug.LogError("QuestionDictionary component not found on Admin GameObject!");
        }
            
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!DialogueManager.Instance.isPlaying && !DialogueManager.Instance.battleComplete)
        {
            Debug.Log("Dialogue is triggered by scene start");
            DialogueManager.Instance.StartQuestionDialogue(inkJSON, knotName);
            //GetQuestion(difficulty);
        }
    }
    
    public QuestionData GetQuestion(string difficulty)
    {
        if (questionDict == null)
        {
            Debug.LogError("questionDict is null");
            return null;
        }

        Debug.Log("A random question is called!");
        testQuestion = questionDict.GetRandomQuestion(difficulty);
        Debug.Log($"{testQuestion.Question} {testQuestion.Answer1}");
        return testQuestion;
        

        /*
        if (QuestionDictionary.Instance != null)
        {
            QuestionData testQuestion = QuestionDictionary.Instance.GetRandomQuestion(difficulty);
            Debug.Log($"Got a question: {testQuestion.Question}");
        }
        */
    }
}