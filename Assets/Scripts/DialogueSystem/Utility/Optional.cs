using System;
using UnityEngine;

//By @aarthificial
namespace DialogueSystem.Utility 
{
    [Serializable]
    public struct Optional<T> : IEquatable<Optional<T>>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        public bool Enabled => enabled;
        public T Value => value;

        public Optional(T initialValue)
        {
            enabled = true;
            value = initialValue;
        }
        
        public static implicit operator Optional<T>(T value) => new (value);
        
        public bool Equals(Optional<T> other) => other.Value.Equals(Value);
    }
}