using UnityEngine;
using UnityEngine.Serialization;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactSource;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactableLayer;

    private const KeyCode InteractionKey = KeyCode.Return;

    private float _inputDirection = 1;

    private void Update()
    {
        CalculateInputDirection();
        InteractWithCharacter();
    }

    private void InteractWithCharacter()
    {
        if (!Input.GetKeyDown(InteractionKey)) return;

        var hit = Physics2D.Raycast(interactSource.position, interactSource.right * _inputDirection, interactionDistance, interactableLayer);

        if (hit.collider == null) return;
        
        var interactable = hit.collider.GetComponent<IInteractable>();
        
        if (interactable is { CanInteract: true })
            interactable.Interact();
    }



    private void CalculateInputDirection()
    {
        var inputValue = Input.GetAxisRaw("Horizontal");
        _inputDirection = inputValue == 0 ? _inputDirection : Input.GetAxisRaw("Horizontal");
    }
    
    private void OnDrawGizmos()
    {
        var rayOrigin = interactSource.position;
        var rayDirection = interactSource.right * _inputDirection;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, rayDirection * interactionDistance);
    }

}