using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Scene
{
    public class ExampleSceneScript : MonoBehaviour
    {
        private void Awake()
        {
            DialogueVariableData.Instance.RemoveAllDialogueVariables();
            DialogueVariableData.Instance.AddDialogueVariable("cost_coffee", 1.5f);
            DialogueVariableData.Instance.AddDialogueVariable("cost_bread", 1);
            DialogueVariableData.Instance.AddDialogueVariable("character", "John");
            DialogueVariableData.Instance.AddDialogueVariable("tmp", 0);
            DialogueVariableData.Instance.RemoveDialogueVariable("tmp");
            DialogueVariableData.Instance.ListAllDialogueVariables();
        }
    }
}