using System.Collections;
using DialogueSystem.Runtime.UI;
using UnityEngine;

namespace DialogueSystem.Runtime.Audio
{
    public class CharacterSpeaker : MonoBehaviour
    {
        [SerializeField] private AudioSource speakingAudioSource;
        [SerializeField] private AudioSource reactionAudioSource;
        [SerializeField] private float speakingPace = 1f;
        [SerializeField] private bool synchronizeWithVariablePace = true;
        [SerializeField] private TextTyper textTyper;
        
        private void Awake()
        {
            textTyper.OnTypingStart += Speak;
            textTyper.OnTypingEnd += StopSpeaking;
        }

        private void Speak() => StartCoroutine(SpeakCoroutine());
        
        public void ChangePitch(float newPitch) => speakingAudioSource.pitch = newPitch;
        public void React(AudioClip reactionClip) => reactionAudioSource.PlayOneShot(reactionClip);
        public void ChangeVoice(AudioClip newVoice) => speakingAudioSource.clip = newVoice;
        private IEnumerator SpeakCoroutine()
        {
            while(textTyper.IsTyping)
            {
                var pace = synchronizeWithVariablePace ? textTyper.TyperPace * speakingPace : textTyper.DefaultTyperPace * speakingPace;

                speakingAudioSource.Play();
                
                yield return new WaitForSeconds(pace);

                if (!textTyper.IsPaused) continue;
                
                speakingAudioSource.Play();
                yield return new WaitUntil(() => textTyper.IsPaused == false);
            }
        }

        private void StopSpeaking() => StopAllCoroutines();
    }
}