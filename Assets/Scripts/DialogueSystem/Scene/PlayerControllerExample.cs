using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace DialogueSystem.Scene
{
    public class PlayerControllerExample : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private NarrativeController narrativeController;
        [SerializeField] private float minMaxPositionValue;
    
        public Vector3 InputDirection { get; private set; }
    
        private Transform _transform;

        private void Awake() => _transform = transform;

        private void FixedUpdate()
        {
            if (narrativeController.IsNarrating)
            {
                return;
            }
            InputDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
            _transform.Translate( Time.deltaTime * moveSpeed * InputDirection.normalized);
            var position = _transform.position;
            position = new Vector3(Mathf.Clamp(position.x, -minMaxPositionValue, minMaxPositionValue), position.y, Mathf.Clamp(position.z, -minMaxPositionValue, minMaxPositionValue));
            _transform.position = position;
        }
    }
}
