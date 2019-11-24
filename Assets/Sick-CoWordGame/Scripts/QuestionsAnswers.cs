using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Question
{
    [HorizontalGroup("Questions")]
    [SerializeField]
    public List<string> question = new List<string>();
    [HorizontalGroup("Questions")]
    [SerializeField]
    public List<string> answer = new List<string>();
}

[System.Serializable]
public class DataHolder
{
    [SerializeField]
    public List<Question> questions;// = new List<Question>();
}

public class QuestionsAnswers : MonoBehaviour
{
    [SerializeField]
    public DataHolder Data;// = new List<Question>();

    [Button(ButtonSizes.Medium)]
    public void ParseToJson()
    {
        Question question = new Question();
        question.question.Add("asldkjas");
        question.answer.Add("vvm,mvld");
        string json = JsonUtility.ToJson(Data, true);
        Debug.Log(json);
    }
}
