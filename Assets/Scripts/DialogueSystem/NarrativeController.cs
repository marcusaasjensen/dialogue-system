using System.Collections.Generic;
using UnityEngine;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private NarrativeUI narrativeUI;
    [SerializeField] private NarrativeLoader narrativeLoader;
    [SerializeField] private AudioClip narrativeMusic;
    [SerializeField] private List<Speaker> speakers;
    [SerializeField] private bool startNarrationOnStart = false;
    [SerializeField] private bool displayChoicesAutomatically = true;
    [SerializeField] private bool disableAlreadyChosenOptions = true;
    [SerializeField] private bool startFromPreviousNarrativePath; //save dialogue state to scriptable object and starts to where the dialogue was left off

    private string NarrativePathID { get; set; }
    
    public bool IsChoosing { get; private set; }
    public bool IsNarrating { get; private set; }
    public bool IsNarrativeEndReached { get; private set; }
    
    private NarrativeNode _currentNarrative;
    private Message _currentMessage = new();
    private Queue<Message> _narrativeQueue;
    private Narrative _narrativeStructure;

    private NarrativeNode _startNode;

    private void Awake() => _narrativeStructure = narrativeLoader.LoadNarrativeFromData(); //to adapt with dialogue that is not loaded at start of interaction

    private void Start()
    {
        if(startNarrationOnStart)
            BeginNarration();
    }

    private void BeginNarration() //extract method to other responsible class
    {
        AudioManager.Instance.PlayMusic(narrativeMusic);
        StartNarrative();
    }

    public void StartNarrative()
    {
        IsNarrativeEndReached = false;
        IsNarrating = true;
        
        narrativeUI.gameObject.SetActive(true); //to change with close dialogue that completely deactivate gameobject!
        narrativeUI.InitializeUI();
        
        if (_narrativeStructure == null)
        {
            Debug.LogError("Can't start narrative because the narrative was not loaded properly.");
            return;
        }

        SetupNarrativeEvents();
        
        _startNode = FindStartNode();
        StartNewDialogue(_startNode);
    }

    private NarrativeNode FindStartNode()
    {
        NarrativePathID = startFromPreviousNarrativePath ? narrativeLoader.GetSavedNarrativePathID() : string.Empty;
        return FindStartNodeFromPath(NarrativePathID, _narrativeStructure.NarrativeEntryNode);
    }

    private static NarrativeNode FindStartNodeFromPath(string pathID, NarrativeNode firstNode)
    {
        if (string.IsNullOrEmpty(pathID))
            return firstNode;

        var node = firstNode;

        while (node != null)
        {
            if (string.IsNullOrEmpty(pathID)) return node;

            if (node.IsCheckpoint)
            {
                pathID = pathID.Substring(1, pathID.Length - 1);
                node = node.DefaultPath;
                continue;
            }
            
            if (node.IsTransitionNode())
            {
                node = node.DefaultPath;
                continue;
            }

            if (node.Options.Count == 0)
                return null;

            var optionIndex = (int) char.GetNumericValue(pathID[0]);

            node.Options[optionIndex].HasAlreadyBeenChosen = true;
            node = node.Options[optionIndex].TargetNarrative;
            
            pathID = pathID.Substring(1, pathID.Length - 1);
        }

        return null;
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
            Debug.Log($"<color=#FAE392>Skip</color>");
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
            NarrativePathID += ".";
            FinishDialogue();
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
        narrativeUI.DisplayDialogueOptionButtons(_currentNarrative.Options, disableAlreadyChosenOptions, ChooseNarrativePath);
    }

    private void ShowNextMessage(Message nextMessage)
    {
        var currentSpeaker = speakers?.Find(speaker => speaker.characterName == nextMessage?.SpeakerName);
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

    private void FinishDialogue()
    {
        LogResults();
        
        narrativeLoader.SaveNarrativePath(NarrativePathID);
        
        narrativeUI.CloseDialogue();
        IsNarrating = false;
        
        if(_currentNarrative.IsTipNarrativeNode())
            IsNarrativeEndReached = true;
    }

    private void LogResults()
    {
        LogHandler.Log("Dialogue finished!", "#2CD3E1");
        LogHandler.Log($"Final narrative path ID: {NarrativePathID}", "#2CD3E1");
    }
}
