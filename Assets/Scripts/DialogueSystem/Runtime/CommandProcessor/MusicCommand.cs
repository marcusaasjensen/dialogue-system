using DialogueSystem.Data;
using Scene;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public class MusicCommand : DialogueCommand
    {
        private readonly AudioClip _musicClip;
        private readonly bool _stopMusic;

        public MusicCommand(int position, bool mustExecute, string musicName, bool stopMusic = false) : base(position, mustExecute)
        {
            _musicClip = DialogueMusicData.Instance.GetMusic(musicName);
            _stopMusic = stopMusic;
            
            if(_musicClip == null && !_stopMusic) LogHandler.Warn($"Music not found: {musicName}");
        }

        public override void Execute()
        {
            if (_stopMusic)
            {
                AudioManager.Instance.StopMusic();
                return;
            }
            
            AudioManager.Instance.LoopMusic(_musicClip);
        }
    }
}