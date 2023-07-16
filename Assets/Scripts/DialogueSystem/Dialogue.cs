using System.Collections.Generic;
using System;

[Serializable]
public class Dialogue : Queue<Message>
{
    public event Action OnLastMessage;

    public Dialogue(IEnumerable<Message> messages) : base(messages){}

    public bool IsLastMessage() => Count == 0;

    public Message NextMessage()
    {
        var nextMessage = Count == 0 ? null : Dequeue();
        
        if (nextMessage != null) return nextMessage;
        
        OnLastMessage?.Invoke();
        return null;
    }
}