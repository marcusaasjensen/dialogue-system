using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
    private const int MaxChoiceTextLength = 35;
    
    private ReorderableList list;

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

    public void CreateNode(string nodeName) => AddElement(CreateDialogueNode(nodeName));

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            //DialogueText = nodeName,
            Messages = new List<Message>(),
            GUID = Guid.NewGuid().ToString(),

        };

        SetupStyleSheet(dialogueNode);
        
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        //Add choice button
        var addChoiceButton = new Button(() => { AddChoicePort(dialogueNode); });
        addChoiceButton.text = "New Choice";
        dialogueNode.titleContainer.Add(addChoiceButton);

        var messagesTitleContainer = new VisualElement();
        messagesTitleContainer.AddToClassList("row-container");

        var messagesTitle = new Label("Dialogue Script");
        messagesTitle.AddToClassList("title-label");

        var addMessageButton = new Button(() => { AddMessage(dialogueNode); });
        addMessageButton.text = "New Message";
        addMessageButton.AddToClassList("new-message-button");

        messagesTitleContainer.Add(messagesTitle);
        messagesTitleContainer.Add(addMessageButton);

        dialogueNode.mainContainer.Add(messagesTitleContainer);
        
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

    private static void AddMessage(DialogueNode dialogueNode)
    {
        var message = new Message
        {
            Speaker = string.Empty,
            EmotionDisplayed = Emotion.Happy,
            Content = string.Empty
        };

        var speakerTextField = new TextField(string.Empty);
        var contentTextField = new TextField(string.Empty);
        var emotionEnumField = new EnumField(Emotion.None);

        contentTextField.multiline = true;

        speakerTextField.SetValueWithoutNotify("Speaker's Name");
        contentTextField.SetValueWithoutNotify("Content\n");
        emotionEnumField.SetValueWithoutNotify(Emotion.None);
        
        speakerTextField.AddToClassList("sized-input");
        emotionEnumField.AddToClassList("sized-input");
        
        contentTextField.RegisterValueChangedCallback(evt => {message.Content = evt.newValue;});
        speakerTextField.RegisterValueChangedCallback(evt => { message.Speaker = evt.newValue;});
        emotionEnumField.RegisterValueChangedCallback(evt => { message.EmotionDisplayed = (Emotion) evt.newValue; });

        dialogueNode.Messages.Add(message);

        
        //Speaker label
        var speakerLabel = new Label("Speaker");
        speakerLabel.AddToClassList("header-label");
        
        //Speaker container
        var speakerContainer = new VisualElement();
        speakerContainer.AddToClassList("row-container");
        speakerContainer.Add(speakerLabel);
        speakerContainer.Add(speakerTextField);

        //Emotion label
        var emotionLabel = new Label("Emotion");
        emotionLabel.AddToClassList("header-label");
        
        //Emotion container
        var emotionContainer = new VisualElement();
        emotionContainer.AddToClassList("row-container");
        emotionContainer.Add(emotionLabel);
        emotionContainer.Add(emotionEnumField);
        
        //Message label
        var messageLabel = new Label("Message");
        messageLabel.AddToClassList("header-label");

        //Message container
        var messageContainer = new VisualElement();
        messageContainer.AddToClassList("message-container");
        messageContainer.Add(speakerContainer);
        messageContainer.Add(emotionContainer);
        messageContainer.Add(messageLabel);
        messageContainer.Add(contentTextField);

        dialogueNode.mainContainer.Add(messageContainer);
        RefreshNode(dialogueNode);
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
        
        if (!targetEdge.Any()) return;
        
        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());
        RefreshNode(dialogueNode);
    }
    
    private void CreateReorderableList()
    {
        // Create a list with initial elements
        list = new ReorderableList(new List<string> { "Item 1", "Item 2", "Item 3" }, typeof(string), true, true, true, true);

        // Define how each element in the list should be displayed
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.list[index] as string;
            EditorGUI.LabelField(rect, element);
        };

        // Define the height of each element in the list
        list.elementHeightCallback = index =>
        {
            // Adjust the value based on your content height
            return EditorGUIUtility.singleLineHeight + 2f;
        };
        
        //list.DoLayoutList();
    }
}
