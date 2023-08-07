using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    public class LabelInfoWindow : EditorWindow
    {
        public Tooltip tooltip;
        public int index;

        private static void ShowWindow()
        {
            var window = GetWindow<LabelInfoWindow>();
            window.titleContent = new GUIContent($"Label Info");
            window.Show();
        }

        public void Open(Tooltip _tooltip, int _index)
        {
            tooltip = _tooltip;
            index = _index;
            Show();
        }

        private void OnGUI()
        {
            tooltip.info = GUILayout.TextArea(tooltip.info, GUILayout.Height(maxSize.y),
                GUILayout.ExpandHeight(true));
        }
    }
#endif
}