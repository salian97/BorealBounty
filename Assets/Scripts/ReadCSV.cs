#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Ink.Parsed;
using System;

public class ReadCSV : MonoBehaviour
{
    /*

    For Zoe's reference:
    CSV files are formated as:

    Key-header, header2, header3 ...
    key-value1, value1.2, value1.3...
    key-value2, value2.2, value 2.3
    etc.

    */

    private string CSVFilepath;
    [SerializeField] private string CSVName;
    public List<QuestionData> QuestionsList;


    // Start is called before the first frame update
    void Start()
    {
        CSVName = "Chem.csv";
        CSVFilepath = "Assets/CSVs/" + CSVName;
        QuestionsList = new List<QuestionData>();

        // Skip the first line as it is just headers
        var questions = File.ReadAllLines(CSVFilepath).Skip(1);

        foreach (var question in questions)
        {
            var columns = question.Split(',');
            QuestionData newQuestion = ScriptableObject.CreateInstance<QuestionData>();
            newQuestion.Question = columns[0];
            newQuestion.Answer1 = columns[1];
            newQuestion.Answer2 = columns[2];
            newQuestion.Answer3 = columns[3];
            newQuestion.Answer4 = columns[4];
            newQuestion.Hint = columns[5];
            newQuestion.CorrectAnswer = columns[6];
            newQuestion.Difficulty = columns[7];
            QuestionsList.Add(newQuestion);
        }

        /*
        //For testing purposes
        Console.WriteLine("Parsed CSV Data:");
        foreach (QuestionData question in QuestionsList)
        {
            Debug.Log($"{question.Question} --- {question.Answer1} --- {question.Answer2} --- {question.Answer3} --- {question.Answer4} --- {question.Hint} --- {question.CorrectAnswer} --- {question.Difficulty}");
        }
        */
    }

}