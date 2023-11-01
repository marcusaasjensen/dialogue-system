﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        [FormerlySerializedAs("NodeLinks")] [HideInInspector]
        public List<NodeLinkData> nodeLinks = new();
        [FormerlySerializedAs("DialogueNodeData")] [HideInInspector]
        public List<DialogueNodeData> dialogueNodeData = new();
        [FormerlySerializedAs("PathToCheckpoint")] public string pathToCheckpoint = string.Empty;
        public bool isNarrativeEndReached;
        
        //save dialogue state to scriptable object and starts to where the dialogue was left off
        [SerializeField] private bool startFromPreviousNarrativePath = true;
        public bool StartFromPreviousNarrativePath => startFromPreviousNarrativePath;

        public List<CharacterData> characters;
    }
}