using System.Collections.Generic;

public class DialogueTreeNode : Node<Dialogue>
{
    public int Id { get; private set; }
    
    public DialogueTreeNode(Dialogue chosenDialogue, List<Node<Dialogue>> nextDialoguesToChoose, int id) 
        : base(chosenDialogue, nextDialoguesToChoose)
        => Id = id;
}