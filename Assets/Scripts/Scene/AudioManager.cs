using System.Collections;
using UnityEngine;

namespace Scene
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource, effectSource, pitchedSource;
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            #region Singleton
            if (Instance == null)
                Instance = this;
            #endregion
        }

        public void PlaySound(AudioClip sound, AudioSource source = null)
        {
            if (source == null) source = effectSource;
        
            if (sound != null)
                source.PlayOneShot(sound);
        }

        public void PlayMusic(AudioClip music)
        {
            if (music != null)
                musicSource.PlayOneShot(music);
        }

        public void PlaySoundAfterDelay(AudioClip sound, float delay) => StartCoroutine(PlaySoundAfterDelayCoroutine(sound, delay));

        private IEnumerator PlaySoundAfterDelayCoroutine(AudioClip sound, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySound(sound);
        }

        public void PlaySoundAtPitch(AudioClip speakingSound, float pitch)
        {
            pitchedSource.pitch = pitch;
            PlaySound(speakingSound, pitchedSource);
        }
        
        public void ChangePitchSource(float pitch) => pitchedSource.pitch = pitch;
        public void PlaySoundAtPitchSource(AudioClip sound) => PlaySound(sound, pitchedSource);
    }
}