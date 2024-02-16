using DialogueSystem.Utility;
using UnityEngine;

namespace DialogueSystem.Runtime.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource, effectSource;
        public static AudioPlayer Instance { get; private set; }

        private void Awake()
        {
            #region Singleton
            Instance ??= this;
            #endregion
        }

        public void PlaySound(AudioClip sound)
        {
            if (sound == null)
            {
                return;
            }
            LogHandler.Log($"SFX played: {sound.name}", LogHandler.Color.Blue);
            effectSource.PlayOneShot(sound);
        }

        public void PlayMusic(AudioClip music)
        {
            StopMusic();
            LogHandler.Log($"Music playing: {music.name}", LogHandler.Color.Blue);
            musicSource.PlayOneShot(music);
        }
        
        public void LoopMusic(AudioClip music)
        {
            StopMusic();
            LogHandler.Log($"Music looping: {music.name}", LogHandler.Color.Blue);
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.Play();
        }
        
        public void StopMusic()
        {
            if (!musicSource.isPlaying)
            {
                return;
            }
            LogHandler.Log($"Music stopped.", LogHandler.Color.Blue);
            musicSource.Stop();
            musicSource.loop = false;
        }
    }
}