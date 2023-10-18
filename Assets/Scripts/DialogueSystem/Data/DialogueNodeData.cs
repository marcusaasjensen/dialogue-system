using System;
using System.Collections.Generic;
using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace DialogueSystem.Data
{
    [Serializable]
    public class DialogueNodeData
    {
        public string Guid;
        public List<Message> Dialogue;
        public Vector2 Position;
        public bool TransitionNode;
        public bool EntryPoint;
        public bool IsCheckpoint;
        public bool DisableAlreadyChosenOptions;
    }
}
