using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.Interaction;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Runtime.UI;
using UnityEngine;

namespace DialogueSystem.Runtime.Command
{
    public class CommandExecutionHandler : MonoBehaviour
    {
        [SerializeField] private TextTyper textTyper;
        [SerializeField] private NarrativeUI narrativeUI;
        [SerializeField] private CharacterSpeaker characterSpeaker;
        [SerializeField] private TextAnimator textAnimator;

        private Queue<DialogueCommand> _commandQueue = new();
        private CharacterData _currentCharacterData;
        private DialogueMessage _currentDialogueMessage;
        private DialogueMonoBehaviour.DialogueEvent[] _events;

        private void Awake() => narrativeUI.OnMessageStart += HandleCommandExecution;
        
        public string ParseDialogueCommands(string message)
        {
            var parsedMessage = DialogueCommandParser.Parse(message);
            
            var processedMessageWithTextTags = parsedMessage.Message;
            var commandList = parsedMessage.Commands;
            
            commandList.Sort((command1, command2) => command1.StartPosition.CompareTo(command2.StartPosition));
            
            _commandQueue = new Queue<DialogueCommand>();
            
            foreach(var commandData in commandList)
            {
                DialogueCommand newCommand;
                
                switch(commandData.Type)
                {
                    case DialogueCommandType.Pause:
                        newCommand = CommandFactory.CreatePauseCommand(commandData, textTyper);
                        break;
                    case DialogueCommandType.Speed:
                        newCommand = CommandFactory.CreateSpeedCommand(commandData, textTyper);
                        break;
                    case DialogueCommandType.DisplayedEmotion:
                        newCommand = CommandFactory.CreateStateCommand(commandData, narrativeUI, characterSpeaker, _currentCharacterData, _currentDialogueMessage.HideCharacter);
                        break;
                    case DialogueCommandType.Animation:
                        newCommand = CommandFactory.CreateAnimationCommand(commandData, textAnimator);
                        break;
                    case DialogueCommandType.MusicStart:
                        newCommand = CommandFactory.CreateMusicCommand(commandData, false);
                        break;
                    case DialogueCommandType.MusicEnd:
                        newCommand = CommandFactory.CreateMusicCommand(commandData, true);
                        break;
                    case DialogueCommandType.SoundEffect:
                        newCommand = CommandFactory.CreateSoundEffectCommand(commandData);
                        break;
                    case DialogueCommandType.Event:
                        newCommand = CommandFactory.CreateEventCommand(commandData, _events);
                        break;
                    default:
                        newCommand = new NullCommand(commandData.StartPosition, commandData.MustExecute);
                        break;
                }

                _commandQueue.Enqueue(newCommand);
            }
            
            return processedMessageWithTextTags;
        }
        
        public void GatherCommandData(DialogueMessage dialogueMessage, CharacterData characterDataData, DialogueMonoBehaviour.DialogueEvent[] events)
        {
            _currentCharacterData = characterDataData;
            _currentDialogueMessage = dialogueMessage;
            _events = events;
        }
        
        public void ExecuteDefaultCommands()
        {
            textTyper.ResetTyper();
            textAnimator.ResetAnimator();
            characterSpeaker.ChangeVoice(_currentCharacterData.SpeakingSound);
            characterSpeaker.ChangePitch(_currentCharacterData.DefaultState.SpeakingSoundPitch);
            characterSpeaker.React(_currentCharacterData.DefaultState.ReactionSound);
        }

        private void HandleCommandExecution() => StartCoroutine(HandleCommandExecutionCoroutine());

        private IEnumerator HandleCommandExecutionCoroutine()
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

            while (_commandQueue.Count > 0 && _commandQueue.Peek().StartPosition == typerPosition)
            {
                commandsAtPosition.Add(_commandQueue.Dequeue());
            }
            
            return commandsAtPosition;
        }

        private void ExecuteCommandsAtPosition(int typerPosition)
        {
            var commands = FindCommandsAtPosition(typerPosition);
            commands.ForEach(command => command.Execute());
        }

        public void ExecuteAllCommands()
        {
            while (_commandQueue.Count > 0)
            {
                var command = _commandQueue.Dequeue();
                if (command.MustExecute)
                {
                    command.Execute();
                }
            }
        }
    }
}
