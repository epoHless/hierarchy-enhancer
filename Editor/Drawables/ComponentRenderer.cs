using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
    public class ComponentRenderer : IRenderer
    {
        public bool IsEnabled { get; set; }
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

                    var text = content.text;
                    text = text.Substring(text.IndexOf('(') + 1).Trim(')');

                    var rect = new Rect(_selectionRect.xMin + 2 + textWidth.x + compOffset, _selectionRect.yMin, 15f, 15f);
                    
                    if (GUI.Button(rect, new GUIContent() { tooltip = text }))
                    {
                        OpenAdditionalLockedInspector.DisplayLockedInspector();
                    }

                    GUI.DrawTexture(rect, content.image);
                    
                    compOffset += 16;
                }
            }
        }
    }
}
