using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Command;
using DialogueSystem.Runtime.UI;
using DialogueSystem.Runtime.Utility;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.Narration
{
    [RequireComponent(typeof(NarrativeLoader)), RequireComponent(typeof(CommandExecutionHandler))]
    public class NarrativeController : MonoBehaviour
    {
        [SerializeField] private NarrativeUI narrativeUI;
        [SerializeField] private NarrativeLoader narrativeLoader;
        [SerializeField] private CommandExecutionHandler commandExecutionHandler;
        
        [Header("Options")]
        [SerializeField] private bool resetNarrativeOnLoad;
        
        [Space, Header("Default Values"), SerializeField]
        private CharacterData defaultCharacterData;

        private string NarrativePathID { get; set; }
    
        public bool IsChoosing { get; private set; }
        public bool IsNarrating { get; private set; }
    
        private NarrativeNode _currentNarrative;
        private DialogueMessage _currentDialogueMessage = new();
        private Queue<DialogueMessage> _narrativeQueue;
        private Narrative _narrative;
        private NarrativeNode _startNode;

        private const string PathSeparator = ".";

        
        public void BeginNarration(DialogueContainer narrativeToLoad)
        {
            _narrative = narrativeLoader.LoadNarrative(narrativeToLoad);

            if (_narrative == null)
            {
                LogHandler.Alert("Can't start narrative because the narrative was not loaded properly.");
                return;
            }

            if (resetNarrativeOnLoad)
            {
                narrativeLoader.ResetNarrative();
            }
        
            StartNarrative();
        }

        private void StartNarrative()
        {
            IsNarrating = true;
        
            narrativeUI.SetUIActive(true);
            narrativeUI.InitializeUI();

            SetupNarrativeEvents();
        
            _startNode = GetStartNode();
            StartNewDialogue(_startNode);
        }

        private NarrativeNode GetStartNode()
        {
            NarrativePathID = narrativeLoader.GetSavedNarrativePathID();
            return _narrative.FindStartNodeFromPath(NarrativePathID);
        }

        private void ContinueToChoiceAutomatically()
        {
            var continueAutomatically = _narrativeQueue.Count == 0 && 
                                        (_currentNarrative.HasNextChoice() || _currentNarrative.HasChoiceAfterSimpleNode() 
                                            && !_currentNarrative.IsCheckpoint);

            if (!continueAutomatically)
            {
                return;
            }

            FindNextPath();
        }

        private void StartNewDialogue(NarrativeNode narrative)
        {
            if (narrative == null)
            {
                return;
            }
            _currentNarrative = narrative;
            _narrativeQueue = new Queue<DialogueMessage>(narrative.Dialogue);
            NextNarrative();
        }

        public void NextNarrative()
        {
            IsChoosing = false;
            if (narrativeUI.IsMessageDisplaying())
            {
                SkipCurrentMessage();
                LogHandler.Log("Skip", LogHandler.Color.Yellow);
                return;
            }
        
            ContinueNarrative();
        }

        private void ContinueNarrative()
        {
            if (_narrativeQueue == null)
            {
                FinishDialogue();
                return;
            }
        
            if (_narrativeQueue.Count == 0)
            {
                FindNextPath();
                return;
            }

            _currentDialogueMessage = _narrativeQueue.Dequeue();
            ShowNextMessage(_currentDialogueMessage);
        }

        private void SkipCurrentMessage()
        {
            narrativeUI.DisplayAllMessage();
            commandExecutionHandler.ExecuteAllCommands();
        }

        private void FindNextPath()
        {
            if (_currentNarrative.IsCheckpoint)
            {
                FinishAtCheckpoint();
                return;
            }
        
            if (_currentNarrative.IsSimpleNode())
            {
                StartNewDialogue(_currentNarrative.DefaultPath);
                return;
            }

            if (_currentNarrative.HasNextChoice())
            {
                SetupDialogueOptions();
                return;
            }

            if (!_currentNarrative.IsTipNarrativeNode())
            {
                return;
            }
            FinishDialogue();
        }

        private void SetupDialogueOptions()
        {
            IsChoosing = true;
            narrativeUI.DisplayOptions(_currentNarrative.Options, _currentNarrative.DisableAlreadyChosenOptions, ChooseNarrativePath);
        }
        
        private CharacterData GetCharacter(string characterName)
        {
            var character = _narrative.FindCharacter(characterName);
            return character ? character : defaultCharacterData;
        }

        private void ShowNextMessage(DialogueMessage nextDialogueMessage)
        {
            var currentSpeakerData = GetCharacter(nextDialogueMessage.CharacterName);
            
            //Gather message commands and data
            commandExecutionHandler.GatherCommandData(nextDialogueMessage, currentSpeakerData);
            commandExecutionHandler.ExecuteDefaultCommands();
            
            var messageWithoutCommands = commandExecutionHandler.ParseDialogueCommands(nextDialogueMessage.Content);
            
            //Display dialogue ui
            narrativeUI.DisplayDialogueBubble(nextDialogueMessage, currentSpeakerData);
            narrativeUI.DisplayMessage(messageWithoutCommands);
        }

        private void ChooseNarrativePath(int choiceIndex)
        {
            NarrativePathID += choiceIndex.ToString();
        
            UnsetNarrativeEvents();

            _currentNarrative.Options[choiceIndex].HasAlreadyBeenChosen = true;
            var nextNarrative = _currentNarrative.Options[choiceIndex].TargetNarrative;

            if (nextNarrative != null)
            {
                SetupNarrativeEvents();
                StartNewDialogue(nextNarrative);
                return;
            }
        
            FinishDialogue();
        }

        private void SetupNarrativeEvents()
        {
            narrativeUI.OnMessageEnd += ContinueToChoiceAutomatically;
        }

        private void UnsetNarrativeEvents()
        {
            narrativeUI.OnMessageEnd -= ContinueToChoiceAutomatically;
        }

        private void FinishAtCheckpoint()
        {
            NarrativePathID += PathSeparator;
            FinishDialogue();
        }
        
        private void FinishDialogue()
        {
            narrativeUI.SetUIActive(false);
            IsNarrating = false;

            narrativeLoader.SaveNarrativePath(NarrativePathID, _currentNarrative?.IsTipNarrativeNode() ?? false);
        
            LogResults();
        }

        private void LogResults()
        {
            LogHandler.Log("Dialogue finished!", LogHandler.Color.Blue);
            LogHandler.Log($"Final narrative path ID: {NarrativePathID}", LogHandler.Color.Blue);
        }
    }
}
