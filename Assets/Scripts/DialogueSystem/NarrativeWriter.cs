using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NarrativeWriter : MonoBehaviour
{
    [SerializeField] private int punctuationDelayMultiplier = 10;
    
    private Coroutine _currentMessageCoroutine;

    private static readonly char[] Punctuation = {'.', ',', '!', '?', ':', ';'};
    private static readonly char[] CharactersToIgnore = {' '};
    
    //private DialogueVertexAnimator dialogueVertexAnimator;
    //public TMP_Text textBox;
    
    //private event Action OnFinish;

    private void Awake()
    {
        //dialogueVertexAnimator = new DialogueVertexAnimator(textBox);
        //OnFinish += EndMessage;
    }
    
    public void WriteMessage(Message message, TextMeshProUGUI textContainer, CharacterNarrativeBehaviour speakerBehaviour)
    {
        //dialogueVertexAnimator.textAnimating = false;
        var commands = DialogueUtility.ProcessInputString(message.Content, out var totalTextMessage);
        Debug.Log(commands.Count);
        if(_currentMessageCoroutine != null)
            StopCoroutine(_currentMessageCoroutine);
        
        message.Content = totalTextMessage;

        //_currentMessageCoroutine = StartCoroutine(dialogueVertexAnimator.AnimateTextIn(commands, totalTextMessage, speakerBehaviour.speakingSound, OnFinish));

        _currentMessageCoroutine = StartCoroutine(
            TypeMessage(message.Content, speakerBehaviour.speakingRhythm, textContainer, speakerBehaviour.speakingSound)
            );
    }
    
    private IEnumerator TypeMessage(string message, float delayBetweenLetters, TMP_Text textContainer, AudioClip speakerSound)
    {
        textContainer.ForceMeshUpdate();
        textContainer.maxVisibleCharacters = 0;

        float currentDelay = 0;
        
        for (var i = 0; i < message.Length; i++)
        {
            if(message[i] == '<')
            {
                while (message[i] != '>')
                    i++;
                
                if (++i == message.Length)
                    break;
            }

            var c = message[i];

            yield return new WaitForSeconds(currentDelay);
            
            textContainer.maxVisibleCharacters++;
            
            currentDelay = delayBetweenLetters;
            
            if (CharactersToIgnore.Contains(c))
                continue;

            if (Punctuation.Contains(c))
                currentDelay = delayBetweenLetters * punctuationDelayMultiplier;

            AudioManager.Instance.PlaySound(speakerSound);
        }

        EndMessage();
    }

    public Coroutine CurrentMessageCoroutine() => _currentMessageCoroutine;

    public bool IsWritingMessage() => _currentMessageCoroutine != null;

    public void EndMessage()
    {
        if (_currentMessageCoroutine == null) return;
        StopCoroutine(_currentMessageCoroutine);
        _currentMessageCoroutine = null;
    }
}
