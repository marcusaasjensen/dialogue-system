﻿using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Utility;

namespace DialogueSystem.Data
{
    [Serializable]
    public abstract class Variable
    {
        public string name;
        public abstract object GetValue();
        public abstract void SetValue(object newValue);
        public abstract override string ToString();
    }

    [Serializable]
    public class IntVariable : Variable
    {
        public int intValue;
        
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
        public string stringValue;

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
        public float floatValue;

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
        protected override string ResourcesPath => "Dialogue";
        protected override string FileName => "DialogueVariableData";
        
        [SerializeReference] private List<Variable> variables;

        protected override void Initialize() => variables = new List<Variable>();

        public string GetValueAsString(string variableName)
        {
            var variable = variables.Find(v => v.name == variableName);
            return variable?.ToString();
        }

        public void AddDialogueVariable<T>(string variableName, T value)
        {
            var valueToString = value.ToString();
            if (variables.Exists(v => v.name == variableName))
            {
                ChangeDialogueVariable(variableName, valueToString);
                return;
            }
            
            switch (value)
            {
                case int _:
                    variables.Add(new IntVariable {name = variableName, intValue = (int) (object) value});
                    break;
                case string _:
                    variables.Add(new StringVariable {name = variableName, stringValue = (string) (object) value});
                    break;
                case float _:
                    variables.Add(new FloatVariable {name = variableName, floatValue = (float) (object) value});
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            
            SaveRuntimeData();
        }

        public void ChangeDialogueVariable<T>(string variableName, T newValue)
        {
            var variable = variables.Find(v => v.name == variableName);
            if(variable != null)
                variable.SetValue(newValue);
            else
                AddDialogueVariable(variableName, newValue);
            SaveRuntimeData();
        }
    }
}