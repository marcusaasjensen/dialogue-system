using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Runtime.Command
{
    public class SoundEffectCommand : DialogueCommand
    {
        private readonly AudioClip _audioClip;

        public SoundEffectCommand(int position, bool mustExecute, string audioName) : base(position, mustExecute)
        {
            _audioClip = DialogueAudioData.Instance.GetSoundEffect(audioName);
            if (_audioClip == null)
            {
                LogHandler.Warn($"Sound effect not found: {audioName}");
            }
        }

        public override void Execute() => AudioPlayer.Instance.PlaySound(_audioClip);
    }
}