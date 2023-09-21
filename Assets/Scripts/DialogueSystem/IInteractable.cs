
public interface IInteractable
{
    // Called when the player interacts with the object.
    void Interact();

    // Check if the object can be interacted with at the moment.
    bool CanInteract { get; }

    // Display a hint or message when the player is near the object.
    string InteractionHint { get; }
}
