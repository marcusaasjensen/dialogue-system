﻿using DialogueSystem.Data;
using UnityEngine;

namespace Scene
{
    public class ExampleSceneScript : MonoBehaviour
    {
        private void Awake() => DialogueVariableData.Instance.AddDialogueVariable("cost", 1.5f);        
    }
}