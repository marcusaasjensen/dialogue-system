using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerScriptableObject", menuName = "ScriptableObjects/Speaker")]
public class Speaker : ScriptableObject
{
    public string characterName;
    public AudioClip SpeakingSound;
    public CharacterNarrativeBehaviour defaultBehaviour;
    public List<CharacterNarrativeBehaviour> narrativeBehaviours;

    public CharacterNarrativeBehaviour GetBehaviourByEmotion(Emotion fromEmotion)
    {
        var speakerBehaviour =
            narrativeBehaviours.Find(emotion => emotion.emotionLabel == fromEmotion)
            ?? defaultBehaviour;
        return speakerBehaviour;
    }
}
