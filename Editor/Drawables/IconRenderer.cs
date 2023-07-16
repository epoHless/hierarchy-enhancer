using System.Collections.Generic;
using HierarchyEnhancer.Editor;
using HierarchyEnhancer.Runtime;
using UnityEngine;

public class IconRenderer : IRenderer
{
    private List<Label> labels;

    public IconRenderer(List<Label> _labels)
    {
        labels = _labels;
        
        foreach (var label in labels)
        {
            Debug.Log(label.name);
        }
    }

    public bool IsEnabled { get; set; }
    public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
    {
        int offset = LabelManager.ShowToggleButton ? 32 : 16;

        foreach (var label in labels)
        {
            for (int i = 0; i < label.tooltips.Count; i++)
            {
                if (!label.tooltips[i].icon) continue;

                var rect = new Rect(_selectionRect.xMax - offset, _selectionRect.yMin, 15, 15);
                
                if (GUI.Button(rect, new GUIContent(label.tooltips[i].icon, label.tooltips[i].tooltip), GUIStyle.none))
                {
                    var infoWindow = ScriptableObject.CreateInstance<LabelInfoWindow>();
                    infoWindow.Open(label, i);
                }

                var iconRect = rect;
                GUI.DrawTexture(iconRect, label.tooltips[i].icon);

                offset += 16;
            }
        }
    }
}
