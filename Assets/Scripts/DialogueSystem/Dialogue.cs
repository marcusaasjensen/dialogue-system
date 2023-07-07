using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Dialogue
{
    [field: SerializeField] public List<Message> Messages { get; private set; }

    public Dialogue(List<Message> messages)
    {
        Messages = messages;
    }
}