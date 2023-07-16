using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class HierarchyEnhancer
    {
        private static IRenderer[] _renderers;

        static HierarchyEnhancer()
        {
            var labels = LabelManager.FetchLabels();
            
            _renderers = new IRenderer[]
            {
                new ParentRenderer(Color.gray),
                new ComponentRenderer(),
                new FocusRenderer(),
                new ToggleRenderer(),
                new LabelRenderer(labels),
                new IconRenderer(labels)
            };
            
            EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchy;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchy;
        }

        static void DrawHierarchy(int _instanceID, Rect _selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
            if (!gameObject) return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].OnGUI(_instanceID, _selectionRect, gameObject);
            }
        }
    }
#endif
}