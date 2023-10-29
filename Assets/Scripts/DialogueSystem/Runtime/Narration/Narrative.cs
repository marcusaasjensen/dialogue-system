using System.Collections.Generic;
using DialogueSystem.Data;

namespace DialogueSystem.Runtime.Narration
{
    public class Narrative
    {
        public List<NarrativeNode> NarrativeNodes { get; }
        public NarrativeNode NarrativeEntryNode { get; set; }
        public List<Speaker> Speakers { get; }

        public Narrative(List<Speaker> speakers)
        {
            NarrativeNodes = new List<NarrativeNode>();
            Speakers = speakers;
        }
    
        public void AddNarrativeNode(NarrativeNode node) => NarrativeNodes.Add(node);
    
        public Speaker GetSpeaker(string speakerName) => Speakers.Find(speaker => speaker.characterName == speakerName);
        
        public NarrativeNode FindStartNodeFromPath(string pathID)
        {
            if (string.IsNullOrEmpty(pathID))
                return NarrativeEntryNode;

            var node = NarrativeEntryNode;

            while (node != null)
            {
                if (string.IsNullOrEmpty(pathID)) return node;

                if (node.IsCheckpoint)
                {
                    pathID = pathID.Substring(1, pathID.Length - 1);
                    node = node.DefaultPath;
                    continue;
                }
            
                if (node.IsTransitionNode())
                {
                    node = node.DefaultPath;
                    continue;
                }

                if (node.Options.Count == 0)
                    return null;

                var optionIndex = (int) char.GetNumericValue(pathID[0]);

                node.Options[optionIndex].HasAlreadyBeenChosen = true;
                node = node.Options[optionIndex].TargetNarrative;
            
                pathID = pathID.Substring(1, pathID.Length - 1);
            }

            return null;
        }
    }
}
