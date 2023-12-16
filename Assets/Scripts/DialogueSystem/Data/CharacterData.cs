using System.Collections.Generic;
using UnityEngine;
namespace DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/DialogueCharacter")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public AudioClip speakingSound;
        public CharacterState defaultState;
        public List<CharacterState> states;

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
