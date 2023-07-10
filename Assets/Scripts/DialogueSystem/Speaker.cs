using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpeakerScriptableObject", menuName = "ScriptableObjects/Speaker")]
public class Speaker : ScriptableObject
{
    public string characterName;
    public List<CharacterNarrativeBehaviour> narrativeBehaviours;
}
