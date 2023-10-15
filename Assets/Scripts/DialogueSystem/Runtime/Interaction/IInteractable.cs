using UnityEngine;

namespace DialogueSystem.Runtime.Interaction
{
    public interface IInteractable
    {
        void Interact();
        bool CanInteract { get; }
        GameObject InteractionHint { get; }
    }
}
