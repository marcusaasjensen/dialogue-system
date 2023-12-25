using System;
using System.Collections;
using DialogueSystem.Runtime.Command;
using TMPro;
using UnityEngine;

namespace DialogueSystem.Runtime.UI
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private float defaultTyperPace = 0.05f;
        
        public int TyperPosition { get; private set; }
        public bool IsTyping { get; private set; }
        public bool IsPaused { get; private set; }
        public float TyperPace { get; private set; }
        public float DefaultTyperPace => defaultTyperPace;
        
        public event Action OnTypingStart;
        public event Action OnTypingEnd;

        private Coroutine _typeTextCoroutine;

        private void Awake() => TyperPace = defaultTyperPace;
        
        public void TypeText(string text, TMP_Text textContainer)
        {
            TyperPace = defaultTyperPace;
            var plainText = DialogueCommandParser.RemoveSimpleTextTags(text);

            if (_typeTextCoroutine != null)
            {
                StopCoroutine(_typeTextCoroutine);
            }
            _typeTextCoroutine = StartCoroutine(TypeTextCoroutine(textContainer, plainText.Length));
        }

        private void StartTyping()
        {
            TyperPosition = 0;
            IsTyping = true;
            OnTypingStart?.Invoke();
        }

        private IEnumerator TypeTextCoroutine(TMP_Text textContainer, int textLength)
        {
            StartTyping();
            
            textContainer.ForceMeshUpdate();

            textContainer.maxVisibleCharacters = 0;

            var currentPace = 0.0f;

            for (var i = 0; i < textLength; i++)
            {
                yield return new WaitUntil(() => IsPaused == false);
                
                TyperPosition = i;

                if (currentPace > 0.0f)
                {
                    yield return new WaitForSeconds(currentPace);
                }

                currentPace = TyperPace;
                
                textContainer.maxVisibleCharacters++;
            }

            FinishTyping();
        } 

        public void FinishTyping()
        {
            if (_typeTextCoroutine == null)
            {
                return;
            }
            StopCoroutine(_typeTextCoroutine);
            ResetTyper();
            OnTypingEnd?.Invoke();
        }

        public void ResetTyper()
        {
            IsPaused = false;
            IsTyping = false;
            TyperPosition = 0;
            TyperPace = defaultTyperPace;
        }

        public void ChangePace(float newPace) => TyperPace = newPace >= 0 ? newPace : defaultTyperPace;
        public void Pause(float floatValue) => StartCoroutine(PauseCoroutine(floatValue));
        
        private IEnumerator PauseCoroutine(float timeToWait)
        {
            IsPaused = true;
            yield return new WaitForSeconds(timeToWait);
            IsPaused = false;
        }
    }
}
