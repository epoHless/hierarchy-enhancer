using System.Collections.Generic;
using HierarchyEnhancer.Editor;
using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

public class LabelRenderer : IRenderer
{
    private List<Label> labels;

    public bool IsEnabled { get; set; }

    public LabelRenderer(List<Label> _labels)
    {
        labels = _labels;
    }
    
    public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
    {
        var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);
        
        foreach (var preset in labels)
        {
            if (!preset.isActive) continue;

            bool value = preset.gameObjects is { Count: > 0 } &&
                         preset.gameObjects.Find(_dictionary => _dictionary.GameObject == _gameObject) != null;

            if (_gameObject != null && value)
            {
                GUI.DrawTexture(_selectionRect, Utilities.DrawCube(LabelManager.UnselectedColor));

                var guiContent = new GUIContent() { text = content.text };

                GUI.Label(
                    new Rect(_selectionRect.xMin + 18, _selectionRect.yMin - 1, _selectionRect.width,
                        _selectionRect.height),
                    guiContent, SetStyle(preset, _instanceID, _selectionRect));

                if (preset.icon)
                {
                    GUI.DrawTexture(new Rect(_selectionRect.xMin, _selectionRect.yMin, 15f, 15f), preset.icon);
                }
            }
        }
    }
    
    private GUIStyle SetStyle(Label _preset, int _instanceID, Rect _rect)
    {
        Color backgroundColor = EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject
            ? _preset.backgroundColor
            : LabelManager.SelectedColor;

        backgroundColor = IsGameObjectEnabled(_instanceID)
            ? backgroundColor
            : Utilities.ChangeColorBrightness(backgroundColor, .5f);

        Color textColor = IsGameObjectEnabled(_instanceID)
            ? _preset.textColor
            : Utilities.ChangeColorBrightness(_preset.textColor, .5f);

        var fadedColor = backgroundColor;
        fadedColor.a = 0;

        GUIStyle style = new GUIStyle
        {
            normal = new GUIStyleState()
            {
                background = Utilities.CreateGradientTexture((int)_rect.width, (int)_rect.height, fadedColor,
                    backgroundColor),
                textColor = textColor
            },
            hover = new GUIStyleState()
            {
                background = Utilities.DrawCube(LabelManager.HoveredColor),
                textColor = textColor
            },

            fontStyle = _preset.fontStyle,
            alignment = _preset.alignment
        };

        return style;
    }
    
    private bool IsGameObjectEnabled(int _instanceID)
    {
        return EditorUtility.GetObjectEnabled(EditorUtility.InstanceIDToObject(_instanceID)) == 1;
    }
}