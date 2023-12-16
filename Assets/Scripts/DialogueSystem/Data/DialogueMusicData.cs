using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace DialogueSystem.Data
{
    [Serializable]
    public class DialogueMusic
    {
        public string musicName;
        public AudioClip musicClip;
    }
    
    [CreateAssetMenu(fileName = "DialogueMusicData", menuName = "ScriptableObjects/EasyScriptableSingletons/DialogueMusicData")]
    
    public sealed class DialogueMusicData : EasyScriptableSingleton<DialogueMusicData>
    {
        public List<DialogueMusic> musicList;
        
        private static DialogueMusicData _instance;

        protected override string PathToResources => "Assets/Resources";
        protected override string ResourcesPath => "Dialogue";
        protected override string FileName => "DialogueMusicData";

        protected override void Initialize()
        {
            musicList = new List<DialogueMusic>();
        }

        public AudioClip GetMusic(string musicName)
        {
            var music = musicList.Find(m => m.musicName == musicName);
            return music?.musicClip;
        }
    }
}