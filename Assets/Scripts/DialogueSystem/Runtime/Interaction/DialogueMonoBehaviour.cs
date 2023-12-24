using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace DialogueSystem.Runtime.Interaction
{
    public abstract class DialogueMonoBehaviour : MonoBehaviour
    {
        [SerializeField] protected DialogueContainer narrativeScriptableObject;
        [SerializeField] protected NarrativeController narrativeController;
        [SerializeField] protected KeyCode skipInput = KeyCode.Space;
        
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
        protected void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject);
    }
}