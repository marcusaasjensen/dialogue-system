using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Runtime.Utility
{
    public class NarrativeLoader : MonoBehaviour
    {
        [HideInInspector][SerializeField] private DialogueContainer narrativeToLoad;

        private const string MissingReferenceError = "Narrative's reference missing.";
        private const string MissingReferenceHint = "Hint: if you have made any changes to your current narrative, do not forget to set up its new reference in the NarrativeLoader component!";
    
        public string GetSavedNarrativePathID() => narrativeToLoad.StartFromPreviousNarrativePath ? narrativeToLoad.PathToCheckpoint : string.Empty;

        public Narrative LoadNarrative(DialogueContainer narrative)
        {
            if (narrative != null)
            {
                narrativeToLoad = narrative;
            }

            return LoadNarrativeFromData();
        }

        private Narrative LoadNarrativeFromData()
        {
            if (narrativeToLoad == null)
            { 
                LogHandler.Alert(MissingReferenceError);
                LogHandler.Warn(MissingReferenceHint);
                return null;
            }
        
            LogHandler.Log($"Narrative loaded: {narrativeToLoad.name}", LogHandler.Color.Blue);
            
            var loadedNarrative = new Narrative(narrativeToLoad.Characters);

            var entryNode = narrativeToLoad.DialogueNodeData.Find(node => node.EntryPoint);
            CreateNodesFromEntryNode(entryNode, loadedNarrative);

            return loadedNarrative;
        }

        private void CreateNodesFromEntryNode(DialogueNodeData entryNode, Narrative narrative)
        {
            var entryLink = narrativeToLoad.NodeLinks.Find(link => link.BaseNodeGuid == entryNode.Guid);

            if (entryLink == null)
            {
                return;
            }
        
            var firstNode = narrativeToLoad.DialogueNodeData.Find(node => node.Guid == entryLink.TargetNodeGuid);
            narrative.NarrativeEntryNode = CreateNextNode(firstNode, narrative);
        }

        private NarrativeNode CreateNextNode(DialogueNodeData node, Narrative narrative)
        {
            if (node == null)
            {
                return null;
            }
            var sameExistingNode = narrative.NarrativeNodes.Find(existingNode => existingNode.NodeId == node.Guid);

            if (sameExistingNode != null)
            {
                return sameExistingNode;
            }

            var dialogue = new List<DialogueMessage>(node.Dialogue);

            return node.TransitionNode ? CreateTransitionNode(node, narrative, dialogue) : CreateChoiceNode(node, narrative, dialogue);
        }

        private NarrativeNode CreateChoiceNode(DialogueNodeData node, Narrative narrative, List<DialogueMessage> dialogue)
        {
            var choiceNode = new NarrativeNode(dialogue, node.Guid, node.DisableAlreadyChosenOptions);

            var options = narrativeToLoad.NodeLinks.Where(x => x.BaseNodeGuid == node.Guid).ToList();

            narrative.AddNarrativeNode(choiceNode);

            options.ForEach(option =>
            {
                var nextNode = narrativeToLoad.DialogueNodeData.Find(dialogueNode => option.TargetNodeGuid == dialogueNode.Guid);
                choiceNode.AddOption(option.PortName, CreateNextNode(nextNode, narrative));
            });

            return choiceNode;
        }

        private NarrativeNode CreateTransitionNode(DialogueNodeData node, Narrative narrative, List<DialogueMessage> dialogue)
        {
            var nextPath = narrativeToLoad.NodeLinks.Find(x => x.BaseNodeGuid == node.Guid);

            var nextNode = nextPath == null ? null : narrativeToLoad.DialogueNodeData.Find(dialogueNode => nextPath.TargetNodeGuid == dialogueNode.Guid);

            var transitionNode = new NarrativeNode(dialogue, node.Guid, node.IsCheckpoint, CreateNextNode(nextNode, narrative));
        
            narrative.AddNarrativeNode(transitionNode);
            return transitionNode;
        }

        public void SaveNarrativePath(string narrativePathID, bool narrativeEndReached)
        {
            if (narrativeToLoad == null)
            {
                return;
            }
            narrativeToLoad.PathToCheckpoint = narrativePathID;
            narrativeToLoad.IsNarrativeEndReached = narrativeEndReached;
        }

        public void ResetNarrative()
        {
            if (narrativeToLoad == null)
            {
                return;
            }
            narrativeToLoad.PathToCheckpoint = string.Empty;
            narrativeToLoad.IsNarrativeEndReached = false;
        }
    }
}
