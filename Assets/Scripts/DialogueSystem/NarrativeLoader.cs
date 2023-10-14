using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NarrativeLoader : MonoBehaviour
{
    [SerializeField] private DialogueContainer narrativeToLoad;

    private const string missingReferenceError = "Narrative's reference missing.";
    private const string missingReferenceHint = "Hint: if you have made any changes to your current narrative, do not forget to set up its new reference in the NarrativeLoader component!";
    
    public string GetSavedNarrativePathID() => narrativeToLoad.StartFromPreviousNarrativePath ? narrativeToLoad.pathToCheckpoint : string.Empty;

    public Narrative LoadNarrative(DialogueContainer narrative)
    {
        if (narrative != null)
            narrativeToLoad = narrative;

        return LoadNarrativeFromData();
    }

    private Narrative LoadNarrativeFromData()
    {
        if (narrativeToLoad == null)
        { 
            LogHandler.LogError(missingReferenceError);
            LogHandler.LogWarning(missingReferenceHint);
            return null;
        }
        
        LogHandler.Log($"Narrative loaded: {narrativeToLoad}", LogHandler.Color.Blue);

        var loadedNarrative = new Narrative(narrativeToLoad.Speakers);

        var entryNode = narrativeToLoad.dialogueNodeData.Find(node => node.EntryPoint);
        CreateNodesFromEntryNode(entryNode, loadedNarrative);

        return loadedNarrative;
    }

    private void CreateNodesFromEntryNode(DialogueNodeData entryNode, Narrative narrative)
    {
        var entryLink = narrativeToLoad.nodeLinks.Find(link => link.BaseNodeGuid == entryNode.Guid);

        if (entryLink == null) return;
        
        var firstNode = narrativeToLoad.dialogueNodeData.Find(node => node.Guid == entryLink.TargetNodeGuid);
        narrative.NarrativeEntryNode = CreateNextNode(firstNode, narrative);
    }

    private NarrativeNode CreateNextNode(DialogueNodeData node, Narrative narrative)
    {
        if (node == null) return null;
        var sameExistingNode = narrative.NarrativeNodes.Find(existingNode => existingNode.NodeId == node.Guid);

        if (sameExistingNode != null) return sameExistingNode;

        var dialogue = new List<Message>(node.Dialogue);

        return node.TransitionNode ? CreateTransitionNode(node, narrative, dialogue) : CreateChoiceNode(node, narrative, dialogue);
    }

    private NarrativeNode CreateChoiceNode(DialogueNodeData node, Narrative narrative, List<Message> dialogue)
    {
        var choiceNode = new NarrativeNode(dialogue, node.Guid);

        var options = narrativeToLoad.nodeLinks.Where(x => x.BaseNodeGuid == node.Guid).ToList();

        narrative.AddNarrativeNode(choiceNode);

        options.ForEach(option =>
        {
            var nextNode = narrativeToLoad.dialogueNodeData.Find(dialogueNode => option.TargetNodeGuid == dialogueNode.Guid);
            choiceNode.AddOption(option.PortName, CreateNextNode(nextNode, narrative));
        });

        return choiceNode;
    }

    private NarrativeNode CreateTransitionNode(DialogueNodeData node, Narrative narrative, List<Message> dialogue)
    {
        var nextPath = narrativeToLoad.nodeLinks.Find(x => x.BaseNodeGuid == node.Guid);

        var nextNode = nextPath == null ? null : narrativeToLoad.dialogueNodeData.Find(dialogueNode => nextPath.TargetNodeGuid == dialogueNode.Guid);

        var transitionNode = new NarrativeNode(dialogue, node.Guid, node.IsCheckpoint, CreateNextNode(nextNode, narrative));
        
        narrative.AddNarrativeNode(transitionNode);
        return transitionNode;
    }

    public void SaveNarrativePath(string narrativePathID, bool narrativeEndReached)
    {
        if (narrativeToLoad == null) return;
        narrativeToLoad.pathToCheckpoint = narrativePathID;
        narrativeToLoad.isNarrativeEndReached = narrativeEndReached;
    }

    public void ResetNarrative()
    {
        if (narrativeToLoad == null) return;
        narrativeToLoad.pathToCheckpoint = string.Empty;
        narrativeToLoad.isNarrativeEndReached = false;
    }
}
