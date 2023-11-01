using System.Collections.Generic;
using System.IO;
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
    
        [SerializeField] private List<string> variableNames;
        [SerializeField] private List<string> variableValues;
        
        public static DialogueVariables Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                var fileResourcePath = $"{ResourcesPath}/{FileName}";
                
                _instance = Resources.Load<DialogueVariables>($"{fileResourcePath}");
                
                if (_instance != null) return _instance;
                _instance = CreateInstance<DialogueVariables>();
            

                var pathToResources = $"{PathToResources}/{ResourcesPath}";
                
                if (!AssetDatabase.IsValidFolder($"{pathToResources}"))
                {
                    Directory.CreateDirectory($"{pathToResources}");
                }

                var assetPath = $"{PathToResources}/{fileResourcePath}.asset";
                
                AssetDatabase.CreateAsset(_instance, assetPath);
                
                _instance.variableNames = new List<string>();
                _instance.variableValues = new List<string>();
                
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