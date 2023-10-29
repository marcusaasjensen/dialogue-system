using System;
using System.Collections;
using DialogueSystem.Runtime.Utility;
using Scene;
using TMPro;
using UnityEngine;

namespace DialogueSystem.Runtime.UI
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private float defaultSpeed = 0.05f;
        
        public int TyperPosition { get; private set; }
        public bool IsTyping { get; private set; }
        public bool IsPaused { get; private set; }
        
        public event Action OnTypingStart;

        private Coroutine _typeTextCoroutine;
        private float _currentSpeed;
        private AudioClip _typerSound;
        private float _typerPitch;
        private const char SpaceCharacter = ' ';

        private void Awake() => _currentSpeed = defaultSpeed;
        
        public void TypeText(string text, TextMeshProUGUI textContainer)
        {
            _currentSpeed = defaultSpeed;
            var plainText = DialogueParser.RemoveSimpleTextTags(text);
            if(_typeTextCoroutine != null) StopCoroutine(_typeTextCoroutine);
            _typeTextCoroutine = StartCoroutine(TypeTextCoroutine(plainText, textContainer));
        }

        private IEnumerator TypeTextCoroutine(string text, TMP_Text textContainer)
        {
            TyperPosition = 0;
            IsTyping = true;
            
            OnTypingStart?.Invoke();
            
            textContainer.ForceMeshUpdate();

            textContainer.maxVisibleCharacters = 0;

            var currentDelay = 0.0f;

            for (var i = 0; i < text.Length; i++)
            {
                yield return new WaitUntil(() => IsPaused == false);
                TyperPosition = i; //++ ??
                
                var c = text[i];
                // REPLACE WITH TALKING CONTROLLER THAT LOOPS WHEN THE TYPER IS TYPING AND NOT PAUSED 
                if (c == SpaceCharacter || currentDelay != 0)
                {
                    yield return new WaitForSeconds(currentDelay);
                    AudioManager.Instance.PlaySoundAtPitch(_typerSound, _typerPitch);
                }
                ////////////////////////////
                currentDelay = _currentSpeed;
                textContainer.maxVisibleCharacters++;
            }

            FinishTyping();
        }

        public void FinishTyping()
        {
            if (_typeTextCoroutine == null) return;
            StopCoroutine(_typeTextCoroutine);
            ResetTyper();
        }

        private void ResetTyper()
        {
            IsPaused = false;
            IsTyping = false;
            TyperPosition = 0;
        }

        public void SetTypingSound(AudioClip sound) => _typerSound = sound;
        public void ChangePitch(float pitch) => _typerPitch = pitch;
        public void ChangeSpeed(float textSpeed) => _currentSpeed = textSpeed;
        public void ResetSpeed() => _currentSpeed = defaultSpeed;
        public void Pause(float floatValue) => StartCoroutine(PauseCoroutine(floatValue));
        //public void PauseUntil(Event evt) => StartCoroutine(PauseUntilCoroutine(evt));
        
        private IEnumerator PauseCoroutine(float timeToWait)
        {
            IsPaused = true;
            yield return new WaitForSeconds(timeToWait);
            IsPaused = false;
        }
    }
}
