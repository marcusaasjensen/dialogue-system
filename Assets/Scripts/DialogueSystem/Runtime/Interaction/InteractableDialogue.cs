using UnityEngine;

namespace DialogueSystem.Runtime.Interaction
{
    public class InteractableDialogue : DialogueMonoBehaviour, IInteractable
    {
        [SerializeField] private bool stopInteractAtNarrativeEnd;
        
        [Header("Hint")]
        [SerializeField] private Transform player;
        [SerializeField] private float hintRadius;
        [SerializeField] private GameObject hint;

        public GameObject InteractionHint => hint;
    
        private bool _hintNull;
        private Quaternion _originalRotation;

        public bool CanInteract =>
            !((stopInteractAtNarrativeEnd && (narrativeScriptableObject is { IsNarrativeEndReached: true })) ||
              narrativeController.IsNarrating);

        private void Update()
        {
            SkipDialogueWithInput();
            ShowHint();
        }

        private void Awake()
        {
            _originalRotation = transform.rotation;
            _hintNull = hint == null;
            narrativeController.OnNarrativeStart.AddListener(TurnCharacterTowardsPlayer);
            narrativeController.OnNarrativeEnd.AddListener(ResetCharacterRotation);
        }

        public void Interact() => StartDialogue();

        private void TurnCharacterTowardsPlayer()
        {
            var position = player.position;
            position.y = transform.position.y;
            transform.LookAt(position);
        }
        
        private void ResetCharacterRotation() => transform.rotation = _originalRotation;

        private void ShowHint()
        {
            if (_hintNull)
            {
                return;
            }
        
            if (!CanInteract)
            {
                hint.SetActive(false);
                return;
            }
            var distance = CalculateHintDistance(player.position, transform.position);
            hint.SetActive(distance <= hintRadius);
        }

        private static float CalculateHintDistance(Vector3 hintPosition, Vector3 playerPosition)
            => Mathf.Sqrt(Mathf.Pow((playerPosition.x - hintPosition.x),2) + Mathf.Pow((playerPosition.z - hintPosition.z),2));
    
        private void OnDrawGizmos()
        {
            var position = transform.position;
            var distance = CalculateHintDistance(player.position, position);
            Gizmos.color = distance <= hintRadius ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(position, hintRadius);
        }
    }
}