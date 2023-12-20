using DialogueSystem.Data;
using Scene;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public class SoundEffectCommand : DialogueCommand
    {
        private readonly AudioClip _audioClip;

        public SoundEffectCommand(int position, bool mustExecute, string audioName) : base(position, mustExecute)
        {
            _audioClip = DialogueAudioData.Instance.GetSoundEffect(audioName);
            if(_audioClip == null) LogHandler.Warn($"Sound effect not found: {audioName}");
        }

        public override void Execute() => AudioManager.Instance.PlaySound(_audioClip);
    }
}