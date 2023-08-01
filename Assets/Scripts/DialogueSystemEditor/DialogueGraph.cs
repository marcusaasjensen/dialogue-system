using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";
    
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void ConstructGraph()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
        
        var miniMap = new MiniMap();

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
        
        var nodeCreateButton = new Button(() =>
        {
            _graphView.CreateNode("Multiple Choice Node");
        });

        var transitionCreateButton = new Button(() =>
        {
            _graphView.CreateTransitionNode("Transition Node");
        });
        
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        transitionCreateButton.text = "Create Transition";
        toolbar.Add(transitionCreateButton);
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
        if(save)
            saveUtility.SaveGraph(_fileName);
        else
            saveUtility.LoadGraph(_fileName);
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
