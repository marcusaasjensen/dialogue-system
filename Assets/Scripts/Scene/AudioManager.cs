using UnityEngine;

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
            if (music != null)
                musicSource.PlayOneShot(music);
        }
    }
}