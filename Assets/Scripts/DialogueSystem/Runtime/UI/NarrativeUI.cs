using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.UI
{
    [RequireComponent(typeof(TextTyper))]
    public abstract class NarrativeUI : MonoBehaviour
    {
        [Header("UI Texts")]
        [SerializeField] protected TextTyper textTyper;
        
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
        
        public delegate void ChoosePathDelegate(int index);
        public abstract void DisplayMessage(string message);
        public abstract void DisplayDialogueBubble(DialogueMessage nextDialogueMessage, CharacterData currentSpeakerData = null);
        public abstract void DisplayOptions(List<DialogueOption> options, bool disableAlreadyChosenOptions, ChoosePathDelegate chooseNarrativePath);
        public abstract void DisplayAllMessage();
        public abstract bool IsMessageDisplaying();
        public abstract void InitializeUI();
        public abstract void SetUIActive(bool active);
        public abstract void DisplayCharacter(Optional<Sprite> characterStateCharacterFace, bool hideCharacter = false);
    }
}