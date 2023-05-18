﻿using System;
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
        
        if(gameObject != null)
        {
            RenderLines(_selectionRect, gameObject, Color.gray);
            RenderGameObjectToggle(_selectionRect, gameObject);

            RenderFocusButton(_selectionRect, gameObject);
        }

        foreach (var preset in LabelManager.Presets)
        {
            if (/*content.text.StartsWith(preset.identifier)*/ gameObject != null && preset.gameObjects.Contains(gameObject))
            {
                string text = content.text/*.Remove(0,preset.identifier.Length)*/;

                if(gameObject)
                {
                    RenderLines(_selectionRect, gameObject, preset.textColor);
                }

                RenderGUI(_instanceID, _selectionRect, text, preset, content, gameObject);
            }
        }
    }

    #region Render Methods

    private static void RenderFocusButton(Rect _selectionRect, GameObject _gameObject)
    {
        if (!LabelManager.ShowFocusButton) return;
        
        if (GUI.Button(new Rect(_selectionRect.xMin, _selectionRect.yMin, 15, 15), new GUIContent(){ tooltip = "Click to focus"}, GUIStyle.none))
        {
            Selection.activeObject = _gameObject;
            SceneView.FrameLastActiveSceneView();
        }
    }

    private static void RenderGameObjectToggle(Rect _selectionRect, GameObject gameObject)
    {
        if (!LabelManager.ShowToggleButton) return;

        gameObject.SetActive(GUI.Toggle(new Rect(_selectionRect.xMax - 16, _selectionRect.yMin, 15, 15),
            gameObject.activeSelf, GUIContent.none));
    }

    private static void RenderGUI(int _instanceID, Rect _selectionRect, string _text, HierarchyLabelPreset _preset,
        GUIContent _content, GameObject _gameObject)
    {
        GUI.Label(new Rect(_selectionRect.xMin + 16, _selectionRect.yMin, _selectionRect.width, _selectionRect.height), _text, SetStylePreset(_preset, _instanceID));
        
        RenderGameObjectToggle(_selectionRect, _gameObject);
        RenderFocusButton(_selectionRect, _gameObject);

        if(_preset.icon)
        {
            GUI.Label(new Rect(_selectionRect.xMin, _selectionRect.yMin + 1, 15, 15), GUIContent.none, SetStylePreset(_preset, _instanceID));
            GUI.DrawTexture(new Rect(_selectionRect.xMin, _selectionRect.yMin + 1, 15, 15), _preset.icon);
        }

        int offset = LabelManager.ShowToggleButton ? 32 : 16;
            
        for (int i = 0; i < _preset.tooltips.Count; i++)
        {
            if(!_preset.tooltips[i].icon) continue;
                
            if (GUI.Button(new Rect(_selectionRect.xMax - offset, _selectionRect.yMin, 15, 15), //opens the info panel
                    new GUIContent(_preset.tooltips[i].icon,_preset.tooltips[i].tooltip), GUIStyle.none))
            {
                var infoWindow = ScriptableObject.CreateInstance<LabelInfoEditorWindow>();
                infoWindow.Open(_preset, i);
            }

            var iconRect = new Rect(_selectionRect.xMax - offset, _selectionRect.yMin, 15, 15);
            GUI.DrawTexture(iconRect, _preset.tooltips[i].icon);

            offset += 16;
        }
    }

    private static void RenderLines(Rect _selectionRect, GameObject _gameObject, Color _color)
    {
        if (!LabelManager.ShowHierarchyLines) return;
        
        if(_gameObject.transform.childCount > 0)
        {
            DrawLine(_selectionRect, _color, 7,7, - 24.5f, 5);
        }

        int parentCount = GetParentCount(_gameObject);

        if (_gameObject.transform.parent != null)
        {
            DrawLine(_selectionRect, _color, 15, 1.5f, -35, 7.45f);
            
            for (int i = 0; i < parentCount; i++) //adds additional lines for nested objects
            {
                if(_gameObject.transform.parent && 
                   _gameObject.transform.childCount == 0 && 
                   _gameObject.transform.parent.GetChild(_gameObject.transform.parent.childCount - 1).gameObject == _gameObject &&
                   i <= _gameObject.transform.parent.childCount)         
                    DrawLine(_selectionRect, _color, 2, 8.5f, -36.5f - (14f * i));
                else
                    DrawLine(_selectionRect, _color, 2, 17, -36.5f - (14f * i));
            }
        }
    }

    private static void DrawLine(Rect _selectionRect, Color _color, float _width, float _height, float _xOffset, float _yOffset = 0)
    {
        GUI.DrawTexture(new Rect(_selectionRect.xMin + _xOffset, _selectionRect.yMin + _yOffset, _width, _height),
            HierarchyUtilities.DrawCube(1, 1, _color));
    }

    #endregion
    
    private static int GetParentCount(GameObject _gameObject)
    {
        int count = 0;
        Transform current = _gameObject.transform;

        while (current.parent != null)
        {
            count++;
            current = current.parent;
        }

        return count;
    }

    private static GUIStyle SetStylePreset(HierarchyLabelPreset _preset, int _instanceID)
    {
        Color color = EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject
            ? LabelManager.UnselectedColor
            : LabelManager.SelectedColor;
        
        GUIStyle style = new GUIStyle
        {
            normal = new GUIStyleState()
            {
                background = HierarchyUtilities.DrawCube(1, 1,color),
                textColor = _preset.textColor
            },
            hover = new GUIStyleState()
            {
                background = HierarchyUtilities.DrawCube(1, 1, LabelManager.HoveredColor),
                textColor = _preset.textColor
            },
            
            fontStyle = _preset.fontStyle,
            alignment = _preset.alignment
        };

        return style;
    }
}

#endif