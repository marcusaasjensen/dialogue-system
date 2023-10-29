using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
    public class DialogueVariables : ScriptableObject
    {
        private static DialogueVariables _instance;

        private const string PathToResources = "Assets/Resources";
        private const string ResourcesPath = "Dialogue";
        private const string FileName = "DialogueVariables";
    
        [SerializeField] private List<string> variableNames = new();
        [SerializeField] private List<string> variableValues = new();

        //TODO: ADD TAGS TO CUSTOMIZE VARIABLE VALUE IN DIALOGUE : START TAGS LIST AND END TAGS LIST
        
        public static DialogueVariables Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Resources.Load<DialogueVariables>(ResourcesPath);

                if (_instance != null) return _instance;
                _instance = CreateInstance<DialogueVariables>();
            
                var assetPath = $"{PathToResources}/{ResourcesPath}/{FileName}.asset";

                if (!AssetDatabase.IsValidFolder($"{PathToResources}/{ResourcesPath}"))
                {
                    Directory.CreateDirectory($"{PathToResources}/{ResourcesPath}");
                }

                AssetDatabase.CreateAsset(_instance, assetPath);
                AssetDatabase.SaveAssets();

                return _instance;
            }
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

        private void SaveRuntimeData()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}