using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueVariables : ScriptableObject
{
    private static DialogueVariables _instance;

    private const string PathToResources = "Assets/Resources";
    private const string ResourcesPath = "Dialogue";
    private const string FileName = "DialogueVariables";
    
    [SerializeField] private List<string> variableNames = new();
    [SerializeField] private List<string> variableValues = new();

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

    public void AddDialogueVariable(string variableName, string value)
    {
        if (variableNames.Contains(variableName))
        {
            ChangeDialogueVariable(variableName, value);
            return;
        }
        variableNames.Add(variableName);
        variableValues.Add(value);
    }

    public void ChangeDialogueVariable(string variableName, string newValue)
    {
        var index = variableNames.IndexOf(variableName);
        
        if (index >= 0)
            variableValues[index] = newValue;
        else
            AddDialogueVariable(variableName, newValue);
    }
}