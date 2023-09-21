using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NarrativeUI : MonoBehaviour
{
    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [FormerlySerializedAs("messageText")] [SerializeField] private TextMeshProUGUI messageTextContainer;
    [SerializeField] private NarrativeWriter narrativeWriter;

    [Space, Header("UI Buttons")] 
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Button dialogueOptionButtonPrefab;
    [SerializeField] private Button disabledOptionButtonPrefab;
    [SerializeField] private Button nextMessageButton;
    [SerializeField] private Vector2 buttonOffset;
    [SerializeField, Min(1)] private int numberOfColumns = 2;

    [Space, Header("UI Rendering")]
    [SerializeField, CanBeNull] private Image speakingCharacterSprite;

    [Space, Header("Default Values"), SerializeField]
    private Speaker defaultSpeaker;
    
    private List<Button> _currentOptionButtonList;
    private bool _disabledButtonPrefabNotNull;
    private bool _narrativeWriterNotNull;
    private bool _speakingCharacterSpriteNull;

    public event Action OnMessageEnd;
    
    private void Awake()
    {
        _currentOptionButtonList = new List<Button>();
        _speakingCharacterSpriteNull = speakingCharacterSprite == null;
        _narrativeWriterNotNull = narrativeWriter != null;
        _disabledButtonPrefabNotNull = disabledOptionButtonPrefab != null;
    }
    
    public delegate void ChoosePathDelegate(int index);

    public void DisplayDialogueOptionButtons(List<DialogueOption> options, bool disableChosenOptions, ChoosePathDelegate choosePathFunction)
    {
        DisableNextNarrationUI();

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

            var optionTextContainer = newOptionButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            DialogueUtility.ProcessInputString(option.Text, out var processedMessageWithTags);
            optionTextContainer.text = processedMessageWithTags;

            var pathIndex = options.IndexOf(option);
            
            newOptionButton.onClick.AddListener(delegate { choosePathFunction(pathIndex); });
            newOptionButton.onClick.AddListener(EnableNextNarrationUI);
            newOptionButton.onClick.AddListener(RemoveOptions);

            columnIndex++;
            optionsLeft.Dequeue();

            _currentOptionButtonList.Add(newOptionButton);
        }
    }

    private void RemoveOptions()
    {
        _currentOptionButtonList.ForEach(button => Destroy(button.gameObject));
        _currentOptionButtonList.Clear();
    }

    private void DisplaySpeakerName(string speakerName, bool hide)
    {
        speakerNameText.text = speakerName;
        speakerNameText.maxVisibleCharacters = hide ? 0 : int.MaxValue;
    }
    
    public void DisplayMessageWithSpeaker(Speaker speaker, Message message)
    {
        if (message == null) return;
        if (speaker == null) speaker = defaultSpeaker;
        var speakerBehaviour = speaker.GetBehaviourByEmotion(message.EmotionDisplayed);
        
        DisplaySpeakerName(message.SpeakerName, message.HideCharacter);
        DisplaySpeakerSprite(speakerBehaviour.characterFace, message.HideCharacter);
        
        if(narrativeWriter) narrativeWriter.WriteMessage(message.Content, messageTextContainer, speakerBehaviour, speaker.SpeakingSound);

        StartCoroutine(WaitMessageEnd());
    }

    private IEnumerator WaitMessageEnd()
    {
        if (_narrativeWriterNotNull)
            yield return new WaitUntil(() => narrativeWriter.IsTyping == false);

        OnMessageEnd?.Invoke();
        yield return null;
    }

    private void DisplaySpeakerSprite(Sprite sprite, bool hideCharacter)
    {
        if (_speakingCharacterSpriteNull) return;
        
        if(sprite == null || hideCharacter)
            speakingCharacterSprite.gameObject.SetActive(false);
        else
        {
            speakingCharacterSprite.gameObject.SetActive(true);
            speakingCharacterSprite.sprite = sprite;
        }
    }

    private void EnableNextNarrationUI() => nextMessageButton.gameObject.SetActive(true);
    private void DisableNextNarrationUI() => nextMessageButton.gameObject.SetActive(false);

    public void CloseDialogue() => gameObject.SetActive(false);

    public void InitializeUI()
    {
        messageTextContainer.text = "";
        speakerNameText.text = "";
        DisplaySpeakerSprite(null, true);
    }

    public bool IsMessageDisplaying() => _narrativeWriterNotNull && narrativeWriter.IsTyping;

    public void DisplayAllMessage(Message currentMessage)
    {
        narrativeWriter.EndMessage();
        StartCoroutine(WaitMessageEnd());
        messageTextContainer.maxVisibleCharacters = currentMessage.Content.Length;
    }
}
