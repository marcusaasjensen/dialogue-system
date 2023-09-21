using UnityEngine;

public class InteractableDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private NarrativeController narrativeController;
    [SerializeField] private bool stopInteractAtNarrativeEnd;
    [SerializeField] private float interactionZoneRadius;

    public bool CanInteract { get; private set; }
    public string InteractionHint { get; }

    private void Start()
    {
        CanInteract = true;
    }
    
    public void Interact()
    {
        if (!CanInteract || narrativeController.IsNarrating ||
            (narrativeController.IsNarrativeEndReached && stopInteractAtNarrativeEnd)) return;
        
        narrativeController.StartNarrative();
    }

}