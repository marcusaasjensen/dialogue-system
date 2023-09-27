﻿using UnityEngine;
using UnityEngine.Serialization;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactSource;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private PlayerController playerController;

    private const KeyCode InteractionKey = KeyCode.Return;
    private Vector3 _rayDirection = default;
    
    private void Update()
    {
        CalculateInputDirection();
        InteractWithCharacter();
    }
    
    private void CalculateInputDirection() => 
        _rayDirection = playerController.InputDirection == Vector3.zero ? _rayDirection : playerController.InputDirection;


    private void InteractWithCharacter()
    {
        if (!Input.GetKeyDown(InteractionKey)) return;

        var ray = new Ray(interactSource.position, _rayDirection);

        if (!Physics.Raycast(ray, out var hit, interactionDistance, interactableLayer)) return;

        var interactable = hit.collider.GetComponent<IInteractable>();

        if (interactable is { CanInteract: true })
            interactable.Interact();
    }

    private void OnDrawGizmos()
    {
        var rayOrigin = interactSource.position;
        var rayDirection = _rayDirection;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, rayDirection * interactionDistance);
    }
    
    

}