using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR

namespace DialogueSystem.Editor
{
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView _graphView;
        private string _fileName = "New Narrative";
    
        [MenuItem("Graph/Dialogue Graph Editor")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Dialogue Graph Editor");
        }

        private void ConstructGraph()
        {
            _graphView = new DialogueGraphView
            {
                name = "Dialogue Graph Editor"
            };
        
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        
            toolbar.Add(fileNameTextField);
            toolbar.Add(new Button(() => RequestDataOperation(true)){text="Save Data"});
            toolbar.Add(new Button(() => RequestDataOperation(false)){text="Load Data"});
        
            var transitionCreateButton = new Button(() =>
            {
                _graphView.CreateSimpleDialogueNode("Simple Node");
            });
            
            var nodeCreateButton = new Button(() =>
            {
                _graphView.CreateNode("Multiple Choice Node");
            });

        
            transitionCreateButton.text = "Create Simple Node";
            toolbar.Add(transitionCreateButton);
            nodeCreateButton.text = "Create Multiple Choice Node";
            toolbar.Add(nodeCreateButton);
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
                return;
            }

            var saveUtility = GraphSaveUtility.GetInstance(_graphView);

            if (save)
            {
                saveUtility.SaveGraph(_fileName);
            }
            else
            {
                saveUtility.LoadGraph(_fileName);
            }
        }

        private void OnEnable()
        {
            ConstructGraph(); 
            GenerateToolbar();
            GenerateMiniMap();
        }
        
        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(miniMap);
        }
    
        private void OnDisable() => rootVisualElement.Remove(_graphView);
    }
}

#endif