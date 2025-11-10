#nullable disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class QuestionDictionary : MonoBehaviour
{
    public Dictionary<string, List<QuestionData>> questionDict;
    private Dictionary<string, List<QuestionData>> askedQuestionsDict;
    private List<QuestionData> questionsList;
    [SerializeField] private GameObject Admin;


    // Start is called before the first frame update
    void Start()
    {
        questionDict = new Dictionary<string, List<QuestionData>>();
        // Asked questions has 4 keys: easy, medium, and hard for questions gotten wrong, and 'correct' for questions gotten right
        askedQuestionsDict = new Dictionary<string, List<QuestionData>>();
        questionsList = Admin.GetComponent<ReadCSV>().QuestionsList;

        /*
        var groupedQuestions = questionsList.GroupBy(p => p.Difficulty);
        foreach (var group in groupedQuestions)
        {
            Debug.Log("Meow");
            questionDict.Add(group.Key, group.ToList());
            Debug.Log(questionDict[group.Key]);
        }
        */

        StartCoroutine(WaitForCSVData());
    }

    IEnumerator WaitForCSVData()
    {
        // Wait until questions are loaded
        while (Admin.GetComponent<ReadCSV>().QuestionsList == null || Admin.GetComponent<ReadCSV>().QuestionsList.Count == 0)
        {
            Debug.Log("Waiting for CSV data to load...");
            yield return new WaitForSeconds(0.1f);
        }

        // Now the data is ready
        questionsList = Admin.GetComponent<ReadCSV>().QuestionsList;
        ProcessQuestions();
    }


    public void ProcessQuestions()
    {
        var groupedQuestions = questionsList.GroupBy(p => p.Difficulty);

        foreach (var group in groupedQuestions)
        {
            questionDict.Add(group.Key, group.ToList());
        }
    }
    
    

    public QuestionData GetRandomQuestion(string diff)
    {
        if (questionDict.TryGetValue(diff, out var questions) && questions.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, questions.Count);
            Debug.Log($"Random question: {questions[randomIndex].Question} and difficulty {questions[randomIndex].Difficulty}");

            return questions[randomIndex];
        }
        return null;
    }

    public void PopulateAnswers(QuestionData answeredQuestion, string state)
    {
        if (askedQuestionsDict.ContainsKey(state))
        {
            // State is either easy, medium, hard, or correct
            askedQuestionsDict[state].Add(answeredQuestion);
        }
        else
        {
            askedQuestionsDict.Add(state, new List<QuestionData>());
            askedQuestionsDict[state].Add(answeredQuestion);
        }
    }
}

/*
    public void AddQuestion(Question question)
    {
        allQuestions.Add(question);
        if (!questionsByDifficulty.ContainsKey(question.difficulty))
            questionsByDifficulty[question.difficulty] = new List<Question>();
        questionsByDifficulty[question.difficulty].Add(question);
    }

*/