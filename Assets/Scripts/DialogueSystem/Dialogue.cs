using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueScriptableObject", menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] private List<GameObject> speakers;
    [SerializeField] private List<Message> messages;
}