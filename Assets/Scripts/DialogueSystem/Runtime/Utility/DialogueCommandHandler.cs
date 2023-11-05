using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Runtime.UI;
using Scene;
using UnityEngine;

namespace DialogueSystem.Runtime.Utility
{
    public class DialogueCommandHandler : MonoBehaviour
    {
        [SerializeField] private TextTyper textTyper;
        [SerializeField] private NarrativeUI narrativeUI;
        [SerializeField] private CharacterSpeaker characterSpeaker;

        private Queue<DialogueCommand> _commandQueue = new();
        private CharacterData _currentCharacterData;
        private DialogueMessage _currentDialogueMessage;

        private void Awake()
        {
            textTyper.OnTypingStart += HandleDialogueCommands;
        }

        public string ParseDialogueCommands(string message)
        {
            var commandList = DialogueParser.ProcessInputString(message, out var processedMessageWithTextTags);
            commandList.Sort((command1, command2) => command1.Position.CompareTo(command2.Position));
            _commandQueue = new Queue<DialogueCommand>(commandList);
            return processedMessageWithTextTags;
        }
        
        public void GatherCommandData(DialogueMessage dialogueMessage, CharacterData characterDataData)
        {
            _currentCharacterData = characterDataData;
            _currentDialogueMessage = dialogueMessage;
        }
        
        public void ExecuteDefaultCommands()
        {
            textTyper.ResetTyper();
            characterSpeaker.ChangeVoice(_currentCharacterData.speakingSound);
            characterSpeaker.ChangePitch(_currentCharacterData.defaultBehaviour.speakingSoundPitch);
            ExecuteSpeedCommand();
        }

        private void HandleDialogueCommands() => StartCoroutine(HandleDialogueCommandsCoroutine());

        private IEnumerator HandleDialogueCommandsCoroutine()
        {
            while (textTyper.IsTyping)
            {
                ExecuteCommandsAtPosition(textTyper.TyperPosition);
                yield return null;
            }
        }

        private List<DialogueCommand> FindCommandsAtPosition(int typerPosition)
        {
            var commandsAtPosition = new List<DialogueCommand>();
            
            while (_commandQueue.Count > 0 && _commandQueue.Peek().Position == typerPosition)
                commandsAtPosition.Add(_commandQueue.Dequeue());
            
            return commandsAtPosition;
        }

        private void ExecuteCommandsAtPosition(int typerPosition)
        {
            var commands = FindCommandsAtPosition(typerPosition);
            if(commands.Count == 0) return;
            commands.ForEach(ExecuteCommand);
        }

        private void ExecuteCommand(DialogueCommand command)
        {
            switch (command.Type)
            {
                case DialogueCommandType.TextSpeedChange:
                    ExecuteSpeedCommand(command.FloatValue);
                    break;
                case DialogueCommandType.Pause:
                    ExecutePauseCommand(command.FloatValue);
                    break;
                case DialogueCommandType.AnimStart:
                case DialogueCommandType.AnimEnd:
                case DialogueCommandType.DisplayedEmotion:
                    ExecuteEmotionCommand(command.EmotionValue);
                    break;
                case DialogueCommandType.Interaction:
                case DialogueCommandType.MusicStart:
                    //REPLACE MUSIC STARTER TO HERE
                case DialogueCommandType.SoundEffect:
                case DialogueCommandType.MusicEnd:
                case DialogueCommandType.CameraShake:
                default:
                    break;
            }
        }

        private void ExecuteSpeedCommand(float newSpeed = -1)
        {
            textTyper.ChangeSpeed(newSpeed);
        }

        private void ExecutePauseCommand(float pauseDuration)
        {
            textTyper.Pause(pauseDuration);
        }

        private void ExecuteEmotionCommand(Emotion emotion)
        {
            var characterEmotionBehaviour = _currentCharacterData.GetBehaviourByEmotion(emotion);

            narrativeUI.DisplaySpeakerSprite(characterEmotionBehaviour.characterFace);
            
            characterSpeaker.React(characterEmotionBehaviour.emotionSound);
            characterSpeaker.ChangePitch(characterEmotionBehaviour.speakingSoundPitch);
            // Also animate speaker to their emotion
        }

        public void ExecuteAllCommands()
        {
            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                if (command.MustExecute) ExecuteCommand(command);
            }
        }
    }
}
