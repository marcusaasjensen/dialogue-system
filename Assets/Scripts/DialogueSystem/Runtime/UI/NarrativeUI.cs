using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Utility;
using UnityEngine;

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
                
        /// <summary>
        /// Display the message in the UI.
        /// </summary>
        /// <param name="message">parsed message you can show.</param>
        public abstract void DisplayMessage(string message);
        /// <summary>
        /// Display dialogue bubble with message data and character data.
        /// </summary>
        /// <param name="messageData">Message data with information about the text to show and the character's name.</param>
        /// <param name="characterData">Character data with information about the character's UI.</param>
        public abstract void DisplayDialogueBubble(DialogueMessage messageData, CharacterData characterData);
        /// <summary>
        /// Display the possible dialogue options the player can choose.
        /// </summary>
        /// <param name="options">List of dialogue options with each containing information about the chosen option.</param>
        /// <param name="disableAlreadyChosenOptions">Disable option if it has already been chosen by the player.</param>
        /// <param name="chooseNarrativePath">Function to call when the player chooses an option. It is generally the function that changes the path of the narrative.</param>
        public abstract void DisplayOptions(List<DialogueOption> options, bool disableAlreadyChosenOptions, ChoosePathDelegate chooseNarrativePath);
        /// <summary>
        /// Display entire message without typing.
        /// </summary>
        public abstract void DisplayAllMessage();
        /// <summary>
        /// If the message is currently being displayed by the typer.
        /// </summary>
        public abstract bool IsMessageDisplaying();
        /// <summary>
        /// Initialize UI values before displaying anything (example: set a default value for the message displayed).
        /// </summary>
        public abstract void InitializeUI();
        /// <summary>
        /// Show or hide the set of dialogue UI.
        /// </summary>
        /// <param name="active">UI Active state.</param>
        public abstract void SetUIActive(bool active);
        /// <summary>
        /// Display the character's sprite if exists or not hidden.
        /// </summary>
        /// <param name="characterStateFace">Character's sprite.</param>
        /// <param name="hideCharacter">Hide character's sprite.</param>
        public abstract void DisplayCharacter(Optional<Sprite> characterStateFace, bool hideCharacter);
    }
}