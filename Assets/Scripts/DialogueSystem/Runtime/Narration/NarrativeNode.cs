using System.Collections.Generic;

namespace DialogueSystem.Runtime.Narration
{
    public class NarrativeNode
    {
        public List<DialogueMessage> Dialogue { get; }
        public string NodeId { get; }
        public List<DialogueOption> Options { get; }
        public NarrativeNode DefaultPath { get; }
        public bool IsCheckpoint { get; }
        public bool DisableAlreadyChosenOptions { get;  }

        private NarrativeNode(List<DialogueMessage> dialogue, string nodeId)
        {
            Dialogue = dialogue;
            NodeId = nodeId;
            Options = new List<DialogueOption>();
        }

        public NarrativeNode(List<DialogueMessage> dialogue, string nodeId, bool isCheckpoint, NarrativeNode defaultPath) : this(dialogue, nodeId)
        {
            DefaultPath = defaultPath;
            IsCheckpoint = isCheckpoint;
            DisableAlreadyChosenOptions = false;
        }
        
        public NarrativeNode(List<DialogueMessage> dialogue, string nodeId, bool disableAlreadyChosenOptions) : this(dialogue, nodeId)
        {
            DefaultPath = null;
            IsCheckpoint = false;
            DisableAlreadyChosenOptions = disableAlreadyChosenOptions;
        }
    
        public void AddOption(string newOption, NarrativeNode targetNode)
        {
            var option = new DialogueOption(newOption, targetNode);
            Options.Add(option);
        }

        public bool IsTipNarrativeNode() => Options.Count == 0 && DefaultPath == null;
        public bool IsSimpleNode() => Options.Count == 0 && DefaultPath != null;
        public bool HasNextChoice() => Options.Count > 0;
        public bool HasChoiceAfterSimpleNode() => !HasNextChoice() && DefaultPath != null && DefaultPath.HasOnlyChoices();
        private bool HasOnlyChoices() => Dialogue.Count == 0 && HasNextChoice();
    }
}