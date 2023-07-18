using HierarchyEnhancer.Editor;
using UnityEngine;

public class ToggleRenderer : IRenderer
{
    public bool IsEnabled { get; set; } = true;
    public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
    {
        if (!LabelManager.ShowToggleButton) return;

        var rect = new Rect(_selectionRect.xMax - 16, _selectionRect.yMin - 1, 15, 15);
        _gameObject.SetActive(GUI.Toggle(rect, _gameObject.activeSelf, GUIContent.none));
    }
}
