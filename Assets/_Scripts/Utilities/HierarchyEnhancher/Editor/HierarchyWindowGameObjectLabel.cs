using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[InitializeOnLoad]
public static class HierarchyWindowGameObjectLabel
{
    static HierarchyWindowGameObjectLabel()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    static void HandleHierarchyWindowItemOnGUI(int _instanceID, Rect _selectionRect)
    {
        var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);
        var gameObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;
        
        if(gameObject) RenderLines(_selectionRect, gameObject, Color.gray);

        foreach (var preset in LabelManager.Presets)
        {
            if (content.text.StartsWith(preset.identifier))
            {
                string text = content.text.Remove(0,preset.identifier.Length);

                if(gameObject) RenderLines(_selectionRect, gameObject, preset.textColor);
                RenderGUI(_instanceID, _selectionRect, text, preset, content);
            }
        }
    }

    private static void RenderGUI(int _instanceID, Rect _selectionRect, string _text, HierarchyLabelPreset _preset,
        GUIContent _content)
    {
        GUI.Label(_selectionRect, _text, SetStylePreset(_preset, _instanceID));

        if (_content.image != null && _preset.icon)
        {
            GUI.DrawTexture(new Rect(_selectionRect.xMax - 20, _selectionRect.yMin, 20, 16),
                HierarchyUtilities.DrawCube(1, 1,
                    EditorUtility.InstanceIDToObject(_instanceID) == Selection.activeObject
                        ? LabelManager.SelectedColor
                        : LabelManager.UnselectedColor));

            GUI.DrawTexture(new Rect(_selectionRect.xMax - 16, _selectionRect.yMin, 15, 15), _preset.icon);
        }
    }

    private static void RenderLines(Rect _selectionRect, GameObject _gameObject, Color _color)
    {
        if(_gameObject.transform.childCount > 0)
        {
            DrawLine(_selectionRect, _color, 7,7, - 24.5f, 5);
        }

        if (_gameObject.transform.parent != null)
        {
            DrawLine(_selectionRect, _color, 15, 1.5f, -35, 7.5f);
            DrawLine(_selectionRect, _color, 2, 17, -36.5f);
        }
    }

    private static void DrawLine(Rect _selectionRect, Color _color, float _width, float _height, float _xOffset, float _yOffset = 0)
    {
        GUI.DrawTexture(new Rect(_selectionRect.xMin + _xOffset, _selectionRect.yMin + _yOffset, _width, _height),
            HierarchyUtilities.DrawCube(1, 1, _color));
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
            background = EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject ? HierarchyUtilities.DrawCube(1, 1, _preset.backgroundColor) : HierarchyUtilities.DrawCube(1, 1, LabelManager.SelectedColor),
            textColor = _preset.textColor
        } : new GUIStyleState
        {
            background = EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject ? HierarchyUtilities.DrawCube(1, 1, _preset.inactiveBackgroundColor) : HierarchyUtilities.DrawCube(1, 1, LabelManager.SelectedColor),
            textColor = _preset.useCustomInactiveColors ? _preset.inactiveTextColor : HierarchyUtilities.ChangeColorBrightness(_preset.textColor, 0.45f)
        };

        style.normal = normalStyleState;
        
        style.fontStyle = _preset.fontStyle;
        style.alignment = _preset.alignment;
        
        return style;
    }
}

#endif