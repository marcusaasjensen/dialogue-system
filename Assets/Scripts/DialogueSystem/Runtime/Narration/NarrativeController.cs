using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.NarrationUI;
using DialogueSystem.Runtime.Utility;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.Narration
{
    public class NarrativeController : MonoBehaviour
    {
        [SerializeField] private NarrativeUI narrativeUI;
        [SerializeField] private NarrativeLoader narrativeLoader;
        
        [Header("Options")]
        [SerializeField] private bool displayChoicesAutomatically = true;
        [SerializeField] private bool resetNarrativeOnLoad = false;

        private string NarrativePathID { get; set; }
    
        public bool IsChoosing { get; private set; }
        public bool IsNarrating { get; private set; }
    
        private NarrativeNode _currentNarrative;
        private Message _currentMessage = new();
        private Queue<Message> _narrativeQueue;
        private Narrative _narrativeStructure;
        private NarrativeNode _startNode;

        private const string PathSeparator = ".";

        public void BeginNarration(DialogueContainer narrativeToLoad = null)
        {
            _narrativeStructure = narrativeLoader.LoadNarrative(narrativeToLoad);

            if (_narrativeStructure == null)
            {
                LogHandler.LogError("Can't start narrative because the narrative was not loaded properly.");
                return;
            }
        
            if(resetNarrativeOnLoad)
                narrativeLoader.ResetNarrative();
        
            StartNarrative();
        }

        private void StartNarrative()
        {
            IsNarrating = true;
        
            narrativeUI.gameObject.SetActive(true); //to change with close dialogue that completely deactivate gameobject!
            narrativeUI.InitializeUI();

            SetupNarrativeEvents();
        
            _startNode = GetStartNode();
            StartNewDialogue(_startNode);
        }

        private NarrativeNode GetStartNode()
        {
            NarrativePathID = narrativeLoader.GetSavedNarrativePathID();
            return _narrativeStructure.FindStartNodeFromPath(NarrativePathID);
        }

        private void ContinueToChoiceAutomatically()
        {
            var continueAutomatically = displayChoicesAutomatically && _narrativeQueue.Count == 0 && 
                                        (_currentNarrative.HasNextChoice() || _currentNarrative.HasChoiceAfterTransition() 
                                            && !_currentNarrative.IsCheckpoint);
        
            if (!continueAutomatically) return;

            FindNextPath();
        }

        private void StartNewDialogue(NarrativeNode narrative)
        {
            if (narrative == null) return;
            _currentNarrative = narrative;
            _narrativeQueue = new Queue<Message>(narrative.Dialogue);
            NextNarrative();
        }

        public void NextNarrative()
        {
            IsChoosing = false;
            if (narrativeUI.IsMessageDisplaying())
            {
                SkipCurrentMessage(_currentMessage);
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

            _currentMessage = _narrativeQueue.Dequeue();
            ShowNextMessage(_currentMessage);
        }

        private void SkipCurrentMessage(Message currentMessage) => narrativeUI.DisplayAllMessage(currentMessage);
    
        private void FindNextPath()
        {
            if (_currentNarrative.IsCheckpoint)
            {
                FinishAtCheckpoint();
                return;
            }
        
            if (_currentNarrative.IsTransitionNode())
            {
                StartNewDialogue(_currentNarrative.DefaultPath);
                return;
            }

            if (_currentNarrative.HasNextChoice())
            {
                SetupDialogueOptions();
                return;
            }

            if (!_currentNarrative.IsTipNarrativeNode()) return;
            FinishDialogue();
        }

        private void SetupDialogueOptions()
        {
            IsChoosing = true;
            narrativeUI.DisplayDialogueOptionButtons(_currentNarrative.Options, _currentNarrative.DisableAlreadyChosenOptions, ChooseNarrativePath);
        }

        private void ShowNextMessage(Message nextMessage)
        {
            var currentSpeaker = _narrativeStructure.Speakers?.Find(speaker => speaker.characterName == nextMessage?.SpeakerName);
            narrativeUI.DisplayMessageWithSpeaker(currentSpeaker, nextMessage);
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

        private void SetupNarrativeEvents() => narrativeUI.OnMessageEnd += ContinueToChoiceAutomatically;
        private void UnsetNarrativeEvents() => narrativeUI.OnMessageEnd -= ContinueToChoiceAutomatically;

        private void FinishAtCheckpoint()
        {
            NarrativePathID += PathSeparator;
            FinishDialogue();
        }
        
        private void FinishDialogue()
        {
            narrativeUI.CloseDialogue();
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
