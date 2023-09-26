using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private NarrativeController narrativeController;

    private Transform _transform;

    private void Awake() => _transform = transform;

    private void Update()
    {
        if (narrativeController.IsNarrating) return;
        
        var inputDirection = Input.GetAxisRaw("Horizontal");
        var vectorMovement = inputDirection * new Vector2(1, 0);
        
        _transform.Translate( Time.deltaTime * moveSpeed * vectorMovement.normalized);
    }
}
