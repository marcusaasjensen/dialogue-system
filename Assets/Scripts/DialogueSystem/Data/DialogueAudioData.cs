using System;
using System.Collections.Generic;
using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Data
{
    [Serializable]
    public class AudioData
    {
        [SerializeField] private string name;
        [SerializeField] private AudioClip audioClip;
        
        public string Name => name;
        public AudioClip AudioClip => audioClip;
        
    }
    
    [CreateAssetMenu(fileName = "DialogueAudioData", menuName = "ScriptableObjects/EasyScriptableSingletons/DialogueAudioData")]
    
    public sealed class DialogueAudioData : EasyScriptableSingleton<DialogueAudioData>
    {
        [SerializeField] private List<AudioData> musicList;
        [SerializeField] private List<AudioData> soundEffectList;
        
        protected override string PathToResources => "Assets/Resources";
        protected override string ResourcesPath => "Dialogue System Data/Dialogue";
        protected override string FileName => "DialogueAudioData";

        protected override void Initialize()
        {
            musicList = new List<AudioData>();
            soundEffectList = new List<AudioData>();
        }

        public AudioClip GetMusic(string musicName)
        {
            var music = musicList.Find(m => m.Name == musicName);
            return music?.AudioClip;
        }
        
        public AudioClip GetSoundEffect(string soundEffectName)
        {
            var soundEffect = soundEffectList.Find(s => s.Name == soundEffectName);
            return soundEffect?.AudioClip;
        }
    }
}