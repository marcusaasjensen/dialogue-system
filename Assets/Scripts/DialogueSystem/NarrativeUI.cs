using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Space, Header("Buttons")] 
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Button dialogueOptionButtonPrefab;
    [SerializeField] private Button disabledOptionButtonPrefab;
    [SerializeField] private Button nextMessageButton;
    [SerializeField] private Vector2 buttonOffset;
    [SerializeField, Min(1)] private int numberOfColumns = 2;

    [Space, Header("Rendering")]
    [SerializeField, CanBeNull] private Image speakingCharacterSprite;

    [Space, Header("Default Values"), SerializeField]
    private Speaker defaultSpeaker;

    [SerializeField] private int punctuationDelayMultiplier = 10;

    private IEnumerator _currentMessageShowing;
    private bool _disabledButtonPrefabNotNull;

    private static readonly char[] Punctuation = {'.', ',', '!', '?', ':', ';'};
    private static readonly char[] CharactersToIgnore = {' '};

    public event Action OnMessageEnd;

    private void Awake() => _disabledButtonPrefabNotNull = disabledOptionButtonPrefab != null;

    public List<Button> DisplayDialogueOptionButtons(List<DialogueOption> options, bool disableChosenOptions)
    {
        DisableNextNarrationUI();

        var buttonList = new List<Button>();
        
        var buttonRect = dialogueOptionButtonPrefab.GetComponent<RectTransform>().rect;
        var parentRect = buttonsParent.GetComponent<RectTransform>().rect;
        
        var columnIndex = 0;
        var rowIndex = 0;
        
        var optionsLeft = new Queue<DialogueOption>(options);
        var numberOfOptionsInRow = Mathf.Min(optionsLeft.Count, numberOfColumns);
        
        var initialButtonXPosition = (buttonRect.width * (1 - numberOfOptionsInRow) - numberOfOptionsInRow * buttonOffset.x + buttonOffset.x) / 2;
        var initialButtonYPosition = parentRect.height / 2 - buttonRect.height / 2;
        
        foreach(var option in options)
        {
            if (columnIndex == numberOfColumns)
            {
                numberOfOptionsInRow = Mathf.Min(optionsLeft.Count, numberOfColumns);
                initialButtonXPosition = (buttonRect.width * (1 - numberOfOptionsInRow) - numberOfOptionsInRow * buttonOffset.x + buttonOffset.x) / 2;
                columnIndex = 0;
                rowIndex++;
            }

            var isDisabledOption = disableChosenOptions && option.HasAlreadyBeenChosen;
            
            Button newOptionButton;

            if (isDisabledOption)
            {
                newOptionButton = Instantiate(_disabledButtonPrefabNotNull ? disabledOptionButtonPrefab : dialogueOptionButtonPrefab, buttonsParent);
                newOptionButton.interactable = false;
            }
            else
                newOptionButton = Instantiate(dialogueOptionButtonPrefab, buttonsParent);

            var xOffset = columnIndex * (buttonRect.width + buttonOffset.x);
            var yOffset = rowIndex * (buttonRect.height + buttonOffset.y);
            
            newOptionButton.GetComponent<RectTransform>().localPosition = new Vector3(initialButtonXPosition + xOffset, initialButtonYPosition - yOffset,0);
            newOptionButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = option.Text;
        
            columnIndex++;
            optionsLeft.Dequeue();

            buttonList.Add(newOptionButton);
        }
        
        return buttonList;
    }

    public void ShowMessage(Speaker speaker, Message message)
    {
        if (message == null) return;
        
        speakerNameText.text = message.HideCharacter ? "" : message.Speaker;

        if (speaker == null) speaker = defaultSpeaker;

        var defaultSpeakerBehaviour = speaker.defaultBehaviour ?? defaultSpeaker.defaultBehaviour;
        
        var speakerBehaviour =
            speaker.narrativeBehaviours.Find(emotion => emotion.emotionLabel == message.EmotionDisplayed)
            ?? defaultSpeakerBehaviour;
        
        SetupSpeakerSprite(speakerBehaviour.characterFace, message.HideCharacter);
        
        if(_currentMessageShowing != null)
            StopCoroutine(_currentMessageShowing);

        _currentMessageShowing = TypeMessage(message.Content, speakerBehaviour.speakingRhythm, speakerBehaviour.speakingSound);
        StartCoroutine(_currentMessageShowing);
    }

    private void SetupSpeakerSprite(Sprite sprite, bool hideCharacter)
    {
        if (speakingCharacterSprite == null) return;
        
        if(sprite == null || hideCharacter)
            speakingCharacterSprite.gameObject.SetActive(false);
        else
        {
            speakingCharacterSprite.gameObject.SetActive(true);
            speakingCharacterSprite.sprite = sprite;
        }
    } 

    private IEnumerator TypeMessage(string message, float delayBetweenLetters, AudioClip speakerSound)
    {
        messageText.ForceMeshUpdate();
        messageText.text = message;
        messageText.maxVisibleCharacters = 0;

        float currentDelay = 0;
        
        for (var i = 0; i < message.Length; i++)
        {
            if(message[i] == '<')
            {
                while (message[i] != '>')
                    i++;
                i++;
            }

            var c = message[i];

            yield return new WaitForSeconds(currentDelay);
            

            messageText.maxVisibleCharacters++;
            
            currentDelay = delayBetweenLetters;
            
            if (CharactersToIgnore.Contains(c))
                continue;

            if (Punctuation.Contains(c))
                currentDelay = delayBetweenLetters * punctuationDelayMultiplier;

            AudioManager.Instance.PlaySound(speakerSound);
        }

        EndMessage();
    }
    
    public bool IsShowingCurrentMessage() => _currentMessageShowing != null;

    public void ShowAllMessage(Message currentMessage)
    {
        messageText.maxVisibleCharacters = currentMessage.Content.Length;
        EndMessage();
    }

    private void EndMessage()
    {
        StopCoroutine(_currentMessageShowing);
        _currentMessageShowing = null;
        OnMessageEnd?.Invoke();
    }

    public void EnableNextNarrationUI() => nextMessageButton.gameObject.SetActive(true);

    private void DisableNextNarrationUI() => nextMessageButton.gameObject.SetActive(false);

    public void CloseDialogue() => gameObject.SetActive(false);

    public void InitializeUI()
    {
        messageText.text = "";
        speakerNameText.text = "";
        SetupSpeakerSprite(null, true);
    }
}
