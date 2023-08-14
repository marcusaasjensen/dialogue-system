using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NarrativeLoader : MonoBehaviour
{
        [SerializeField] private DialogueContainer narrativeToLoad;

        public string GetSavedNarrativePathID() => narrativeToLoad == null ? string.Empty : narrativeToLoad.PathToCheckpoint;

        public Narrative LoadNarrativeFromData()
        {
                if (narrativeToLoad == null)
                { 
                        Debug.LogError("Narrative's reference missing.", this);
                        Debug.LogWarning("Hint: if you have made any changes to your current narrative, do not forget to set up its new reference in the NarrativeLoader component!", this);
                  
                        return null;
                }
                
                var loadedNarrative = new Narrative();

                //Entry node is the first element node of all dialogue nodes in saved narrative
                var entryNode = narrativeToLoad.DialogueNodeData.Find(node => node.EntryPoint);
                CreateNodesFromEntryNode(entryNode, loadedNarrative);
                
                Debug.Log($"<color=#2CD3E1>Narrative loaded: {loadedNarrative}.</color>", this);
                
                return loadedNarrative;
        }

        private void CreateNodesFromEntryNode(DialogueNodeData entryNode, Narrative narrative)
        {
                var entryLink = narrativeToLoad.NodeLinks.Find(link => link.BaseNodeGuid == entryNode.Guid);
                
                if (entryLink == null) return;
                
                var firstNode = narrativeToLoad.DialogueNodeData.Find(node => node.Guid == entryLink.TargetNodeGuid);
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
                //Options to choose but no default path
                
                var choiceNode = new NarrativeNode(dialogue, node.Guid);

                var options = narrativeToLoad.NodeLinks.Where(x => x.BaseNodeGuid == node.Guid).ToList();

                narrative.AddNarrativeNode(choiceNode);

                options.ForEach(option =>
                {
                        var nextNode = narrativeToLoad.DialogueNodeData.Find(dialogueNode => option.TargetNodeGuid == dialogueNode.Guid);
                        choiceNode.AddOption(option.PortName, CreateNextNode(nextNode, narrative));
                });

                return choiceNode;
        }

        private NarrativeNode CreateTransitionNode(DialogueNodeData node, Narrative narrative, List<Message> dialogue)
        {
                //No option and default path exists
                
                var nextPath = narrativeToLoad.NodeLinks.Find(x => x.BaseNodeGuid == node.Guid);

                var nextNode = nextPath == null ? null : narrativeToLoad.DialogueNodeData.Find(dialogueNode => nextPath.TargetNodeGuid == dialogueNode.Guid);

                var transitionNode = new NarrativeNode(dialogue, node.Guid, CreateNextNode(nextNode, narrative));
                
                narrative.AddNarrativeNode(transitionNode);
                return transitionNode;
        }

        public void SaveNarrativePath(string narrativePathID)
        {
                narrativeToLoad.PathToCheckpoint = narrativePathID;
        }
}

