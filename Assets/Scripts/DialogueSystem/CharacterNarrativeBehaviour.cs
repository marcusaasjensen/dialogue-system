using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[Serializable]
public class CharacterNarrativeBehaviour
{
     public Emotion emotionLabel;
     public AudioClip emotionSound;
     [Range(0, 2)] public float SpeakingSoundPitch = 1;
     public Sprite characterFace;
}