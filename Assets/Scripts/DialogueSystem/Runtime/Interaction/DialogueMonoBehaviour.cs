using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEngine;
using UnityEngine.Serialization;

namespace DialogueSystem.Runtime.Interaction
{
    public abstract class DialogueMonoBehaviour : MonoBehaviour
    {
        [SerializeField] protected DialogueContainer narrativeScriptableObject;
        [SerializeField] protected NarrativeController narrativeController;
        [SerializeField] protected KeyCode skipInput = KeyCode.Space;
        
        protected void SkipDialogueWithInput()
        {
            if (!Input.GetKeyDown(skipInput) || narrativeController.IsChoosing ||
                !narrativeController.IsNarrating) return;

            narrativeController.NextNarrative();
        }

        protected void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject);
    }
}