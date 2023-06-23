using UnityEngine;

[System.Serializable]
public class Message
{
    [SerializeField] private string speaker;
    [SerializeField] private Emotion emotionDisplayed;
    [SerializeField, TextArea] private string content;
}