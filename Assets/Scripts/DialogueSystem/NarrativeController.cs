using System.Collections.Generic;
using UnityEngine;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private NarrativeUI narrativeUI;
    [SerializeField] private NarrativeLoader narrativeLoader;
    [SerializeField] private AudioClip narrativeMusic;
    [SerializeField] private bool isLockingPlayer;
    [SerializeField] private List<Speaker> speakers;
    public string NarrativePathID { get; private set; }
    
    private NarrativeNode _currentNarrative;
    private Message _currentMessage;
    private Narrative _narrativeStructure;

    private void Awake() => _narrativeStructure = narrativeLoader.LoadNarrativeFromData();
    private void Start() => StartNarrative();

    private void StartNarrative()
    {
        AudioManager.Instance.PlayMusic(narrativeMusic);
        _currentNarrative = _narrativeStructure.NarrativeEntryNode;
        _currentNarrative.Dialogue.OnLastMessage += DisplayDialogueOptions;
        ContinueNarrative();
    }

    public void ContinueNarrative()
    {
        if (narrativeUI.IsShowingCurrentMessage())
        {
            SkipCurrentMessage(_currentMessage);
            Debug.Log($"<color=#FAE392>Skip</color>");
            return;
        }
        
        _currentMessage = _currentNarrative.Dialogue.NextMessage();
        ShowNextMessage(_currentMessage);
    }

    private void SkipCurrentMessage(Message currentMessage) => narrativeUI.ShowAllMessage(currentMessage);
    private void DisplayDialogueOptions()
    {
        if(_currentNarrative.Options.Count == 0)
        {
            FinishDialogue();
            return;
        }

        var buttonList = narrativeUI.DisplayDialogueOptionButtons(_currentNarrative.Options);

        for (var i = 0; i < buttonList.Count; i++)
        {
            var newIndex = i;
            buttonList[i].onClick.AddListener(() => ChooseNarrativePath(newIndex));
            buttonList[i].onClick.AddListener(() => narrativeUI.EnableNarrationUI());
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
        
        _currentNarrative.Dialogue.OnLastMessage -= DisplayDialogueOptions;
        _currentNarrative = _currentNarrative.Options[choiceIndex].TargetNarrative;

        if (_currentNarrative != null)
        {
            _currentNarrative.Dialogue.OnLastMessage += DisplayDialogueOptions;
            ContinueNarrative();
        }
        else
            FinishDialogue();

    }

    private void FinishDialogue()
    {
        Debug.Log("<color=#2CD3E1>Dialogue finished!</color>");
        Debug.Log($"<color=#2CD3E1>Final narrative path ID: {NarrativePathID}</color>");
        narrativeUI.CloseDialogue();
        
    }

}
