using System;
using UnityEngine;
namespace DialogueSystem.Data
{
     [Serializable]
     public class CharacterState
     {
          public Emotion emotionLabel;
          public AudioClip emotionSound;
          [Range(0, 2)] public float speakingSoundPitch = 1;
          public Sprite characterFace;
     }
}