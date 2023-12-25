using System.Collections.Generic;
using DialogueSystem.Runtime.Narration;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor
{
    public class DialogueNode : Node
    {
        public string Guid { get; set; }
        public List<DialogueMessage> Messages { get; set; } = new();
        public bool EntryPoint { get; set; }
        public bool TransitionNode { get; set; }
        public bool Checkpoint { get; set; }
        public bool DisableAlreadyChosenOptions { get; set; }
    }
}
