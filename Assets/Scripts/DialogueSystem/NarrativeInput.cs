using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeInput : MonoBehaviour
{
    [SerializeField] private NarrativeController narrativeController;
    
    private void Update() => SkipDialogueWithSpaceBar();

    private void SkipDialogueWithSpaceBar()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || narrativeController.IsChoosing ||
            narrativeController.IsDialogueFinished) return;

        narrativeController.ContinueNarrative();
    }
}
