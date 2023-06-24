using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("Editor/DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        grid.StretchToParentSize();
        
        Insert(0, grid);
        
        AddElement(GenerateEntryPointNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if(startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    private Port GeneratePort(DialogueGraphNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        => node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));

    private DialogueGraphNode GenerateEntryPointNode()
    {
        var node = new DialogueGraphNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true

        };
        var port = GeneratePort(node, Direction.Output);
        port.portName = "Next";
        node.outputContainer.Add(port);
        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(100,200,100,150));
        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateGraphDialogueNode(nodeName));
    }

    public DialogueGraphNode CreateGraphDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueGraphNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
            
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(dialogueNode);});
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return dialogueNode;
    }

    private void AddChoicePort(DialogueGraphNode dialogueNode)
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var outputPortName = $"Choice {outputPortCount}";
        generatedPort.portName = outputPortName;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }
}
