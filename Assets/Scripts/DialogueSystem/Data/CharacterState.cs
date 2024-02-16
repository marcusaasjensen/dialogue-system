using System;
using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Data
{
     [Serializable]
     public class CharacterState
     {
          [SerializeField] private Emotion emotionLabel;
          [SerializeField, Range(0, 2)] private float speakingSoundPitch = 1;
          [SerializeField] private Optional<AudioClip> reactionSound;
          [SerializeField] private Optional<Sprite> characterFace;
          
          public Emotion EmotionLabel => emotionLabel;
          public float SpeakingSoundPitch => speakingSoundPitch;
          public Optional<AudioClip> ReactionSound => reactionSound;
          public Optional<Sprite> CharacterFace => characterFace;
     }
}