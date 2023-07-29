using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public string Guid;
    public List<Message> Dialogue;
    public Vector2 Position;
    public bool TransitionNode;
    public bool EntryPoint;
}
