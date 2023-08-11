using System.Globalization;
using UnityEngine;

public class NarrativeInput : MonoBehaviour
{
    [SerializeField] private NarrativeController narrativeController;


    private void Awake() => DialogueVariables.Instance.AddDialogueVariable("cost",1.5f.ToString(CultureInfo.InvariantCulture));
    
    private void Update() => SkipDialogueWithSpaceBar();

    private void SkipDialogueWithSpaceBar()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || narrativeController.IsChoosing ||
            narrativeController.IsDialogueFinished) return;

        narrativeController.NextNarrative();
    }
}
