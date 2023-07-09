using System.Collections.Generic;
using UnityEngine;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] private NarrativeUI narrativeUI;
    [SerializeField] private NarrativeLoader narrativeLoader;
    [SerializeField] private bool isLockingPlayer;
    [SerializeField] private List<Speaker> speakers;
    public string NarrativePathID { get; private set; }
    
    private NarrativeNode _currentNarrative;
    private Narrative _narrativeStructure;

    private void Awake()
    {
        _narrativeStructure = narrativeLoader.LoadNarrativeFromData();
    }

    private void Start()
    {
        StartNarrative();
    }
    
    private void StartNarrative()
    {
        _currentNarrative = _narrativeStructure.NarrativeEntryNode;
        _currentNarrative.Dialogue.OnMessageQueueEmpty += DisplayDialogueOptions;
        ContinueNarrative();
    }

    public void ContinueNarrative()
    {
        var nextMessage = _currentNarrative.Dialogue.NextMessage();
        ShowNextMessage(nextMessage);
    }

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
    public void ShowNextMessage(Message nextMessage)
    {
        var currentSpeaker = speakers?.Find(speaker => speaker.characterName == nextMessage?.Speaker);
        narrativeUI.ShowMessage(currentSpeaker, nextMessage);
    }

    public void ChooseNarrativePath(int choiceIndex)
    {
        NarrativePathID += choiceIndex.ToString();
        
        _currentNarrative.Dialogue.OnMessageQueueEmpty -= DisplayDialogueOptions;
        _currentNarrative = _currentNarrative.Options[choiceIndex].TargetNarrative;

        if (_currentNarrative != null)
        {
            _currentNarrative.Dialogue.OnMessageQueueEmpty += DisplayDialogueOptions;
            ContinueNarrative();
        }
        else
            FinishDialogue();

    }

    private void FinishDialogue()
    {
        Debug.Log("Dialogue finished!");
        Debug.Log($"Final narrative path ID: {NarrativePathID}");
        narrativeUI.CloseDialogue();
        
    }
}
