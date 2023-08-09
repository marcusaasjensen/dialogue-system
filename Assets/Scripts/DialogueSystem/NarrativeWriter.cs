using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NarrativeWriter : MonoBehaviour
{
    private List<DialogueCommand> _dialogueCommands = new List<DialogueCommand>();
    private Coroutine _currentMessageCoroutine;
    private static readonly char[] CharactersToIgnore = {' '};

    public void WriteMessage(string message, TextMeshProUGUI textContainer, AudioClip speakingSound)
    {
        _dialogueCommands.Clear();
        _dialogueCommands = DialogueUtility.ProcessInputString(message, out var processedMessage);
        
        textContainer.text = processedMessage;

        processedMessage = RemoveMessageTags(processedMessage);

        var delays = new float[processedMessage.Length];

        var defaultDelay = DialogueUtility.DefaultSpeed;
        
        for (var i = 0; i < delays.Length; i++)
        {
            var commandAtPosition = _dialogueCommands.Find(command => command.position == i);

            if (commandAtPosition == null)
            {
                delays[i] = defaultDelay;
                continue;
            }
            
            switch(commandAtPosition.type)
            {
                case DialogueCommandType.TextSpeedChange:
                    defaultDelay = commandAtPosition.floatValue;
                    delays[i] = defaultDelay;
                    break;
                case DialogueCommandType.Pause:
                    delays[i] = commandAtPosition.floatValue;
                    break;
                case DialogueCommandType.AnimStart:
                case DialogueCommandType.AnimEnd:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        if(_currentMessageCoroutine != null)
            StopCoroutine(_currentMessageCoroutine);
        
        _currentMessageCoroutine = StartCoroutine(
            TypeMessage(message, delays, textContainer, speakingSound)
        );
    }
    
    private static string RemoveMessageTags(string message)
    {
        var result = string.Empty;
    
        for (var i = 0; i < message.Length; i++)
        {
            if(message[i] == '<')
            {
                while (message[i] != '>')
                    i++;
                i++;
            }
    
            result += message[i];
        }
        return result;
    }
    
    private IEnumerator TypeMessage(string message, IReadOnlyList<float> delayBetweenLetters, TMP_Text textContainer, AudioClip speakerSound)
    {
        textContainer.ForceMeshUpdate();

        textContainer.maxVisibleCharacters = 0;

        var currentDelay = DialogueUtility.DefaultSpeed;

        for (var i = 0; i < textContainer.textInfo.characterCount; i++)
        {
            var c = message[i];

            if (!CharactersToIgnore.Contains(c))
            {
                yield return new WaitForSeconds(currentDelay);
                AudioManager.Instance.PlaySound(speakerSound);
            }

            currentDelay = delayBetweenLetters[i];
            textContainer.maxVisibleCharacters++;
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
