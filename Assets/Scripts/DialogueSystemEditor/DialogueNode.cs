using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class DialogueNode : Node
{
    public string GUID;
    public List<Message> Messages;
    public bool EntryPoint = false;
}
