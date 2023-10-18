using System.Collections.Generic;

namespace DialogueSystem.Runtime.Narration
{
    public class NarrativeNode
    {
        public List<Message> Dialogue { get; }
        public string NodeId { get; }
        public List<DialogueOption> Options { get; }
        public NarrativeNode DefaultPath { get; }
        public bool IsCheckpoint { get; }
        public bool DisableAlreadyChosenOptions { get;  }

        public NarrativeNode(List<Message> dialogue, string nodeId, bool isCheckpoint = false, NarrativeNode defaultPath = null)
        {
            Dialogue = dialogue;
            NodeId = nodeId;
            DefaultPath = defaultPath;
            Options = new List<DialogueOption>();
            IsCheckpoint = isCheckpoint;
            DisableAlreadyChosenOptions = false;
        }
        
        public NarrativeNode(List<Message> dialogue, string nodeId, bool disableAlreadyChosenOptions = false)
        {
            Dialogue = dialogue;
            NodeId = nodeId;
            DefaultPath = null;
            Options = new List<DialogueOption>();
            IsCheckpoint = false;
            DisableAlreadyChosenOptions = disableAlreadyChosenOptions;
        }
    
        public void AddOption(string newOption, NarrativeNode targetNode)
        {
            var option = new DialogueOption(newOption, targetNode);
            Options.Add(option);
        }

        public bool IsTipNarrativeNode() => Options.Count == 0 && DefaultPath == null;
        public bool IsTransitionNode() => Options.Count == 0 && DefaultPath != null;
        public bool HasNextChoice() => Options.Count > 0;
        public bool HasChoiceAfterTransition() => !HasNextChoice() && DefaultPath != null && DefaultPath.HasOnlyChoices();
        private bool HasOnlyChoices() => Dialogue.Count == 0 && HasNextChoice();
    }
}