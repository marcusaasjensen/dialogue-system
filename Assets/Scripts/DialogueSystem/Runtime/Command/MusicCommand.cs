using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.Command
{
    public class MusicCommand : DialogueCommand
    {
        private readonly AudioClip _musicClip;
        private readonly bool _stopMusic;

        public MusicCommand(int position, bool mustExecute, string musicName, bool stopMusic = false) : base(position, mustExecute)
        {
            _musicClip = DialogueAudioData.Instance.GetMusic(musicName);
            _stopMusic = stopMusic;

            if (_musicClip == null && !_stopMusic)
            {
                LogHandler.Warn($"Music not found: {musicName}");
            }
        }

        public override void Execute()
        {
            if (_stopMusic)
            {
                AudioPlayer.Instance.StopMusic();
                return;
            }
            
            AudioPlayer.Instance.LoopMusic(_musicClip);
        }
    }
}