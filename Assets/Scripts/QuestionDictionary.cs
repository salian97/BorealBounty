#nullable disable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestionDictionary : MonoBehaviour
{
    private Dictionary<string, List<QuestionData>> questionDict;
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
            int randomIndex = Random.Range(0, questions.Count);
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

