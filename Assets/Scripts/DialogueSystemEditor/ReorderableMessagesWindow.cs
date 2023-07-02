using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;
using UnityEngine;

public class ReorderableMessagesWindow : EditorWindow
{
    private ReorderableList _reorderableMessages;
    private const int GUIOffset = 5;
    private Vector2 _scrollPosition = Vector2.zero;
    private static List<Message> _dialogue;
    private static EditorWindow windowProperties;
    
    public static void OpenWindow(List<Message> messages)
    {
        _dialogue = messages;
        var window = GetWindow<ReorderableMessagesWindow>();

        if (windowProperties)
        {
            window.position = windowProperties.position;
        }

        window.titleContent = new GUIContent("Reorderable List Example");
        window.Show();
    }

    public static void CloseWindow()
    {
        var window = GetWindow<ReorderableMessagesWindow>();
        windowProperties = window;
        window.Close();
    }

    private void OnEnable()
    {
        SetWindowsMinimumSize();
        DrawUIElements();
    }

    private void SetWindowsMinimumSize() => minSize = new Vector2(300, 250);

    private void DrawUIElements()
    {
        _reorderableMessages = new ReorderableList(_dialogue, typeof(Message),true, true, true, true);

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