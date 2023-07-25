using JetBrains.Annotations;

public class DialogueOption
{
    public string Text { get; private set; }
    [NotNull] public NarrativeNode SourceNarrative { get; private set; }
    [CanBeNull] public NarrativeNode TargetNarrative { get; set; }
    //[CanBeNull] public Dialogue DialogueTransition { get; private set; }
    public bool HasAlreadyBeenChosen { get; set; }

    public DialogueOption(string text, NarrativeNode sourceNode, NarrativeNode targetNode)//, Dialogue dialogueTransition)
    {
        Text = text;
        SourceNarrative = sourceNode;
        TargetNarrative = targetNode;
        //DialogueTransition = dialogueTransition;
    }
}