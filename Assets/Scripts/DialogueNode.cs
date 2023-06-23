using System.Collections.Generic;

public class DialogueNode : Node<Dialogue>
{
    public int Id { get; private set; }
    
    public DialogueNode(Dialogue chosenDialogue, List<Node<Dialogue>> nextDialoguesToChoose, int id) 
        : base(chosenDialogue, nextDialoguesToChoose)
        => Id = id;
}