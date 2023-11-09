using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        private Dictionary<string, string> _variableDictionary = new(); 
        
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
                
                AssetDatabase.SaveAssets();

                return _instance;
            }
        }

        public string GetValue(string variableName) => _variableDictionary[variableName];

        public void AddDialogueVariable<T>(string variableName, T value)
        {

            if (_variableDictionary.ContainsKey(variableName))
            {
                ChangeDialogueVariable(variableName, value);
                return;
            }
            
            _variableDictionary.Add(variableName, value.ToString());
            SaveRuntimeData();
        }

        public void ChangeDialogueVariable<T>(string variableName, T newValue)
        {
            if(_variableDictionary.ContainsKey(variableName))
                _variableDictionary[variableName] = newValue.ToString();
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