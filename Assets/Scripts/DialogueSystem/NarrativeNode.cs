using System.Collections.Generic;

public class NarrativeNode
{
    public Dialogue Dialogue { get; }
    public string NodeId { get; }
    public List<DialogueOption> Options { get; }
    public NarrativeNode DefaultPath { get; }

    public NarrativeNode(Dialogue dialogue, string nodeId, NarrativeNode defaultPath = null)
    {
        Dialogue = dialogue;
        NodeId = nodeId;
        DefaultPath = defaultPath;
        Options = new List<DialogueOption>();
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