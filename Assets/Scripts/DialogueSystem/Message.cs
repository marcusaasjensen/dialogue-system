using UnityEngine;

[System.Serializable]
public class Message
{
    [field: SerializeField] public string Speaker { get; set; }
    [field: SerializeField] public Emotion EmotionDisplayed { get; set; }
    [field: SerializeField, TextArea] public string Content { get; set; }
}