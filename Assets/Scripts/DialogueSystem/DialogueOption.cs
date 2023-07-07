using JetBrains.Annotations;

public class DialogueOption
{
    public string Text { get; private set; }
    [NotNull] public NarrativeNode SourceNarrative { get; private set; }
    [CanBeNull] public NarrativeNode TargetNarrative { get; set; }

    public DialogueOption(string text, NarrativeNode sourceNode, NarrativeNode targetNode)
    {
        Text = text;
        SourceNarrative = sourceNode;
        TargetNarrative = targetNode;
    }
}