using System.Collections.Generic;
using UnityEngine;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private NarrativeUI narrativeUI;
    [SerializeField] private NarrativeLoader narrativeLoader;
    [SerializeField] private AudioClip narrativeMusic;
    [SerializeField] private bool isLockingPlayer;
    [SerializeField] private List<Speaker> speakers;
    [SerializeField] private bool displayChoicesAutomatically = true;
    [SerializeField] private bool disableAlreadyChosenOptions = true;

    private string NarrativePathID { get; set; }
    
    public bool IsChoosing { get; private set; }
    public bool IsDialogueFinished { get; private set; }
    
    private NarrativeNode _currentNarrative;
    private Message _currentMessage = new();
    private Queue<Message> _narrativeQueue;
    private Narrative _narrativeStructure;

    private void Awake() => _narrativeStructure = narrativeLoader.LoadNarrativeFromData();
    private void Start() => StartNarrative();

    private void StartNarrative()
    {
        IsDialogueFinished = false;
        narrativeUI.InitializeUI();
        if (_narrativeStructure == null)
        {
            Debug.LogError("Can't start narrative because the narrative was not loaded properly.");
            return;
        }
        
        AudioManager.Instance.PlayMusic(narrativeMusic);

        SetupNarrativeEvents();
        StartNewDialogue(_narrativeStructure.NarrativeEntryNode);
    }

    private void ContinueToChoiceAutomatically()
    {
        var continueAutomatically = displayChoicesAutomatically && _narrativeQueue.Count == 0 && 
                                    (_currentNarrative.HasNextChoice() || _currentNarrative.HasChoiceAfterTransition());
        
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
        if (narrativeUI.IsShowingCurrentMessage())
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

    private void SkipCurrentMessage(Message currentMessage) => narrativeUI.ShowAllMessage(currentMessage);
    
    private void FindNextPath()
    {
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

        var buttonList = narrativeUI.DisplayDialogueOptionButtons(_currentNarrative.Options, disableAlreadyChosenOptions);

        for (var i = 0; i < buttonList.Count; i++)
        {
            var newIndex = i;
            buttonList[i].onClick.AddListener(() => ChooseNarrativePath(newIndex));
            buttonList[i].onClick.AddListener(() => narrativeUI.EnableNextNarrationUI());
            buttonList[i].onClick.AddListener(() => buttonList.ForEach(button => Destroy(button.gameObject)));
        }
    }

    private void ShowNextMessage(Message nextMessage)
    {
        var currentSpeaker = speakers?.Find(speaker => speaker.characterName == nextMessage?.Speaker);
        narrativeUI.ShowMessage(currentSpeaker, nextMessage);
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
    
    private void FinishDialogue()
    {
        LogResults();
        narrativeUI.CloseDialogue();
        IsDialogueFinished = true;
    }

    private void LogResults()
    {
        Debug.Log("<color=#2CD3E1>Dialogue finished!</color>");
        Debug.Log($"<color=#2CD3E1>Final narrative path ID: {NarrativePathID}</color>");
    }

}
