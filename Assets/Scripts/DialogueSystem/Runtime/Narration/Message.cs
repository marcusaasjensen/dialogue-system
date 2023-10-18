using System;
using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Runtime.Narration
{
    [Serializable]
    public class Message
    {
        [field: SerializeField] public string SpeakerName { get; set; }
        [field: SerializeField] public Emotion EmotionDisplayed { get; set; }
        [field: SerializeField, TextArea] public string Content { get; set; }
        [field: SerializeField] public bool HideCharacter { get; set; }
    }
}