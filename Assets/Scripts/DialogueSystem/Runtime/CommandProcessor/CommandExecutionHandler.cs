using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Audio;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Runtime.UI;
using UnityEngine;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public class CommandExecutionHandler : MonoBehaviour
    {
        [SerializeField] private TextTyper textTyper;
        [SerializeField] private NarrativeUI narrativeUI;
        [SerializeField] private CharacterSpeaker characterSpeaker;

        private Queue<DialogueCommand> _commandQueue = new();
        private CharacterData _currentCharacterData;
        private DialogueMessage _currentDialogueMessage;

        private void Awake() => narrativeUI.OnMessageStart += HandleCommandExecution;

        public string ParseDialogueCommands(string message)
        {
            var commandList = DialogueCommandParser.Parse(message, out var processedMessageWithTextTags);
            
            commandList.Sort((command1, command2) => command1.Position.CompareTo(command2.Position));
            
            _commandQueue = new Queue<DialogueCommand>();
            
            foreach(var commandData in commandList)
            {
                DialogueCommand newCommand;
                
                switch(commandData.Type)
                {
                    case DialogueCommandType.Pause:
                        newCommand = CommandFactory.CreatePauseCommand(commandData, textTyper);
                        break;
                    case DialogueCommandType.TextSpeedChange:
                        newCommand = CommandFactory.CreateSpeedCommand(commandData, textTyper);
                        break;
                    case DialogueCommandType.DisplayedEmotion:
                        newCommand = CommandFactory.CreateStateCommand(commandData, narrativeUI, characterSpeaker, _currentCharacterData);
                        break;
                    case DialogueCommandType.AnimStart:
                    case DialogueCommandType.AnimEnd:
                    case DialogueCommandType.Interaction:
                    case DialogueCommandType.MusicStart:
                        newCommand = CommandFactory.CreateMusicCommand(commandData);
                        break;
                    case DialogueCommandType.MusicEnd:
                        newCommand = CommandFactory.CreateMusicCommand(commandData, true);
                        break;
                    case DialogueCommandType.SoundEffect:
                    case DialogueCommandType.CameraShake:
                    default:
                        newCommand = new NullCommand(commandData.Position, commandData.MustExecute);
                        break;
                }

                _commandQueue.Enqueue(newCommand);
            }
            
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
            characterSpeaker.ChangePitch(_currentCharacterData.defaultState.speakingSoundPitch);
            characterSpeaker.React(_currentCharacterData.defaultState.reactionSound);
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
            
            while (_commandQueue.Count > 0 && _commandQueue.Peek().Position == typerPosition)
                commandsAtPosition.Add(_commandQueue.Dequeue());
            
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
                if (command.MustExecute) command.Execute();
            }
        }
    }
}
