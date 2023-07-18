using System.Collections.Generic;

public class NarrativeNode
{
    public Dialogue Dialogue { get; private set; }
    public List<DialogueOption> Options { get; private set; }
    public string NodeId { get; private set; }

    public NarrativeNode(Dialogue dialogue, string nodeId)
    {
        Dialogue = dialogue;
        Options = new List<DialogueOption>();
        NodeId = nodeId;
    }

    public void AddOption(string newOption, NarrativeNode targetNode, Dialogue dialogueTransition)
    {
        var option = new DialogueOption(newOption, this, targetNode, dialogueTransition);
        Options.Add(option);
    }

    public bool IsLastDialogue() => Options.Count == 0;
}