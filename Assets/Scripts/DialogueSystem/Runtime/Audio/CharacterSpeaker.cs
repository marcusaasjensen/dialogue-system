using System.Collections;
using DialogueSystem.Runtime.UI;
using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Runtime.Audio
{
    public class CharacterSpeaker : MonoBehaviour
    {
        [SerializeField] private TextTyper textTyper;
        [Header("Audio Sources"), SerializeField] private AudioSource speakingAudioSource;
        [SerializeField] private AudioSource reactionAudioSource;
        [Header("Speaking Pace"), SerializeField] private float speakingPace = 1f;
        [SerializeField] private bool synchronizeWithVariablePace = true;
        
        private void Awake()
        {
            textTyper.OnTypingStart += Speak;
            textTyper.OnTypingEnd += StopSpeaking;
        }

        private void Speak() => StartCoroutine(SpeakCoroutine());
        
        public void ChangePitch(float newPitch) => speakingAudioSource.pitch = newPitch;

        public void React(Optional<AudioClip> reactionClip)
        {
            if (!reactionClip.Enabled)
            {
                return;
            }
            reactionAudioSource.PlayOneShot(reactionClip.Value);
        }

        public void ChangeVoice(AudioClip newVoice) => speakingAudioSource.clip = newVoice;
        private IEnumerator SpeakCoroutine()
        {
            while(textTyper.IsTyping)
            {
                var pace = synchronizeWithVariablePace ? textTyper.TyperPace * speakingPace : textTyper.DefaultTyperPace * speakingPace;

                speakingAudioSource.Play();
                
                yield return new WaitForSeconds(pace);

                if (!textTyper.IsPaused)
                {
                    continue;
                }
                
                speakingAudioSource.Play();
                yield return new WaitUntil(() => textTyper.IsPaused == false);
            }
        }

        private void StopSpeaking() => StopAllCoroutines();
    }
}