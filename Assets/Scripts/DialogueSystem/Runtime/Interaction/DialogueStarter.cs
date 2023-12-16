using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using JetBrains.Annotations;
using Scene;
using UnityEngine;

namespace DialogueSystem.Runtime.Interaction
{
    public class DialogueStarter : MonoBehaviour
    {
        [SerializeField] private DialogueContainer narrativeScriptableObject;
        [SerializeField] private NarrativeController narrativeController;
     
        private void Start() => StartDialogue();
        private void Update() => SkipDialogueWithSpaceBar();
     
        private void StartDialogue() => narrativeController.BeginNarration(narrativeScriptableObject);
     
        private void SkipDialogueWithSpaceBar()
        {
            if (!Input.GetKeyDown(KeyCode.Space) || narrativeController.IsChoosing ||
                !narrativeController.IsNarrating) return;

            narrativeController.NextNarrative();
        }
    }
}