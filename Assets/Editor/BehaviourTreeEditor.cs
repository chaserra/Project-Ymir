using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
using QuaternionGames.BT;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;

    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    /** Uncomment below if on Unity v.2021 **/
    //private void OnEnable()
    //{
    //    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    //    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    //}

    //private void OnDisable()
    //{
    //    EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    //}

    //private void OnPlayModeStateChanged(PlayModeStateChange obj)
    //{
    //    switch (obj)
    //    {
    //        case PlayModeStateChange.EnteredEditMode:
    //            OnSelectionChange();
    //            break;
    //        case PlayModeStateChange.ExitingEditMode:
    //            break;
    //        case PlayModeStateChange.EnteredPlayMode:
    //            OnSelectionChange();
    //            break;
    //        case PlayModeStateChange.ExitingPlayMode:
    //            break;
    //    }
    //}

    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (!tree)
        {
            if (Selection.activeObject)
            {
                var o = Selection.activeObject as GameObject;
                BT_Tester tester = o.GetComponent<BT_Tester>();
                if (tester)
                {
                    tree = tester.Tree;
                }
            }
        }

        // Expected to return a bug. CanOpenAssetInEditor is not available on Unity 2020.3f
        // Just delete Element 0 to proceed
        if (tree)
        {
            treeView.PopulateView(tree);
        }

        /** Below code is for Unity version 2021 up **/
        //if (Application.isPlaying)
        //{
        //    if (tree)
        //    {
        //        treeView.PopulateView(tree);
        //    }
        //}
        //else
        //{
        //    if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        //    {
        //        treeView.PopulateView(tree);
        //    }
        //}
    }

    void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

}