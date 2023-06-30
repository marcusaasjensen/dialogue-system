using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine;

public class ReorderableListExample : EditorWindow
{
    private ReorderableList reorderableList;
    private const int GUIOffset = 5;
    public Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Reorderable List Example")]
    private static void OpenWindow()
    {
        var window = GetWindow<ReorderableListExample>();
        window.titleContent = new GUIContent("Reorderable List Example");
        window.Show();
    }

    private void OnEnable()
    {
        var m1 = new Message
        {
            Content = "blablabla",
            EmotionDisplayed = Emotion.None,
            Speaker = "Toto"
        };
        
        reorderableList = new ReorderableList(new List<Message>{m1}, typeof(Message),true, true, true, true);
        // Define how each element in the list should be displayed
        
        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = reorderableList.list[index] as Message;
            
            EditorGUI.EnumFlagsField(new Rect(), Emotion.None);
            element.Speaker = EditorGUI.TextField(new Rect(rect.x + GUIOffset, rect.y + GUIOffset, position.width - 50, 20),new GUIContent("Speaker"),  element.Speaker);
            element.EmotionDisplayed = (Emotion)
                EditorGUI.EnumPopup(new Rect(rect.x + GUIOffset, rect.y + GUIOffset + 30, position.width - 50, 20),new GUIContent("Emotion"),  element.EmotionDisplayed);
            element.Content = EditorGUI.TextArea(new Rect(rect.x + GUIOffset, rect.y + GUIOffset + 60, position.width - 50, 100 - GUIOffset), element.Content);
        };
        
        reorderableList.elementHeightCallback = index =>
        {
            return EditorGUIUtility.singleLineHeight + 150f + GUIOffset;
        };
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }
}