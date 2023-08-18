using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new();
    public List<DialogueNodeData> DialogueNodeData = new();
    public string PathToCheckpoint= "";
}