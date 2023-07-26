using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;
    private IEnumerable<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    private const string PathToResources = "Assets/Resources";
    private const string PathInResourcesFolder = "Narratives";
    
    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if (!Edges.Any()) return;

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (var i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                Guid = dialogueNode.GUID,
                Dialogue = dialogueNode.Messages,
                Position = dialogueNode.GetPosition().position,
                TransitionNode = dialogueNode.TransitionNode
            });
        }

        if (!AssetDatabase.IsValidFolder($"{PathToResources}/{PathInResourcesFolder}"))
            Directory.CreateDirectory($"{PathToResources}/{PathInResourcesFolder}");
        AssetDatabase.CreateAsset(dialogueContainer, $"{PathToResources}/{PathInResourcesFolder}/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>($"{PathInResourcesFolder}/{fileName}");

        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File not found.", "Target dialogue graph file does not exists.", "OK");
            return;
        }
        
        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }


    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);

                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(_containerCache.DialogueNodeData.First(x => x.Guid == targetNodeGuid).Position,
                    _targetGraphView.DefaultNodeSize
                    ));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }
    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.DialogueNodeData)
        { 
            var tempNode =  nodeData.TransitionNode 
                ? _targetGraphView.CreateDialogueTransitionNode("Transition Node", nodeData.Dialogue)
                : _targetGraphView.CreateDialogueNode("Multiple Choice Node", nodeData.Dialogue);
            tempNode.GUID = nodeData.Guid;
            
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            
            if (nodeData.TransitionNode) continue;

            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;
        foreach (var node in Nodes.Where(node => !node.EntryPoint))
        {
            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));
            
            _targetGraphView.RemoveElement(node);
        }
    }
}
