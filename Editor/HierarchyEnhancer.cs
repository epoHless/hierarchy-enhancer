using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class HierarchyEnhancer
    {
        private static List<IRenderer> _renderers;

        static HierarchyEnhancer()
        {
            _renderers = new List<IRenderer>()
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

            for (int i = 0; i < _renderers.Count; i++)
            {
                if(_renderers[i].IsEnabled) _renderers[i].OnGUI(_instanceID, _selectionRect, gameObject);
            }
        }

        public static void ToggleRender<T>(bool _toggle)
        {
            var renderer = _renderers.Find(_renderer => _renderer.GetType() == typeof(T));

            if (renderer != null)
            {
                renderer.IsEnabled = _toggle;
            }
        }
    }
#endif
}