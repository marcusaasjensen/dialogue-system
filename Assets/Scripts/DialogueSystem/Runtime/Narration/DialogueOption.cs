using JetBrains.Annotations;

namespace DialogueSystem.Runtime.Narration
{
    public class DialogueOption
    {
        public string Text { get; }
        [CanBeNull] public NarrativeNode TargetNarrative { get;}
        public bool HasAlreadyBeenChosen { get; set; }

        public DialogueOption(string text, NarrativeNode targetNode)
        {
            Text = text;
            TargetNarrative = targetNode;
        }
    }
}