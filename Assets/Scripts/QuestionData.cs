using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

public class QuestionData : ScriptableObject
{
    public string? Question { get; set; }
    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public string? Answer4 { get; set; }

    public string? Hint { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Difficulty { get; set; }
}