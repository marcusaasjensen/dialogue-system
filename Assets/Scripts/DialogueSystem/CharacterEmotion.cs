using System;
using UnityEngine;

[Serializable]
public class CharacterEmotion
{
     public Emotion emotionLabel;
     public AudioClip speakingSound;
     public float speakingRhythm;
     public Renderer characterFace;
}