using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine;

public class ReorderableListExample : EditorWindow
{
    private ReorderableList _reorderableMessages;
    private const int GUIOffset = 5;
    private Vector2 _scrollPosition = Vector2.zero;

    [MenuItem("Window/Reorderable List Example")]
    private static void OpenWindow()
    {
        var window = GetWindow<ReorderableListExample>();
        window.titleContent = new GUIContent("Reorderable List Example");
        window.Show();
    }

    private void OnEnable()
    {
        SetWindowsMinimumSize();
        DrawUIElements();
    }

    private void SetWindowsMinimumSize() => minSize = new Vector2(300, 250);

    private void DrawUIElements()
    {
        _reorderableMessages = new ReorderableList(new List<Message>(), typeof(Message),true, true, true, true);

        _reorderableMessages.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = _reorderableMessages.list[index] as Message;
            
            EditorGUI.EnumFlagsField(new Rect(), Emotion.None);

            if (element == null) return;
            
            element.Speaker =
                EditorGUI.TextField(new Rect(rect.x + GUIOffset, rect.y + GUIOffset, position.width - 50, 20),
                    new GUIContent("Speaker"), element.Speaker);
            element.EmotionDisplayed = (Emotion)
                EditorGUI.EnumPopup(new Rect(rect.x + GUIOffset, rect.y + GUIOffset + 30, position.width - 50, 20),
                    new GUIContent("Emotion"), element.EmotionDisplayed);
            element.Content =
                EditorGUI.TextArea(
                    new Rect(rect.x + GUIOffset, rect.y + GUIOffset + 60, position.width - 50, 100 - GUIOffset),
                    element.Content);
        };
        
        _reorderableMessages.elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 150f + GUIOffset;
    }
    
    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
        _reorderableMessages.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }
}