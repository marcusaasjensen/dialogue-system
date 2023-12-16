using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "DialogueVariableData", menuName = "ScriptableObjects/EasyScriptableSingletons/DialogueVariableData")]
    
    public sealed class DialogueVariableData : EasyScriptableSingleton<DialogueVariableData>
    {
        private static DialogueVariableData _instance;

        protected override string PathToResources => "Assets/Resources";
        protected override string ResourcesPath => "Dialogue";
        protected override string FileName => "DialogueVariableData";
        
        [SerializeField] private List<string> variableNames;
        [SerializeField] private List<string> variableValues;

        protected override void Initialize()
        {
            variableValues = new List<string>();
            variableNames = new List<string>();
        }

        public string GetValue(string variableName)
        {
            var index = variableNames.IndexOf(variableName);
            return index >= 0 ? variableValues[index] : string.Empty;
        }

        public void AddDialogueVariable<T>(string variableName, T value)
        {
            var valueToString = value.ToString();
            if (variableNames.Contains(variableName))
            {
                ChangeDialogueVariable(variableName, valueToString);
                return;
            }
            variableNames.Add(variableName);
            variableValues.Add(valueToString);
            SaveRuntimeData();
        }

        public void ChangeDialogueVariable<T>(string variableName, T newValue)
        {
            var index = variableNames.IndexOf(variableName);
        
            if (index >= 0)
                variableValues[index] = newValue.ToString();
            else
                AddDialogueVariable(variableName, newValue);
            SaveRuntimeData();
        }
    }
}