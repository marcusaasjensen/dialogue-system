using System;
using UnityEngine;

namespace DialogueSystem.Data
{
     [Serializable]
     public class CharacterNarrativeBehaviour
     {
          public Emotion emotionLabel;
          public AudioClip emotionSound;
          [Range(0, 2)] public float SpeakingSoundPitch = 1;
          public Sprite characterFace;
     }
}