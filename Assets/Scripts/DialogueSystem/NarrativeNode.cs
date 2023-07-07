using System.Collections.Generic;

public class NarrativeNode
{
    private Dialogue Dialogue { get; set; }
    public List<DialogueOption> Options { get; private set; }
    public string NodeId { get; private set; }

    public NarrativeNode(Dialogue dialogue, string nodeId)
    {
        Dialogue = dialogue;
        Options = new List<DialogueOption>();
        NodeId = nodeId;
    }

    public void AddOption(string newOption, NarrativeNode targetNode)
    {
        var option = new DialogueOption(newOption, this, targetNode);
        Options.Add(option);
    }
}