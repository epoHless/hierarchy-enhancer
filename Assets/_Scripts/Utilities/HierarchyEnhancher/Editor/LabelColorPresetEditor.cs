using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HierarchyLabelPreset))]
public class LabelColorPresetEditor : Editor
{
    public HierarchyLabelPreset script;
    private GUIStyle labelStyle;

    public bool showBase = false;

    private void Awake()
    {
        script = (HierarchyLabelPreset)target;

        labelStyle = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            normal = new GUIStyleState()
            {
                textColor = Color.white
            }
        };
        
        EditorUtility.SetDirty(script);
    }

    private void OnDisable()
    {
        AssetDatabase.SaveAssetIfDirty(script);
    }

    public override void OnInspectorGUI()
    {
        if(showBase) base.OnInspectorGUI();
        else
        {
            EditorGUILayout.BeginHorizontal();

            ShowIdentifierIcon();

            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
        
            ShowFontStyleAlignment();

            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 40;
        
            ShowTextColorBGColor();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            ShowCustomInactiveColors();

            EditorGUILayout.Space(20);
        
            GUILayout.FlexibleSpace();
            ShowPresetButtons();
        
            EditorApplication.RepaintHierarchyWindow();
        }
    }

    public void ShowPresetButtons()
    {
        if (!LabelManager.Presets.Contains(script))
        {
            if (GUILayout.Button("Add To Presets"))
            {
                LabelManager.AddPreset(script);
            }
        }
        else
        {
            if (GUILayout.Button("Remove From Presets"))
            {
                LabelManager.RemovePreset(script);
            }
        }
    }

    public void ShowCustomInactiveColors()
    {
        EditorGUIUtility.labelWidth = 160;
        script.useCustomInactiveColors = EditorGUILayout.Toggle("Use Custom Inactive Colors", script.useCustomInactiveColors);

        if (script.useCustomInactiveColors)
        {
            // EditorGUI.indentLevel++;
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Text Color", labelStyle);
            script.inactiveTextColor = EditorGUILayout.ColorField(script.inactiveTextColor);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Background Color", labelStyle);
            script.inactiveBackgroundColor = EditorGUILayout.ColorField(script.inactiveBackgroundColor);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            // EditorGUI.indentLevel--;
        }
    }

    public void ShowTextColorBGColor()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Text Color", labelStyle);
        script.textColor = EditorGUILayout.ColorField(script.textColor);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Background Color", labelStyle);
        script.backgroundColor = EditorGUILayout.ColorField(script.backgroundColor);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public void ShowFontStyleAlignment()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Font Style", labelStyle);
        script.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(script.fontStyle);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Alignment", labelStyle);
        script.alignment = (TextAnchor)EditorGUILayout.EnumPopup(script.alignment);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public void ShowIdentifierIcon()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Icon", labelStyle);
        script.icon = EditorGUILayout.ObjectField(script.icon, typeof(Texture), true) as Texture;
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }
}