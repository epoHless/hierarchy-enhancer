using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
    public class ParentRenderer : IRenderer
    {
        public bool IsEnabled { get; set; } = true;
        private Color color;

        public ParentRenderer(Color _color)
        {
            color = new Color(0.44f, 0.44f, 0.44f, 1f);
        }

        public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
        {
            if (!LabelManager.ShowHierarchyLines) return;

            if (_gameObject.transform.childCount == 0 && !_gameObject.transform.parent)
            {
                DrawLine(_selectionRect, color, 1, 16, -8f);
            }

            var transforms = GetParentCount(_gameObject);

            if (_gameObject.transform.parent != null)
            {
                if (_gameObject.transform.childCount == 0)
                {
                    DrawLine(_selectionRect, color, 16f, 1f, -22f, 7.45f);
                    // DrawLine(_selectionRect, color, 1, 16, -8f);
                }
                else
                    DrawLine(_selectionRect, color, 8, 1f, -22, 7.45f);

                for (int i = 0; i < transforms.Count; i++) //adds additional lines for nested objects
                {
                    if (transforms[i] &&
                        _gameObject.transform.childCount == 0 &&
                        transforms[i].GetChild(transforms[i].childCount - 1).gameObject == _gameObject)
                        DrawLine(_selectionRect, color, 1, 8f, -22f - (14f * i));
                    else
                        DrawLine(_selectionRect, color, 1, 16, -22f - (14f * i));
                }
            }
        }

        private void DrawLine(Rect _selectionRect, Color _color, float _width, float _height, float _xOffset,
            float _yOffset = 0)
        {
            EditorGUI.DrawRect(
                new Rect(_selectionRect.xMin + _xOffset, _selectionRect.yMin + _yOffset, _width, _height), _color);
        }

        private List<Transform> GetParentCount(GameObject _gameObject)
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
    }
}
