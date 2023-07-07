using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class NarrativeNode
{
    [field: SerializeField] public Dialogue Dialogue { get; private set; }
    public List<DialogueOption> Options { get; private set; }
    
    [FormerlySerializedAs("Id")] public string NodeId;

    public NarrativeNode(Dialogue dialogue, string nodeId)
    {
        Dialogue = dialogue;
        Options = new List<DialogueOption>();
        NodeId = nodeId;
    }

    public void AddOption(string newOption, NarrativeNode targetNode)
    {
        var option = new DialogueOption(newOption, this, targetNode);
        Options.Add(option);
    }  
}