using System;
using UnityEditor;
using UnityEngine;

public class LabelInfoEditorWindow : EditorWindow
{
    public HierarchyLabelPreset label;
    
    private static void ShowWindow()
    {
        var window = GetWindow<LabelInfoEditorWindow>();
        window.titleContent = new GUIContent($"Label Info");
        window.Show();
    }

    public void Open(HierarchyLabelPreset _preset)
    {
        label = _preset;
        string text = label.name.Split('_')[1];
        titleContent.text = $"{text}";
        Show();
    }

    private void OnGUI()
    {
        label.info = GUILayout.TextArea(label.info, GUILayout.Height(maxSize.y), GUILayout.ExpandHeight(true));
    }
}
