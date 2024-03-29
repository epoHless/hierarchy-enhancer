﻿using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    public class ComponentRenderer : IRenderer
    {
        public bool IsEnabled { get; set; } = true;

        private Color GUIColor;
        
        public ComponentRenderer()
        {
            GUIColor = GUI.color;
        }
        
        public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject)
        {
            var objectContent = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(_instanceID), null);
        
            var textWidth = GUI.skin.label.CalcSize(new GUIContent() { text = objectContent.text  });
            
            var components = _gameObject.GetComponents(typeof(Component));
            int compOffset = 16;

            if (components.Length > 1)
            {
                for (int i = 1; i < components.Length; i++)
                {
                    var content = EditorGUIUtility.ObjectContent(_gameObject.GetComponents(typeof(Component))[i],
                        typeof(Component));

                    var text = content.text.Remove(0, _gameObject.name.Length);
                    
                    var rect = new Rect(_selectionRect.xMin + textWidth.x + compOffset, _selectionRect.y, 16f, 16f);

                    var component = components[i] as Object;
                    bool isHidden = component.hideFlags == HideFlags.HideInInspector;

                    if (GUI.Button(rect, new GUIContent("", text), GUIStyle.none))
                    {
                        if(component)
                        {
                            component.hideFlags = isHidden ? HideFlags.None : HideFlags.HideInInspector;
                        }
                    }

                    if(component && isHidden) GUI.color = Color.red;

                    GUI.DrawTexture(rect, content.image);
                    
                    GUI.color = GUIColor;

                    compOffset += 17;
                }
            }
        }
    }
#endif
}
