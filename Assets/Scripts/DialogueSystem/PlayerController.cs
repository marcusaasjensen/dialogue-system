using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private NarrativeController narrativeController;
    
    public Vector3 InputDirection { get; private set; }
    
    private Transform _transform;

    private void Awake() => _transform = transform;

    private void FixedUpdate()
    {
        if (narrativeController.IsNarrating) return;
        InputDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        _transform.Translate( Time.deltaTime * moveSpeed * InputDirection.normalized);
    }
}
