using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class HierarchyEnhancer
    {
        private static IRenderer[] _drawables;

        static HierarchyEnhancer()
        {
            _drawables = new IRenderer[]
            {
                new ParentRenderer(Color.gray),
                new ComponentRenderer(),
                new FocusRenderer()
            };
            
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchy;
            EditorApplication.hierarchyChanged += DrawHierarchyLines;

            LabelManager.OnRequestLineDraw += OnRequestLineDraw;
        }

        static void DrawHierarchy(int _instanceID, Rect _selectionRect)
        {
            var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);
            var gameObject = EditorUtility.InstanceIDToObject(_instanceID) as GameObject;

            if (!gameObject) return;

            
            for (int i = 0; i < _drawables.Length; i++)
            {
                _drawables[i].OnGUI(_instanceID, _selectionRect, gameObject);
            }
            
            if (gameObject != null)
            {
                RenderGameObjectToggle(_selectionRect, gameObject);
            }

            foreach (var preset in LabelManager.Labels)
            {
                if (!preset.isActive) continue;

                bool value = preset.gameObjects is { Count: > 0 } &&
                             preset.gameObjects.Find(_dictionary => _dictionary.GameObject == gameObject) != null;

                if (gameObject != null && value)
                {
                    string text = content.text;
                    RenderGUI(_instanceID, _selectionRect, text, preset, gameObject);
                }
            }
        }

        private static void DrawHierarchyLines()
        {
            // drawLines = true;
        }

        private static void OnRequestLineDraw(bool _value)
        {
            // drawLines = _value;
        }

        #region Render Methods

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
            // RenderFocusButton(_selectionRect, _gameObject);

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

        #endregion

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
#endif
}