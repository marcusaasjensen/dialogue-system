using System.Collections;
using UnityEngine;
using Utility;

namespace Scene
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource, effectSource;
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
            if (!musicSource.isPlaying) return;
            LogHandler.Log($"Music stopped.", LogHandler.Color.Blue);
            musicSource.Stop();
            musicSource.loop = false;
        }
    }
}