﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NarrativeWriter : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 0.05f;
    
    private List<DialogueCommand> _dialogueCommands = new();
    private Coroutine _currentMessageCoroutine;
    private static readonly char[] CharactersToIgnore = {' '};

    public bool IsTyping { get; private set; }

    public void WriteMessage(string message, TextMeshProUGUI textContainer, AudioClip speakingSound = null)
    {
        _dialogueCommands = DialogueUtility.ProcessInputString(message, out var processedMessageWithTags);
        
        textContainer.text = processedMessageWithTags;

        var processedMessage = RemoveMessageTags(processedMessageWithTags);
        
        var delays = DelaysBetweenEachCharacter(processedMessage.Length);

        TypeMessage(message, delays, textContainer, speakingSound);
    }


    private void TypeMessage(string message, IReadOnlyList<float> delayBetweenLetters, TMP_Text textContainer, AudioClip speakingSound = null)
    {
        if(_currentMessageCoroutine != null)
            StopCoroutine(_currentMessageCoroutine);
        
        _currentMessageCoroutine = StartCoroutine(
            TypeMessageCoroutine(message, delayBetweenLetters, textContainer, speakingSound)
        );
    }
    
    private float[] DelaysBetweenEachCharacter(int messageLength)
    {
        var delays = new float[messageLength];

        var defaultDelay = defaultSpeed;

        for (var i = 0; i < delays.Length; i++)
        {
            var commandAtPosition = _dialogueCommands.Find(command => command.Position == i);

            if (commandAtPosition == null)
            {
                delays[i] = defaultDelay;
                continue;
            }

            switch (commandAtPosition.Type)
            {
                case DialogueCommandType.TextSpeedChange:
                    defaultDelay = commandAtPosition.FloatValue;
                    delays[i] = defaultDelay;
                    break;
                case DialogueCommandType.Pause:
                    delays[i] = commandAtPosition.FloatValue;
                    break;
                case DialogueCommandType.AnimStart:
                case DialogueCommandType.AnimEnd:
                default:
                    break;
            }
        }

        return delays;
    }

    private static string RemoveMessageTags(string message)
    {
        var result = string.Empty;
        var i = 0;

        while (i < message.Length)
        {
            if (message[i] == '<')
            {
                while (i < message.Length && message[i] != '>') 
                    i++;
                i++;
            }
            else
            {
                result += message[i];
                i++;
            }
        }

        return result;
    }
    
    private IEnumerator TypeMessageCoroutine(string message, IReadOnlyList<float> delayBetweenLetters, TMP_Text textContainer, AudioClip speakingSound = null)
    {
        IsTyping = true;
        textContainer.ForceMeshUpdate();

        textContainer.maxVisibleCharacters = 0;

        var currentDelay = defaultSpeed;

        for (var i = 0; i < textContainer.textInfo.characterCount; i++)
        {
            var c = message[i];

            if (!CharactersToIgnore.Contains(c))
            {
                yield return new WaitForSeconds(currentDelay);
                AudioManager.Instance.PlaySound(speakingSound);
            }

            currentDelay = delayBetweenLetters[i];
            textContainer.maxVisibleCharacters++;
        }
        
        EndMessage();
    }

    public void EndMessage()
    {
        if (_currentMessageCoroutine == null) return;
        StopCoroutine(_currentMessageCoroutine);
        IsTyping = false;
    }
}