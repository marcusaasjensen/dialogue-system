using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#if UNITY_EDITOR

namespace DialogueSystem.Editor
{
    public class DialogueView : EditorWindow
    {
        private ReorderableList _reorderableMessages;
        private const int GUIOffset = 5;
        private Vector2 _scrollPosition = Vector2.zero;
        private static List<DialogueMessage> _dialogue;
        private static EditorWindow _windowProperties;
    
        public static void OpenWindow(List<DialogueMessage> messages)
        {
            _dialogue = messages;
            var window = GetWindow<DialogueView>();

            if (_windowProperties)
            {
                window.position = _windowProperties.position;
            }

            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        public static void CloseWindow()
        {
            var window = GetWindow<DialogueView>();
            _windowProperties = window;
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
            _reorderableMessages = new ReorderableList(_dialogue, typeof(DialogueMessage),true, true, true, true);

            _reorderableMessages.drawElementCallback = (rect, index, _, _) =>
            {
                var element = _reorderableMessages.list[index] as DialogueMessage;
            
                EditorGUI.EnumFlagsField(new Rect(), Emotion.Default);

                if (element == null)
                {
                    return;
                }
            
                element.CharacterName =
                    EditorGUI.TextField(new Rect(rect.x + GUIOffset, rect.y + GUIOffset, position.width - 50, 20),
                        new GUIContent("Character name"), element.CharacterName);
                element.HideCharacter = EditorGUI.Toggle(new Rect(rect.x + GUIOffset, rect.y + GUIOffset + 30, position.width - 50, 20), new GUIContent("Hide character"), element.HideCharacter);
                element.Content =
                    EditorGUI.TextArea(
                        new Rect(rect.x + GUIOffset, rect.y + GUIOffset + 60, position.width - 50, 125),
                        element.Content);
            };
        
            _reorderableMessages.elementHeightCallback = _ => EditorGUIUtility.singleLineHeight + 170f + GUIOffset;
        }
    
        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            _reorderableMessages.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }
    }
}

#endif