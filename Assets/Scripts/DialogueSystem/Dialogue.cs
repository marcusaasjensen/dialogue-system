using System.Collections.Generic;
using System;

[Serializable]
public class Dialogue : List<Message>
{
    public Dialogue(IEnumerable<Message> messages) : base(messages){}
}