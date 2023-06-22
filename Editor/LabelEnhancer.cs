using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer
{
    [InitializeOnLoad]
    internal static class LabelEnhancer
    {
        private static bool drawLines = true;

        static LabelEnhancer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchy;
            EditorApplication.hierarchyChanged += DrawHierarchyLines;

            LabelManager.OnRequestLineDraw += OnRequestLineDraw;
        }

        static void DrawHierarchy(int _instanceID, Rect _selectionRect)
        {
            var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);
            var gameObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;

            if (gameObject != null)
            {
                RenderLines(_selectionRect, gameObject, Color.gray);
                RenderGameObjectToggle(_selectionRect, gameObject);
                RenderFocusButton(_selectionRect, gameObject);
            }

            foreach (var preset in LabelManager.Labels)
            {
                if (!preset.isActive) continue;

                bool value = preset.gameObjects is { Count: > 0 } &&
                             preset.gameObjects.Find(_dictionary => _dictionary.GameObject == gameObject) != null;

                if (gameObject != null && value)
                {
                    string text = content.text;

                    if (gameObject)
                    {
                        RenderLines(_selectionRect, gameObject, preset.backgroundColor);
                    }

                    RenderGUI(_instanceID, _selectionRect, text, preset, gameObject);
                }
            }
        }

        private static void DrawHierarchyLines()
        {
            drawLines = true;
        }

        private static void OnRequestLineDraw(bool _value)
        {
            drawLines = _value;
        }

        #region Render Methods

        private static void RenderFocusButton(Rect _selectionRect, GameObject _gameObject)
        {
            if (!LabelManager.ShowFocusButton) return;

            if (GUI.Button(new Rect(_selectionRect.xMin, _selectionRect.yMin, 15, 15),
                    new GUIContent() { tooltip = "Click to focus" }, GUIStyle.none))
            {
                Selection.activeObject = _gameObject;
                SceneView.FrameLastActiveSceneView();
            }
        }

        private static void RenderGameObjectToggle(Rect _selectionRect, GameObject _gameObject)
        {
            if (!LabelManager.ShowToggleButton) return;

            _gameObject.SetActive(GUI.Toggle(new Rect(_selectionRect.xMax - 16, _selectionRect.yMin - 1, 15, 15),
                _gameObject.activeSelf, GUIContent.none));
        }

        private static void RenderGUI(int _instanceID, Rect _selectionRect, string _text, Label _preset,
            GameObject _gameObject)
        {
            GUI.DrawTexture(_selectionRect, Utilities.DrawCube(1, 1, LabelManager.UnselectedColor));

            var guiContent = new GUIContent() { text = _text };

            GUI.Label(
                new Rect(_selectionRect.xMin + 18, _selectionRect.yMin - 1, _selectionRect.width,
                    _selectionRect.height),
                guiContent, SetStylePreset(_preset, _instanceID, _selectionRect));

            RenderGameObjectToggle(_selectionRect, _gameObject);
            RenderFocusButton(_selectionRect, _gameObject);

            var textwidht = GUI.skin.label.CalcSize(guiContent);
            var components = _gameObject.GetComponents(typeof(Component));
            int compOffset = 16;

            if (components.Length > 1)
            {
                for (int i = 1; i < components.Length; i++)
                {
                    var content = EditorGUIUtility.ObjectContent(_gameObject.GetComponents(typeof(Component))[i],
                        typeof(Component));

                    var text = content.text;
                    text = text.Substring(text.IndexOf('(') + 1).Trim(')');

                    if (GUI.Button(
                            new Rect(_selectionRect.xMin + 2 + textwidht.x + compOffset, _selectionRect.yMin, 15f, 15f),
                            new GUIContent() { tooltip = text }))
                    {
                        Selection.activeGameObject = _gameObject;
                        OpenAdditionalLockedInpsector.DisplayLockedInspector();
                    }

                    GUI.DrawTexture(
                        new Rect(_selectionRect.xMin + 2 + textwidht.x + compOffset, _selectionRect.yMin, 15f, 15f),
                        content.image);
                    compOffset += 16;
                }
            }

            if (_preset.icon)
            {
                GUI.DrawTexture(new Rect(_selectionRect.xMin, _selectionRect.yMin, 15f, 15f), _preset.icon);
            }

            int offset = LabelManager.ShowToggleButton ? 32 : 16;

            for (int i = 0; i < _preset.tooltips.Count; i++)
            {
                if (!_preset.tooltips[i].icon) continue;

                if (GUI.Button(
                        new Rect(_selectionRect.xMax - offset, _selectionRect.yMin - 1, 15, 15), //opens the info panel
                        new GUIContent(_preset.tooltips[i].icon, _preset.tooltips[i].tooltip), GUIStyle.none))
                {
                    var infoWindow = ScriptableObject.CreateInstance<LabelInfoWindow>();
                    infoWindow.Open(_preset, i);
                }

                var iconRect = new Rect(_selectionRect.xMax - offset, _selectionRect.yMin, 15, 15);
                GUI.DrawTexture(iconRect, _preset.tooltips[i].icon);

                offset += 16;
            }
        }

        private static void RenderLines(Rect _selectionRect, GameObject _gameObject, Color _color)
        {
            if (!LabelManager.ShowHierarchyLines && !drawLines) return;

            if (_gameObject.transform.childCount > 0)
            {
                DrawLine(_selectionRect, _color, 6, 6, -24.5f, 5);
            }

            var transforms = GetParentCount(_gameObject);

            if (_gameObject.transform.parent != null)
            {
                if (_gameObject.transform.childCount == 0)
                    DrawLine(_selectionRect, _color, 30f, 1f, -36f, 7.45f);
                else
                    DrawLine(_selectionRect, _color, 17, 1f, -36, 7.45f);

                for (int i = 0; i < transforms.Count; i++) //adds additional lines for nested objects
                {
                    if (transforms[i] &&
                        _gameObject.transform.childCount == 0 &&
                        transforms[i].GetChild(transforms[i].childCount - 1).gameObject == _gameObject)
                        DrawLine(_selectionRect, _color, 1, 8f, -36f - (14f * i));
                    else
                        DrawLine(_selectionRect, _color, 1, 16, -36f - (14f * i));
                }
            }

            drawLines = false;
        }

        private static void DrawLine(Rect _selectionRect, Color _color, float _width, float _height, float _xOffset,
            float _yOffset = 0)
        {
            EditorGUI.DrawRect(
                new Rect(_selectionRect.xMin + _xOffset, _selectionRect.yMin + _yOffset, _width, _height), _color);
        }

        #endregion

        private static List<Transform> GetParentCount(GameObject _gameObject)
        {
            List<Transform> parents = new List<Transform>();

            Transform current = _gameObject.transform;

            while (current.parent != null)
            {
                current = current.parent;
                parents.Add(current);
            }

            return parents;
        }

        private static bool IsGameObjectEnabled(int _instanceID)
        {
            return EditorUtility.GetObjectEnabled(EditorUtility.InstanceIDToObject(_instanceID)) == 1;
        }

        private static GUIStyle SetStylePreset(Label _preset, int _instanceID, Rect _rect)
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

            var colorFaded = backgroundColor;
            colorFaded.a = 0;

            GUIStyle style = new GUIStyle
            {
                normal = new GUIStyleState()
                {
                    background = Utilities.CrateGradientTexture((int)_rect.width, (int)_rect.height, colorFaded,
                        backgroundColor),
                    textColor = textColor
                },
                hover = new GUIStyleState()
                {
                    background = Utilities.DrawCube(1, 1, LabelManager.HoveredColor),
                    textColor = textColor
                },

                fontStyle = _preset.fontStyle,
                alignment = _preset.alignment
            };

            return style;
        }
    }
}