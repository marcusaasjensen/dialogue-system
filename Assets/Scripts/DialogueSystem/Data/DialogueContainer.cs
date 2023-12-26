using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        [field: FormerlySerializedAs("nodeLinks")] public List<NodeLinkData> NodeLinks { get; set; } = new();
        [field: FormerlySerializedAs("dialogueNodeData")] public List<DialogueNodeData> DialogueNodeData { get; set; } = new();

        [field: SerializeField] public string PathToCheckpoint { get; set; } = string.Empty;
        [field: SerializeField] public bool IsNarrativeEndReached { get; set; }
        //save dialogue state to scriptable object and starts to where the dialogue was left off
        [field: SerializeField] public bool StartFromPreviousNarrativePath { get; set; } = true;
        [field: SerializeField] public List<CharacterData> Characters { get; set; } = new();
    }
}