using UnityEngine;

namespace HierarchyEnhancer.Editor
{
    public class IconRenderer : IRenderer
    {
        public bool IsEnabled { get; set; } = true;

        public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
        {
            int offset = LabelManager.ShowToggleButton ? 32 : 16;

            foreach (var label in LabelManager.Labels)
            {
                foreach (var gameObject in label.gameObjects)
                {
                    if (!gameObject.GameObject) continue;

                    for (int i = 0; i < gameObject.tooltips.Count; i++)
                    {
                        if (!gameObject.tooltips[i].icon || gameObject.GameObject != _gameObject) continue;

                        var rect = new Rect(_selectionRect.xMax - offset, _selectionRect.yMin, 16, 16);

                        if (GUI.Button(rect,
                                new GUIContent(gameObject.tooltips[i].icon, gameObject.tooltips[i].tooltip),
                                GUIStyle.none))
                        {
                            var infoWindow = ScriptableObject.CreateInstance<LabelInfoWindow>();
                            infoWindow.Open(gameObject.tooltips[i], i);
                        }

                        var iconRect = rect;
                        GUI.DrawTexture(iconRect, gameObject.tooltips[i].icon);

                        offset += 16;
                    }
                }
            }
        }
    }
}
