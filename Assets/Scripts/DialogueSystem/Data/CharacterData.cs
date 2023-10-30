using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "SpeakerScriptableObject", menuName = "ScriptableObjects/Speaker")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public AudioClip speakingSound;
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
}
