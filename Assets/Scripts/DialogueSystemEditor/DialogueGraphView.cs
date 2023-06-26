using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
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

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        => node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));

    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            Messages = new List<Message>(),
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

    public void CreateNode(string nodeName) => AddElement(CreateDialogueNode(nodeName));

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            Messages = new List<Message>(),
            GUID = Guid.NewGuid().ToString(),
            
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        
        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Editor/Node"));
        
        var addChoiceButton = new Button(() => { AddChoicePort(dialogueNode);});
        addChoiceButton.text = "New Choice";
        dialogueNode.titleContainer.Add(addChoiceButton);
        
        var addMessageButton = new Button(() => { AddMessage(dialogueNode);});
        addMessageButton.text = "New Message";
        dialogueNode.titleContainer.Add(addMessageButton);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));
        return dialogueNode;
    }

    private void AddMessage(DialogueNode dialogueNode)
    {
        var message = new Message
        {
            Speaker = string.Empty,
            EmotionDisplayed = Emotion.Happy,
            Content = string.Empty
        };

        var speakerTextField = new TextField(string.Empty);
        var contentTextField = new TextField(string.Empty);
        var emotionEnumField = new EnumField(Emotion.Happy);

        speakerTextField.SetValueWithoutNotify("Speaker's Name");
        contentTextField.SetValueWithoutNotify("Message");
        emotionEnumField.SetValueWithoutNotify(Emotion.Happy);
        
        contentTextField.RegisterValueChangedCallback(evt =>
        {
            message.Content = evt.newValue;
        });

        speakerTextField.RegisterValueChangedCallback(evt =>
        {
            message.Speaker = evt.newValue;
        });
        
        emotionEnumField.RegisterValueChangedCallback(evt =>
        {
            message.EmotionDisplayed = (Emotion) evt.newValue;
        });

        dialogueNode.Messages.Add(message);

        dialogueNode.mainContainer.Add(speakerTextField);
        dialogueNode.mainContainer.Add(contentTextField);
        dialogueNode.mainContainer.Add(emotionEnumField);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
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
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);

        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList()
            .Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        dialogueNode.outputContainer.Remove(generatedPort);
        
        if (!targetEdge.Any()) return;
        
        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
