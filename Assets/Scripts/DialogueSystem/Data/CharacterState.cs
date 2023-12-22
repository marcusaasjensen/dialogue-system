using System;
using UnityEngine;
using Utility;

namespace DialogueSystem.Data
{
     [Serializable]
     public class CharacterState
     {
          public Emotion emotionLabel;
          [Range(0, 2)] public float speakingSoundPitch = 1;
          public Optional<AudioClip> reactionSound;
          public Optional<Sprite> characterFace;
     }
}