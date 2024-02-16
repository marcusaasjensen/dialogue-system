using System;
using System.Collections.Generic;
using System.Globalization;
using DialogueSystem.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [Serializable]
    public abstract class Variable
    {
        [field: SerializeField] public string Name { get; set; }
        public abstract object GetValue();
        public abstract void SetValue(object newValue);
        public abstract override string ToString();
    }

    [Serializable]
    public class IntVariable : Variable
    {
        [SerializeField] internal int intValue;
        
        public override object GetValue() => intValue;

        public override void SetValue(object newValue)
        {
            try{
                intValue = int.Parse(newValue.ToString());
            }
            catch (Exception e)
            {
                LogHandler.Alert($"Error while parsing int value: {e.Message}");
            }
        }

        public override string ToString() => intValue.ToString(CultureInfo.InvariantCulture);
    }

    [Serializable]
    public class StringVariable : Variable
    {
        [SerializeField] internal string stringValue;

        public override object GetValue() => stringValue;
        
        public override void SetValue(object newValue)
        {
            try
            {
                stringValue = newValue.ToString();
            }
            catch (Exception e)
            {
                LogHandler.Alert($"Error while parsing string value: {e.Message}");
            }
        }
        
        public override string ToString() => stringValue;
    }

    [Serializable]
    public class FloatVariable : Variable
    {
        [SerializeField] internal float floatValue;

        public override object GetValue() => floatValue;
        
        public override void SetValue(object newValue)
        {
            try
            {
                floatValue = float.Parse(newValue.ToString());
            }
            catch (Exception e)
            {
                LogHandler.Alert($"Error while parsing float value: {e.Message}");
            }
        }
        
        public override string ToString() => floatValue.ToString(CultureInfo.InvariantCulture);
    }


    [CreateAssetMenu(fileName = "DialogueVariableData", menuName = "ScriptableObjects/EasyScriptableSingletons/DialogueVariableData")]
    
    public sealed class DialogueVariableData : EasyScriptableSingleton<DialogueVariableData>
    {
        protected override string PathToResources => "Assets/Resources";
        protected override string ResourcesPath => "Dialogue System Data/Dialogue";
        protected override string FileName => "DialogueVariableData";
        
        [SerializeReference] private List<Variable> variables;

        protected override void Initialize() => variables = new List<Variable>();

        public string GetValueAsString(string variableName)
        {
            var variable = variables.Find(v => v.Name == variableName);
            return variable?.ToString();
        }

        public void AddDialogueVariable<T>(string variableName, T value)
        {
            var valueToString = value.ToString();
            if (variables.Exists(v => v.Name == variableName))
            {
                ChangeDialogueVariable(variableName, valueToString);
                return;
            }
            
            switch (value)
            {
                case int _:
                    variables.Add(new IntVariable {Name = variableName, intValue = (int) (object) value});
                    break;
                case string _:
                    variables.Add(new StringVariable {Name = variableName, stringValue = (string) (object) value});
                    break;
                case float _:
                    variables.Add(new FloatVariable {Name = variableName, floatValue = (float) (object) value});
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            
            SaveRuntimeData();
        }

        public void ChangeDialogueVariable<T>(string variableName, T newValue)
        {
            var variable = variables.Find(v => v.Name == variableName);
            if (variable != null)
            {
                variable.SetValue(newValue);
            }
            else
            {
                AddDialogueVariable(variableName, newValue);
            }
            SaveRuntimeData();
        }

        public void RemoveAllDialogueVariables()
        {
            variables.Clear();
            SaveRuntimeData();
        }
        
        public void RemoveDialogueVariable(string variableName)
        {
            variables.RemoveAll(v => v.Name == variableName);
            SaveRuntimeData();
        }
        
        public void ListAllDialogueVariables()
        {
            LogHandler.Log("Dialogue Variables:", LogHandler.Color.Green);
            
            if(variables.Count == 0)
            {
                LogHandler.Log("No variables.", LogHandler.Color.Green);
                return;
            }
            
            foreach (var variable in variables)
            {
                LogHandler.Log( $"{variable.Name}: {variable}", LogHandler.Color.Green);
            }
        }
    }
}