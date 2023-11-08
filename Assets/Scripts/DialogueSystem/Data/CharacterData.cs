using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "SpeakerScriptableObject", menuName = "ScriptableObjects/Speaker")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public AudioClip speakingSound;
        [FormerlySerializedAs("defaultBehaviour")] public CharacterState defaultState;
        [FormerlySerializedAs("narrativeBehaviours")] public List<CharacterState> states;

        public CharacterState GetState(Emotion fromEmotion)
        {
            if(fromEmotion == Emotion.Default)
                return defaultState;
            
            var state =
                states.Find(emotion => emotion.emotionLabel == fromEmotion)
                ?? defaultState;
            
            return state;
        }
    }
}
