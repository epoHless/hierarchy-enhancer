using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    public class ToggleRenderer : IRenderer
    {
        public bool IsEnabled { get; set; } = true;

        public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
        {
            if (!LabelManager.ShowToggleButton) return;

            var rect = new Rect(_selectionRect.xMax - 16, _selectionRect.yMin, 16, 16);
            _gameObject.SetActive(GUI.Toggle(rect, _gameObject.activeSelf, GUIContent.none));
        }
    }
#endif
}
