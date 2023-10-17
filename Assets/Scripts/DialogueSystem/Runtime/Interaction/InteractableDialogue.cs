using System.Globalization;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace DialogueSystem.Runtime.Interaction
{
    public class InteractableDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueContainer narrativeScriptableObject;
        [SerializeField] private NarrativeController narrativeController;
        [SerializeField] private bool stopInteractAtNarrativeEnd;

        [Header("Hint")]
        [SerializeField] private Transform player;
        [SerializeField] private float hintRadius;
        [SerializeField] private GameObject hint;

        public GameObject InteractionHint => hint;
    
        private bool _hintNull;

        public bool CanInteract => !((stopInteractAtNarrativeEnd && (narrativeScriptableObject?.isNarrativeEndReached ?? false)) || narrativeController.IsNarrating);

        private void Update()
        {
            SkipDialogueWithSpaceBar();
            ShowHint();
        }

        private void Awake()
        {
            _hintNull = hint == null;
            DialogueVariables.Instance.AddDialogueVariable("cost", 1.5f);
        }

        public void Interact()
        {
            // We assume it can interact when using the function
            narrativeController.BeginNarration(narrativeScriptableObject);
        }
    
        private void SkipDialogueWithSpaceBar()
        {
            if (!Input.GetKeyDown(KeyCode.Space) || narrativeController.IsChoosing ||
                !narrativeController.IsNarrating) return;

            narrativeController.NextNarrative();
        }

        private void ShowHint()
        {
            if (_hintNull) return;
        
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