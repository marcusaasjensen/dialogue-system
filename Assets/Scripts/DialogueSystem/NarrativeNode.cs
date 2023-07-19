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
        var option = new DialogueOption(newOption, this, targetNode);
        Options.Add(option);
    }

    public bool IsLastDialogue() => Options.Count == 0;
}