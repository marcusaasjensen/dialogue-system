using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogueSystem.Data;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;
        private IEnumerable<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        private const string PathToResources = "Assets/Resources";
        private const string ResourcesPath = "Dialogue System Data/Narratives";
    
        public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = targetGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            foreach (var edge in connectedPorts)
            {
                var outputNode = edge.output.node as DialogueNode;
                var inputNode = edge.input.node as DialogueNode;

                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode!.Guid,
                    PortName = edge.output.portName,
                    TargetNodeGuid = inputNode!.Guid
                });
            }

            var entryPoint = Nodes.Find(node => node.EntryPoint);
        
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                {
                    Guid = entryPoint.Guid,
                    Dialogue = null,
                    Position = entryPoint.GetPosition().position,
                    TransitionNode = false,
                    EntryPoint = true
                }
            );

            foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
            {
                dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                {
                    Guid = dialogueNode.Guid,
                    Dialogue = dialogueNode.Messages,
                    Position = dialogueNode.GetPosition().position,
                    TransitionNode = dialogueNode.TransitionNode,
                    IsCheckpoint = dialogueNode.Checkpoint,
                    DisableAlreadyChosenOptions = dialogueNode.DisableAlreadyChosenOptions
                });
            }
            
            dialogueContainer.Characters = new List<CharacterData>();

            _containerCache = Resources.Load<DialogueContainer>($"{ResourcesPath}/{fileName}");
            if (_containerCache)
            {
                _containerCache.Characters.ForEach(character => dialogueContainer.Characters.Add(character));
                dialogueContainer.StartFromPreviousNarrativePath = _containerCache.StartFromPreviousNarrativePath;
            }

            if (!AssetDatabase.IsValidFolder($"{PathToResources}/{ResourcesPath}"))
            {
                Directory.CreateDirectory($"{PathToResources}/{ResourcesPath}");
            }
        
            AssetDatabase.DeleteAsset($"{PathToResources}/{ResourcesPath}/{fileName}.asset");
            AssetDatabase.CreateAsset(dialogueContainer, $"{PathToResources}/{ResourcesPath}/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<DialogueContainer>($"{ResourcesPath}/{fileName}");

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
            foreach (var currentNode in Nodes)
            {
                var node = currentNode;
                var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == node.Guid).ToList();
  
                for (var j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.Find(x => x.Guid == targetNodeGuid);

                    LinkNodes(currentNode.outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
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
                if (nodeData.EntryPoint)
                {
                    continue;
                }
            
                var tempNode =  nodeData.TransitionNode 
                    ? _targetGraphView.CreateSimpleDialogueNode("Transition Node", nodeData.Dialogue, nodeData.IsCheckpoint)
                    : _targetGraphView.CreateMultipleChoiceNode("Multiple Choice Node", nodeData.Dialogue, nodeData.DisableAlreadyChosenOptions);
                tempNode.Guid = nodeData.Guid;
            
                tempNode.SetPosition(new Rect(nodeData.Position, _targetGraphView.DefaultNodeSize));

                _targetGraphView.AddElement(tempNode);

                if (nodeData.TransitionNode)
                {
                    continue;
                }

                var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ClearGraph()
        {
            Nodes.Find(node => node.EntryPoint).Guid = _containerCache.DialogueNodeData.Find(node => node.EntryPoint)?.Guid;
        
            foreach (var node in Nodes.Where(node => !node.EntryPoint))
            {
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
                _targetGraphView.RemoveElement(node);
            }
        }
    }
}
