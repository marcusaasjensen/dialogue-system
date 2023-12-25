using System;
using System.Collections.Generic;
using DialogueSystem.Runtime.Narration;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [Serializable]
    public class DialogueNodeData
    {
        [FormerlySerializedAs("Guid")] [SerializeField] private string guid;
        [FormerlySerializedAs("Dialogue")] [SerializeField] private List<DialogueMessage> dialogue;
        [FormerlySerializedAs("Position")] [SerializeField] private Vector2 position;
        [FormerlySerializedAs("TransitionNode")] [SerializeField] private bool transitionNode; //TODO: rename to simple node or inverse values to name it multiple choice node
        [FormerlySerializedAs("EntryPoint")] [SerializeField] private bool entryPoint;
        [FormerlySerializedAs("IsCheckpoint")] [SerializeField] private bool isCheckpoint;
        [FormerlySerializedAs("DisableAlreadyChosenOptions")] [SerializeField] private bool disableAlreadyChosenOptions;
        
        public string Guid
        {
            get => guid;
            set => guid = value;
        }

        public List<DialogueMessage> Dialogue
        {
            get => dialogue;
            set => dialogue = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public bool TransitionNode
        {
            get => transitionNode;
            set => transitionNode = value;
        }

        public bool EntryPoint
        {
            get => entryPoint;
            set => entryPoint = value;
        }

        public bool IsCheckpoint
        {
            get => isCheckpoint;
            set => isCheckpoint = value;
        }

        public bool DisableAlreadyChosenOptions
        {
            get => disableAlreadyChosenOptions;
            set => disableAlreadyChosenOptions = value;
        }
    }
}
