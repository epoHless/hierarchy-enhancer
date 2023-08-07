using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
    public class LabelRenderer : IRenderer
    {
        public bool IsEnabled { get; set; } = true;

        public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
        {
            var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);

            foreach (var preset in LabelManager.Labels)
            {
                if (!preset.isActive) continue;

                if (_gameObject != null && HasObjects(_gameObject, preset))
                {
                    GUI.DrawTexture(_selectionRect, Utilities.CreateColoredTexture(LabelManager.UnselectedColor));

                    var guiContent = new GUIContent() { text = content.text };

                    GUI.Label(_selectionRect, guiContent, SetStyle(preset, _instanceID, _selectionRect));

                    var iconRect = new Rect(_selectionRect.xMin, _selectionRect.yMin, 16f, 16f);

                    if(preset.useIcon)
                        GUI.DrawTexture(iconRect, preset.icon ? preset.icon : SetIcon(_gameObject));
                }
            }
        }

        private Texture SetIcon(GameObject _gameObject)
        {
            return IsPrefab(_gameObject)
                ? EditorGUIUtility.IconContent("d_Prefab Icon").image
                : EditorGUIUtility.IconContent("d_GameObject Icon").image;
        }

        private bool HasObjects(GameObject _gameObject, Label preset)
        {
            return preset.gameObjects is { Count: > 0 } &&
                   preset.gameObjects.Find(_dictionary => _dictionary.GameObject == _gameObject) != null;
        }

        private GameObject IsPrefab(GameObject _gameObject)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(_gameObject);
        }

        private GUIStyle SetStyle(Label _preset, int _instanceID, Rect _rect)
        {
            var guiColor = LabelManager.GetGUIColor();

            var color = _preset.useCustomBackground ? _preset.backgroundColor : guiColor;

            Color backgroundColor = GetBackgroundColor(_instanceID, color);
            backgroundColor = SetBrightness(_instanceID, backgroundColor);
            Color textColor = GetTextColor(_preset, _instanceID);

            var fadedColor = backgroundColor;
            fadedColor.a = 0;

            var backgroundTexture = SetBackgroundType(_preset, _rect, fadedColor, backgroundColor);

            GUIStyle style = new GUIStyle
            {
                normal = new GUIStyleState()
                {
                    background = backgroundTexture,
                    textColor = textColor
                },
                hover = new GUIStyleState()
                {
                    background = Utilities.CreateColoredTexture(LabelManager.HoveredColor),
                    textColor = textColor
                },

                contentOffset = new Vector2(18, -1),
                fontStyle = _preset.fontStyle,
                alignment = _preset.alignment,
                font = _preset.font,
                fontSize = _preset.fontSize
            };

            return style;
        }

        private bool IsGameObjectEnabled(int _instanceID)
        {
            return EditorUtility.GetObjectEnabled(EditorUtility.InstanceIDToObject(_instanceID)) == 1;
        }

        private Texture2D SetBackgroundType(Label _preset, Rect _rect, Color _fadedColor, Color _backgroundColor)
        {
            return _preset.useGradient
                ? Utilities.CreateGradientTexture((int)_rect.width, (int)_rect.height, _fadedColor, _backgroundColor)
                : Utilities.CreateColoredTexture(_backgroundColor);
        }

        private Color GetTextColor(Label _preset, int _instanceID)
        {
            return IsGameObjectEnabled(_instanceID)
                ? _preset.textColor
                : Utilities.ChangeColorBrightness(_preset.textColor, .5f);
        }

        private Color SetBrightness(int _instanceID, Color _backgroundColor)
        {
            return IsGameObjectEnabled(_instanceID)
                ? _backgroundColor
                : Utilities.ChangeColorBrightness(_backgroundColor, .5f);
        }

        private Color GetBackgroundColor(int _instanceID, Color _color)
        {
            return EditorUtility.InstanceIDToObject(_instanceID) != Selection.activeObject
                ? _color
                : LabelManager.SelectedColor;
        }
        
        
    }
}