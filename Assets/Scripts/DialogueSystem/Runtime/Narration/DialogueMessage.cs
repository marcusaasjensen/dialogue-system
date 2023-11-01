using System;
using UnityEngine;

namespace DialogueSystem.Runtime.Narration
{
    [Serializable]
    public class DialogueMessage
    {
        [field: SerializeField] public string CharacterName { get; set; }
        [field: SerializeField, TextArea] public string Content { get; set; }
        [field: SerializeField] public bool HideCharacter { get; set; }
    }
}