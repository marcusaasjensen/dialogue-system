using UnityEngine;

public interface IInteractable
{
    void Interact();
    bool CanInteract { get; }
    GameObject InteractionHint { get; }
}
