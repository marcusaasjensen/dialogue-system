using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
        [SerializeField] private DialogueContainer narrative;
        [SerializeField] private List<NarrativeNode> narrativeNodes = new List<NarrativeNode>();
        private NarrativeNode _narrativeEntryNode;
        
        private void Awake() => CreateNarrativeFromData();

        private void CreateNarrativeFromData()
        {
                if (narrative == null)
                { 
                        Debug.LogError("Narrative's reference missing.");
                        return;
                }
                
                //initialize from entry node
                InitializeNarrativeNodes(narrative.DialogueNodeData[0]);
        }

        private void InitializeNarrativeNodes(DialogueNodeData entryNode) => _narrativeEntryNode = CreateNextNode(entryNode);

        private NarrativeNode CreateNextNode(DialogueNodeData node)
        {
                if (node == null) return null;

                var sameExistingNode = narrativeNodes.Find(existingNode => existingNode.NodeId == node.Guid);

                if (sameExistingNode != null) return sameExistingNode;

                var dialogue = new Dialogue(node.Dialogue);
                var narrativeNode = new NarrativeNode(dialogue, node.Guid);
                narrativeNodes.Add(narrativeNode);

                var options = narrative.NodeLinks.Where(x => x.BaseNodeGuid == node.Guid).ToList();
                options.ForEach(option =>
                {
                        var nextNode = narrative.DialogueNodeData.Find(dialogueNode => option.TargetNodeGuid == dialogueNode.Guid);
                        narrativeNode.AddOption(option.PortName, CreateNextNode(nextNode));
                });

                return narrativeNode;
        }
}

