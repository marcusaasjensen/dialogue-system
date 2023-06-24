using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DialogueGraphNode : Node
{
    public string GUID;

    public string DialogueText;
    //public List<Message> Messages;
    public bool EntryPoint = false;
}
