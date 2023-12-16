using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Data
{
     [Serializable]
     public class CharacterState
     {
          public Emotion emotionLabel;
          public AudioClip reactionSound;
          [Range(0, 2)] public float speakingSoundPitch = 1;
          public Sprite characterFace;
     }
}