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
        public List<DialogueMessage> Dialogue;
        public Vector2 Position;
        public bool TransitionNode; //TODO: rename to simple node or inverse values to name it multiple choice node
        public bool EntryPoint;
        public bool IsCheckpoint;
        public bool DisableAlreadyChosenOptions;
    }
}
