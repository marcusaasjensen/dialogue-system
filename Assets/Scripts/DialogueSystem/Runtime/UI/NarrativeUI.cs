using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.CommandProcessor;
using DialogueSystem.Runtime.Narration;
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
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private Button disabledButtonPrefab;
        [SerializeField] private Transform buttonsParent;
        [SerializeField] private Button nextMessageButton;
        [SerializeField] private Vector2 buttonOffset;
        [SerializeField, Min(1)] private int numberOfColumns = 2;

        [Space, Header("UI Rendering")]
        [SerializeField, CanBeNull] private Image characterSprite;
    
        private List<Button> _currentOptionButtonList;
        private bool _textTyperNotNull;
        private bool _characterSpriteNull;
        
        public event Action OnMessageStart
        {
            add => textTyper.OnTypingStart += value;
            remove => textTyper.OnTypingStart -= value;
        }
        
        public event Action OnMessageEnd
        {
            add => textTyper.OnTypingEnd += value;
            remove => textTyper.OnTypingEnd -= value;
        }
    
        private void Awake()
        {
            _currentOptionButtonList = new List<Button>();
            _characterSpriteNull = characterSprite == null;
            _textTyperNotNull = textTyper != null;
            CloseDialogue();
        }
    
        public delegate void ChoosePathDelegate(int index);

        public void DisplayOptions(List<DialogueOption> options, bool disableChosenOptions,
            ChoosePathDelegate choosePathFunction)
        {
            DisableNextNarrationUI();
            var parentRect = buttonsParent.GetComponent<RectTransform>().rect;

            var columnIndex = 0;
            var rowIndex = 0;

            var numberOfOptionsLeft = options.Count;
            var numberOfOptionsInRow = Mathf.Min(numberOfOptionsLeft, numberOfColumns);

            foreach (var option in options)
            {
                var isDisabledOption = disableChosenOptions && option.HasAlreadyBeenChosen;

                var newOptionButton = ButtonFactory.CreateButton(
                    buttonPrefab,
                    disabledButtonPrefab,
                    buttonsParent,
                    isDisabledOption,
                    DialogueCommandParser.ReplaceVariableTagsByValue(option.Text),
                    delegate
                    {
                        choosePathFunction(options.IndexOf(option));
                        RemoveOptions();
                        EnableNextNarrationUI();
                    });

                var buttonRect = newOptionButton.GetComponent<RectTransform>();
                ButtonFactory.PlaceButton(buttonRect, parentRect, columnIndex, rowIndex, numberOfOptionsInRow, buttonOffset);

                columnIndex++;
                numberOfOptionsLeft--;

                _currentOptionButtonList.Add(newOptionButton);

                if (columnIndex != numberOfColumns) continue;
                numberOfOptionsInRow = Mathf.Min(numberOfOptionsLeft, numberOfColumns);
                columnIndex = 0;
                rowIndex++;
            }
        }

        private void RemoveOptions()
        {
            _currentOptionButtonList.ForEach(button => Destroy(button.gameObject));
            _currentOptionButtonList.Clear();
        }

        private void DisplayCharacterName(string speakerName, bool hide)
        {
            speakerNameText.text = speakerName;
            speakerNameText.maxVisibleCharacters = hide ? 0 : int.MaxValue;
        }
    
        public void DisplayDialogue(DialogueMessage messageData, CharacterData characterData)
        {
            DisplayCharacterName(messageData.CharacterName, messageData.HideCharacter);
            DisplayCharacterSprite(characterData.defaultState.characterFace, messageData.HideCharacter);
        }

        public void DisplayMessage(string text)
        {
            messageTextContainer.text = text;
            if(textTyper) textTyper.TypeText(text, messageTextContainer);
        }

        public void DisplayCharacterSprite(Sprite sprite, bool hideCharacter = false)
        {
            if (_characterSpriteNull) return;
        
            characterSprite!.sprite = sprite;
            characterSprite.gameObject.SetActive(sprite != null && !hideCharacter);
        }

        private void EnableNextNarrationUI() => nextMessageButton.gameObject.SetActive(true);
        private void DisableNextNarrationUI() => nextMessageButton.gameObject.SetActive(false);

        public void CloseDialogue() => gameObject.SetActive(false);
        public void OpenDialogue() => gameObject.SetActive(true);
    
        public void InitializeUI()
        {
            messageTextContainer.text = string.Empty;
            speakerNameText.text = string.Empty;
            DisplayCharacterSprite(null, true);
        }

        public bool IsMessageDisplaying() => _textTyperNotNull && textTyper.IsTyping;

        public void DisplayAllMessage()
        {
            textTyper.FinishTyping();
            messageTextContainer.maxVisibleCharacters = int.MaxValue;
        }
    }
}
