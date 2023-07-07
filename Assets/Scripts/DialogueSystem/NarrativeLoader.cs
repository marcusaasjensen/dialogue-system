using System.Linq;
using UnityEngine;

public class NarrativeLoader : MonoBehaviour
{
        [SerializeField] private DialogueContainer narrativeToLoad;

        public Narrative LoadNarrativeFromData()
        {
                if (narrativeToLoad == null)
                { 
                        Debug.LogError("Narrative's reference missing.");
                        return null;
                }
                
                var loadedNarrative = new Narrative();
                
                //Entry node is the first element node of all dialogue nodes in saved narrative
                var entryNode = narrativeToLoad.DialogueNodeData[0];
                
                CreateNodesFromEntryNode(entryNode, loadedNarrative);
                
                Debug.Log($"Narrative loaded: {loadedNarrative}.", this);
                
                return loadedNarrative;
        }

        private void CreateNodesFromEntryNode(DialogueNodeData node, Narrative narrative) => narrative.NarrativeEntryNode = CreateNextNode(node, narrative);

        private NarrativeNode CreateNextNode(DialogueNodeData node, Narrative narrative)
        {
                if (node == null) return null;

                var sameExistingNode = narrative.NarrativeNodes.Find(existingNode => existingNode.NodeId == node.Guid);

                if (sameExistingNode != null) return sameExistingNode;

                var dialogue = new Dialogue(node.Dialogue);
                var narrativeNode = new NarrativeNode(dialogue, node.Guid);
                narrative.AddNarrativeNode(narrativeNode);

                var options = narrativeToLoad.NodeLinks.Where(x => x.BaseNodeGuid == node.Guid).ToList();
                options.ForEach(option =>
                {
                        var nextNode = narrativeToLoad.DialogueNodeData.Find(dialogueNode => option.TargetNodeGuid == dialogueNode.Guid);
                        narrativeNode.AddOption(option.PortName, CreateNextNode(nextNode, narrative));
                });

                return narrativeNode;
        }
}

