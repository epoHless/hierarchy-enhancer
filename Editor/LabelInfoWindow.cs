﻿using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    public class LabelInfoWindow : EditorWindow
    {
        public Label label;
        public int index;

        private static void ShowWindow()
        {
            var window = GetWindow<LabelInfoWindow>();
            window.titleContent = new GUIContent($"Label Info");
            window.Show();
        }

        public void Open(Label _preset, int _index)
        {
            label = _preset;
            index = _index;

            string text = label.name.Split('_')[1];
            titleContent.text = $"{text} | {label.tooltips[index].tooltip} info";
            Show();
        }

        private void OnGUI()
        {
            label.tooltips[index].info = GUILayout.TextArea(label.tooltips[index].info, GUILayout.Height(maxSize.y),
                GUILayout.ExpandHeight(true));
        }
    }
#endif
}