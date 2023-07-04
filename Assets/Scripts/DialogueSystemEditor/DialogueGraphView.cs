using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
    private const int MaxChoiceTextLength = 35;
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

    private static Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        => node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));

    private static DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = true
        };
        var port = GeneratePort(node, Direction.Output);
        port.portName = "Next";
        node.outputContainer.Add(port);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(100,200,100,150));
        return node;
    }

    public void CreateNode(string nodeName) => AddElement(CreateDialogueNode(nodeName, new List<Message>()));

    public DialogueNode CreateDialogueNode(string nodeName, List<Message> messages)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            Messages = messages,
            GUID = Guid.NewGuid().ToString(),
        };

        SetupStyleSheet(dialogueNode);
        
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        
        var openDialogueButton = new Button(() =>
        {
            DialogueView.CloseWindow();
            DialogueView.OpenWindow(dialogueNode.Messages);
        }) { text = "Edit Dialogue" };
        var addChoiceButton = new Button(() => { AddChoicePort(dialogueNode); }) { text = "New Choice" };
        
        dialogueNode.titleContainer.Add(openDialogueButton);
        dialogueNode.titleContainer.Add(addChoiceButton);

        RefreshNode(dialogueNode);
        
        dialogueNode.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));
        return dialogueNode;
    }

    private static void RefreshNode(Node node)
    {
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    private static void SetupStyleSheet(Node dialogueNode)
    {
        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Editor/Node"));
        dialogueNode.mainContainer.AddToClassList("dialogueNodeMainContainer");
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overridenPortName) 
            ? $"Choice {outputPortCount+1}" 
            : overridenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName,
            maxLength = MaxChoiceTextLength
        };
        textField.AddToClassList("sized-input");
        
        var choiceContainer = new VisualElement();
        choiceContainer.AddToClassList("choice-container");
        
        
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort)) { text = "Remove" };

        var clickableLabel = new Label("[ Select ]");
        
        choiceContainer.Add(textField);
        choiceContainer.Add(deleteButton);
        choiceContainer.Add(clickableLabel);

        generatedPort.contentContainer.Add(choiceContainer);
        generatedPort.contentContainer.AddToClassList("content-container");

        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        RefreshNode(dialogueNode);
    }

    private void RemovePort(Node dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList()
            .Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        dialogueNode.outputContainer.Remove(generatedPort);

        var enumerable = targetEdge.ToList();
        if (!enumerable.Any()) return;
        
        var edge = enumerable.First();
        edge.input.Disconnect(edge);
        RemoveElement(enumerable.First());
        RefreshNode(dialogueNode);
    }
}
