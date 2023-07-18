using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class HierarchyEnhancer
    {
        public static IRenderer[] _renderers { get; private set; }

        static HierarchyEnhancer()
        {
            _renderers = new IRenderer[]
            {
                new LabelRenderer(),
                new ParentRenderer(Color.gray),
                new ComponentRenderer(),
                new FocusRenderer(),
                new ToggleRenderer(),
                new IconRenderer()
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
                if(_renderers[i].IsEnabled) _renderers[i].OnGUI(_instanceID, _selectionRect, gameObject);
            }
        }
    }
#endif
}