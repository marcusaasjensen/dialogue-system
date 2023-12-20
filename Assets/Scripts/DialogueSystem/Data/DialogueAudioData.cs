using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace DialogueSystem.Data
{
    [Serializable]
    public class AudioData
    {
        public string name;
        public AudioClip audioClip;
    }
    
    [CreateAssetMenu(fileName = "DialogueAudioData", menuName = "ScriptableObjects/EasyScriptableSingletons/DialogueAudioData")]
    
    public sealed class DialogueAudioData : EasyScriptableSingleton<DialogueAudioData>
    {
        public List<AudioData> musicList;
        public List<AudioData> soundEffectList;

        protected override string PathToResources => "Assets/Resources";
        protected override string ResourcesPath => "Dialogue";
        protected override string FileName => "DialogueAudioData";

        protected override void Initialize()
        {
            musicList = new List<AudioData>();
            soundEffectList = new List<AudioData>();
        }

        public AudioClip GetMusic(string musicName)
        {
            var music = musicList.Find(m => m.name == musicName);
            return music?.audioClip;
        }
        
        public AudioClip GetSoundEffect(string soundEffectName)
        {
            var soundEffect = soundEffectList.Find(s => s.name == soundEffectName);
            return soundEffect?.audioClip;
        }
    }
}