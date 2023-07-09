using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Dialogue
{
    [field: SerializeField] public Queue<Message> Messages { get; private set; }
    public event Action OnMessageQueueEmpty; 

    public Dialogue(IEnumerable<Message> messages)
    {
        Messages = new Queue<Message>(messages);
    }

    public Message NextMessage()
    {
        var nextMessage = Messages.Count == 0 ? null : Messages?.Dequeue();
        
        if (nextMessage != null) return nextMessage;
        
        OnMessageQueueEmpty?.Invoke();
        return null;
    }
}