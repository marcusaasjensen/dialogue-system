using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Runtime.Utility;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem.Runtime.UI
{
    public class NarrativeUI : MonoBehaviour
    {
        [Header("UI Texts")]
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI messageTextContainer;
        [SerializeField] private TextTyper textTyper;

        [Space, Header("UI Buttons")] 
        [SerializeField] private Transform buttonsParent;
        [SerializeField] private Button dialogueOptionButtonPrefab;
        [SerializeField] private Button disabledOptionButtonPrefab;
        [SerializeField] private Button nextMessageButton;
        [SerializeField] private Vector2 buttonOffset;
        [SerializeField, Min(1)] private int numberOfColumns = 2;

        [Space, Header("UI Rendering")]
        [SerializeField, CanBeNull] private Image speakingCharacterSprite;
    
        private List<Button> _currentOptionButtonList;
        private bool _disabledButtonPrefabNotNull;
        private bool _narrativeWriterNotNull;
        private bool _speakingCharacterSpriteNull;

        public event Action OnMessageEnd;
    
        private void Awake()
        {
            _currentOptionButtonList = new List<Button>();
            _speakingCharacterSpriteNull = speakingCharacterSprite == null;
            _narrativeWriterNotNull = textTyper != null;
            _disabledButtonPrefabNotNull = disabledOptionButtonPrefab != null;
            CloseDialogue();
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
                DialogueParser.ProcessInputString(option.Text, out var processedMessageWithTags); //only process variable tags
                optionTextContainer.text = processedMessageWithTags;

                var pathIndex = options.IndexOf(option);
            
                newOptionButton.onClick.AddListener(
                    delegate 
                    {
                        choosePathFunction(pathIndex); 
                        EnableNextNarrationUI(); 
                        RemoveOptions(); 
                    });

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
    
        public void DisplayDialogueBubble(DialogueMessage messageData, Sprite characterFace)
        {
            DisplaySpeakerName(messageData.CharacterName, messageData.HideCharacter);
            DisplaySpeakerSprite(characterFace, messageData.HideCharacter);
        }

        public void DisplayMessage(string text)
        {
            messageTextContainer.text = text;
            if(textTyper) textTyper.TypeText(text, messageTextContainer);
            StartCoroutine(WaitMessageEnd());
        }

        private IEnumerator WaitMessageEnd()
        {
            if (_narrativeWriterNotNull)
                yield return new WaitUntil(() => textTyper.IsTyping == false);

            OnMessageEnd?.Invoke(); //move to text typer?
            yield return null;
        }

        public void DisplaySpeakerSprite(Sprite sprite, bool hideCharacter = false)
        {
            if (_speakingCharacterSpriteNull) return;
        
            speakingCharacterSprite!.sprite = sprite;
            speakingCharacterSprite.gameObject.SetActive(sprite != null && !hideCharacter);
        }

        private void EnableNextNarrationUI() => nextMessageButton.gameObject.SetActive(true);
        private void DisableNextNarrationUI() => nextMessageButton.gameObject.SetActive(false);

        public void CloseDialogue() => gameObject.SetActive(false);
        public void OpenDialogue() => gameObject.SetActive(true);
    
        public void InitializeUI()
        {
            messageTextContainer.text = string.Empty;
            speakerNameText.text = string.Empty;
            DisplaySpeakerSprite(null, true);
        }

        public bool IsMessageDisplaying() => _narrativeWriterNotNull && textTyper.IsTyping;

        public void DisplayAllMessage()
        {
            StartCoroutine(WaitMessageEnd());
            textTyper.FinishTyping();
            messageTextContainer.maxVisibleCharacters = int.MaxValue;
            //execute commands of all position?
        }
    }
}
