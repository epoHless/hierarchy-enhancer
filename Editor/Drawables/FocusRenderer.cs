using HierarchyEnhancer.Editor;
using UnityEditor;
using UnityEngine;

public class FocusRenderer : IRenderer
{
    public bool IsEnabled { get; set; }
    public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
    {
        if (!LabelManager.ShowFocusButton) return;

        if (GUI.Button(new Rect(_selectionRect.xMin, _selectionRect.yMin, 15, 15),
                new GUIContent() { tooltip = "Click to focus" }, GUIStyle.none))
        {
            Selection.activeObject = _gameObject;
            SceneView.FrameLastActiveSceneView();
        }
    }
}
