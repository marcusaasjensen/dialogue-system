using System.Globalization;
using UnityEngine;

public class NarrativeInput : MonoBehaviour
{
    [SerializeField] private NarrativeController narrativeController;
    [SerializeField] private bool stopInteractAtNarrativeEnd;


    private void Awake() => DialogueVariables.Instance.AddDialogueVariable("cost",1.5f.ToString(CultureInfo.InvariantCulture));

    private void Update()
    {
        SkipDialogueWithSpaceBar();
        InteractWithCharacter();
    }

    private void InteractWithCharacter()
    {
        if (!Input.GetKeyDown(KeyCode.Return) || narrativeController.IsNarrating || 
            (narrativeController.IsNarrativeEndReached && stopInteractAtNarrativeEnd)) return;
        narrativeController.StartNarrative();
    }

    private void SkipDialogueWithSpaceBar()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || narrativeController.IsChoosing ||
            !narrativeController.IsNarrating) return;

        narrativeController.NextNarrative();
    }
}
