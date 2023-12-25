using DialogueSystem.Data;
using UnityEngine;

namespace Scene
{
    public class ExampleSceneScript : MonoBehaviour
    {
        private void Awake()
        {
            DialogueVariableData.Instance.RemoveAllDialogueVariables();
            DialogueVariableData.Instance.ListAllDialogueVariables();
            DialogueVariableData.Instance.AddDialogueVariable("cost_coffee", 1.5f);
            DialogueVariableData.Instance.AddDialogueVariable("cost_bread", 1);
            DialogueVariableData.Instance.AddDialogueVariable("character", "John");
            DialogueVariableData.Instance.ListAllDialogueVariables();
        }
    }
}