using System.Collections.Generic;
using UnityEngine;
namespace DialogueSystem.Data
{
    [CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/DialogueCharacter")]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private AudioClip speakingSound;
        [SerializeField] private CharacterState defaultState;
        [SerializeField] private List<CharacterState> states;
        
        public string CharacterName => characterName;
        public AudioClip SpeakingSound => speakingSound;
        public CharacterState DefaultState => defaultState;

        public CharacterState GetState(Emotion fromEmotion)
        {
            if (fromEmotion == Emotion.Default)
            {
                return defaultState;
            }
            
            var state =
                states.Find(emotion => emotion.EmotionLabel == fromEmotion)
                ?? defaultState;
            
            return state;
        }
    }
}
