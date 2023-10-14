using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private NarrativeUI narrativeUI;
    [SerializeField] private NarrativeLoader narrativeLoader;
    [SerializeField] private List<Speaker> speakers;
    [SerializeField] private bool displayChoicesAutomatically = true;
    [SerializeField] private bool disableAlreadyChosenOptions = true;
    [SerializeField] private bool startFromPreviousNarrativePath; //save dialogue state to scriptable object and starts to where the dialogue was left off
    [SerializeField] private bool startNarrationOnStart;
    [SerializeField] private bool resetNarrativeOnLoad;

    private string NarrativePathID { get; set; }
    
    public bool IsChoosing { get; private set; }
    public bool IsNarrating { get; private set; }
    
    private NarrativeNode _currentNarrative;
    private Message _currentMessage = new();
    private Queue<Message> _narrativeQueue;
    private Narrative _narrativeStructure;

    private NarrativeNode _startNode;

    private void Start()
    {
        //In this case, a default narration must be assigned to narrative loader.
        //It allows narration starting without interactions, like in specific scenes.
        if(startNarrationOnStart) BeginNarration();
    }

    public void BeginNarration(DialogueContainer narrativeToLoad = null)
    {
        if(resetNarrativeOnLoad)
            narrativeLoader.ResetNarrative();
        
        _narrativeStructure = narrativeLoader.LoadNarrative(narrativeToLoad);
        
        if (_narrativeStructure == null)
        {
            LogHandler.LogError("Can't start narrative because the narrative was not loaded properly.");
            return;
        }
        
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
        NarrativePathID = startFromPreviousNarrativePath ? narrativeLoader.GetSavedNarrativePathID() : string.Empty;
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
