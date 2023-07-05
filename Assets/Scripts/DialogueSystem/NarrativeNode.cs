using System.Collections.Generic;

public class NarrativeNode
{
    public Dialogue Dialogue { get; private set; }
    public List<DialogueOption> Options { get; private set; }

    public NarrativeNode(Dialogue dialogue, List<DialogueOption> options)
    {
        Dialogue = dialogue;
        Options = options;
    }
}