﻿using System.Collections.Generic;
using System;

[Serializable]
public class Dialogue
{
    public Queue<Message> Messages { get; private set; }
    public event Action OnLastMessage; 

    public Dialogue(IEnumerable<Message> messages)
    {
        Messages = new Queue<Message>(messages);
    }

    public bool IsLastMessage() => Messages.Count == 0;

    public Message NextMessage()
    {
        var nextMessage = Messages.Count == 0 ? null : Messages?.Dequeue();
        
        if (nextMessage != null) return nextMessage;
        
        OnLastMessage?.Invoke();
        return null;
    }
}