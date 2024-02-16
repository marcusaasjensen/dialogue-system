using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace DialogueSystem.Runtime.Interaction
{
    public abstract class DialogueMonoBehaviour : MonoBehaviour
    {
        [System.Serializable]
        public class DialogueEvent
        {
            [field: SerializeField] public string EventName { get; private set; }
            [field: SerializeField] public UnityEvent OnDialogueEvent { get; private set; }
            
            public void InvokeEvent() => OnDialogueEvent?.Invoke();
        }
        
        [SerializeField] protected DialogueContainer narrativeScriptableObject;
        [SerializeField] protected NarrativeController narrativeController;
        [SerializeField] protected KeyCode skipInput = KeyCode.Space;
        [SerializeField] protected DialogueEvent[] dialogueEvents;
        
        /// <summary>
        /// Skips the current dialogue text if the player presses the skip input.
        /// </summary>
        protected void SkipDialogueWithInput()
        {
            if (!Input.GetKeyDown(skipInput) || narrativeController.IsChoosing ||
                !narrativeController.IsNarrating)
            {
                return;
            }

            narrativeController.NextNarrative();
        }

        /// <summary>
        /// Start the dialogue using the narrative controller and by loading the narrative scriptable object.
        /// </summary>
        protected void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject, dialogueEvents);
    }
}