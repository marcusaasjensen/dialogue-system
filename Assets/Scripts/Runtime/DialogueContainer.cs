using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    [FormerlySerializedAs("NodeLinks")] [HideInInspector]
    public List<NodeLinkData> nodeLinks = new();
    [FormerlySerializedAs("DialogueNodeData")] [HideInInspector]
    public List<DialogueNodeData> dialogueNodeData = new();
    [FormerlySerializedAs("PathToCheckpoint")] public string pathToCheckpoint= string.Empty;
    public bool isNarrativeEndReached;
}