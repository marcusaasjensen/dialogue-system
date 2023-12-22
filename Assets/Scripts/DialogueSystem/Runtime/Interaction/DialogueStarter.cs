namespace DialogueSystem.Runtime.Interaction
{
    public class DialogueStarter : DialogueMonoBehaviour
    {
        private void Start() => StartDialogue();
        private void Update() => SkipDialogueWithInput();
    }
}