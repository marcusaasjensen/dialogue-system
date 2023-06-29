using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine;

public class ReorderableListExample : EditorWindow
{
    private ReorderableList list;
    
    private const int GUIOffset = 5;

    [MenuItem("Window/Reorderable List Example")]
    private static void OpenWindow()
    {
        var window = GetWindow<ReorderableListExample>();
        window.titleContent = new GUIContent("Reorderable List Example");
        window.Show();
    }

    private void OnEnable()
    {
        // Create a list with initial elements
        //list = new ReorderableList(new List<string> { "Item 1", "Item 2", "Item 3" }, typeof(string), true, true, true, true);
        Message m1 = new Message
        {
            Content = "blablabla",
            EmotionDisplayed = Emotion.None,
            Speaker = "Toto"
        };
        list = new ReorderableList(new List<Message>{m1}, typeof(Message),true, true, true, true);
        // Define how each element in the list should be displayed
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.list[index] as Message;
            //EditorGUI.LabelField(rect, element.Speaker);
            EditorGUI.EnumFlagsField(new Rect(), Emotion.None);
            element.Speaker = EditorGUI.TextField(new Rect(rect.x + GUIOffset, rect.y + GUIOffset, 100, 25), element.Speaker);
            element.Content = EditorGUI.TextArea(new Rect(125 + GUIOffset, rect.y + GUIOffset, 100, rect.height - GUIOffset), element.Content);
        };

        // Define the height of each element in the list
        list.elementHeightCallback = index =>
        {
            // Adjust the value based on your content height
            return EditorGUIUtility.singleLineHeight + 100f + GUIOffset;
        };
    }
    
    private void OnGUI()
    {
        // Display the reorderable list
        list.DoLayoutList();
    }
}