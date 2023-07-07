﻿using System.Collections.Generic;

public class Narrative
{
    private NarrativeNode _narrativeEntryNode;
    public List<NarrativeNode> NarrativeNodes { get; }
    public NarrativeNode NarrativeEntryNode { set => _narrativeEntryNode = value; }

    public Narrative() => NarrativeNodes = new List<NarrativeNode>();
    public void AddNarrativeNode(NarrativeNode node) => NarrativeNodes.Add(node);
}
