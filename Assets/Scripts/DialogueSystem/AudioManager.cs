using UnityEditor;
using UnityEngine;

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

    public void PlaySound(AudioClip sound)
    {
        if (sound != null) 
            effectSource.PlayOneShot(sound);
    }
}