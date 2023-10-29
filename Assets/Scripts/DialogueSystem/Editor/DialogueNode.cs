using System.Collections.Generic;
using DialogueSystem.Runtime;
using DialogueSystem.Runtime.Narration;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor
{
    public class DialogueNode : Node
    {
        public string GUID;
        public List<MessageData> Messages;
        public bool EntryPoint = false;
        public bool TransitionNode = false;
        public bool Checkpoint = false;
        public bool DisableAlreadyChosenOptions = false;
    }
}
