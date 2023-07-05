using System.Collections.Generic;

public class DialogueOption
{
    public string Text { get; private set; }
    public NarrativeNode NextNarrative;
    
    public DialogueOption(string text, NarrativeNode nextNode)
    {
        Text = text;
        NextNarrative = nextNode;
    }
}