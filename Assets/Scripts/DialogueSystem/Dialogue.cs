using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public Dictionary<string, GameObject> Speakers { get; private set; }
    [field: SerializeField] public List<Message> Messages { get; private set; }
    [SerializeField] private bool isLocked;

    public Dialogue(Dictionary<string, GameObject> speakers, List<Message> messages, bool isLocked = true)
    {
        Speakers = speakers;
        Messages = messages;
        isLocked = this.isLocked;
    }
}