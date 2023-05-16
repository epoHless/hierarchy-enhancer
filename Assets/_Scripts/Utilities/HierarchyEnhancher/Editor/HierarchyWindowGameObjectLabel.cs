using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[InitializeOnLoad]
public static class HierarchyWindowGameObjectLabel
{
    public static string LabelsDirectory { get; private set; }
    
    public static List<HierarchyLabelPreset> Presets = new List<HierarchyLabelPreset>();

    private static readonly Color SelectedColor = new Color(44f / 255f, 93f / 255f, 135f / 255f, 1f);
    private static readonly Color UnselectedColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);

    static HierarchyWindowGameObjectLabel()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        AssemblyReloadEvents.afterAssemblyReload += FetchLabels;
    }

    public static void FetchLabels()
    {
        if (!Directory.Exists($"{Application.dataPath}/HierarchyLabels"))
            AssetDatabase.CreateFolder("Assets", "HierarchyLabels");
        
        LabelsDirectory = "Assets/HierarchyLabels/";

        var assets = AssetDatabase.FindAssets("", new[] { LabelsDirectory });

        Presets = new List<HierarchyLabelPreset>();
        
        foreach (var asset in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var item = AssetDatabase.LoadAssetAtPath(path, typeof(HierarchyLabelPreset)) as HierarchyLabelPreset;

            if (item)
            {
                AddPreset(item);
            }
        }
    }

    static void HandleHierarchyWindowItemOnGUI(int _instanceID, Rect _selectionRect)
    {
        var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);
        var gameobject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
        
        if(gameobject) RenderLines(_selectionRect, gameobject, Color.gray);

        foreach (var preset in Presets)
        {
            if (content.text.StartsWith(preset.identifier))
            {
                string text = content.text.Remove(0,preset.identifier.Length);

                if(gameobject) RenderLines(_selectionRect, gameobject, preset.textColor);

                GUI.Label(_selectionRect, text, SetStylePreset(preset, _instanceID));

                if (content.image != null && preset.icon)
                {
                    GUI.DrawTexture(new Rect(_selectionRect.xMax - 20, _selectionRect.yMin, 20, 16), 
                        HierarchyUtilities.DrawCube(1,1,
                            EditorUtility.InstanceIDToObject(_instanceID) == Selection.activeObject ? SelectedColor : UnselectedColor));
                    
                    GUI.DrawTexture(new Rect(_selectionRect.xMax - 16, _selectionRect.yMin, 15, 15), preset.icon);
                }
            }
        }
    }

    private static void RenderLines(Rect _selectionRect, GameObject _gameObject, Color _color)
    {
        if(_gameObject.transform.childCount > 0)
        {
            GUI.DrawTexture(new Rect(_selectionRect.xMin - 24.5f, _selectionRect.yMin + 5, 7, 7),
                HierarchyUtilities.DrawCube(1, 1, _color));
        }

        if (_gameObject.transform.parent != null)
        {
            GUI.DrawTexture(new Rect(_selectionRect.xMin - 35, _selectionRect.yMin + 7.5f, 15, 1.5f),
                HierarchyUtilities.DrawCube(1, 1, _color));

            GUI.DrawTexture(new Rect(_selectionRect.xMin - 36.5f, _selectionRect.yMin, 2, 17),
                HierarchyUtilities.DrawCube(1, 1, _color));
        }
    }

    private static bool IsGameObjectEnabled(int _instanceID)
    {
        return EditorUtility.GetObjectEnabled(EditorUtility.InstanceIDToObject(_instanceID)) != 0;
    }

    private static GUIStyle SetStylePreset(HierarchyLabelPreset _preset, int _instanceID)
    {
        GUIStyle style = new GUIStyle();

        var normalStyleState = IsGameObjectEnabled(_instanceID) ? new GUIStyleState
        {
            background = EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject ? HierarchyUtilities.DrawCube(1, 1, _preset.backgroundColor) : HierarchyUtilities.DrawCube(1, 1, SelectedColor),
            textColor = _preset.textColor
        } : new GUIStyleState
        {
            background = EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject ? HierarchyUtilities.DrawCube(1, 1, _preset.inactiveBackgroundColor) : HierarchyUtilities.DrawCube(1, 1, SelectedColor),
            textColor = _preset.useCustomInactiveColors ? _preset.inactiveTextColor : HierarchyUtilities.ChangeColorBrightness(_preset.textColor, 0.45f)
        };

        style.normal = normalStyleState;
        
        style.fontStyle = _preset.fontStyle;
        style.alignment = _preset.alignment;
        
        return style;
    }

    public static void AddPreset(HierarchyLabelPreset _preset)
    {
        if (!Presets.Contains(_preset))
        {
            Presets.Add(_preset);
        }
        
        EditorApplication.RepaintHierarchyWindow();
    }
    
    public static void RemovePreset(HierarchyLabelPreset _preset)
    {
        if (Presets.Contains(_preset))
        {
            Presets.Remove(_preset);
        }
        
        EditorApplication.RepaintHierarchyWindow();
    }
}

#endif