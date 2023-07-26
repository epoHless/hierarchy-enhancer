using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Label))]
    public class LabelEditor : UnityEditor.Editor
    {
        public Label script;
        private GUIStyle labelStyle; 

        private void Awake()
        {
            script = (Label)target;

            labelStyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 13,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                }
            };

            EditorUtility.SetDirty(script);
        }

        private void OnDisable()
        {
            AssetDatabase.SaveAssetIfDirty(script);
        }

        internal void RenderColors()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Color", labelStyle);
            script.textColor = EditorGUILayout.ColorField(script.textColor);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (script.useCustomBackground)
            {
                EditorGUILayout.LabelField("Background Color", labelStyle);
                script.backgroundColor = EditorGUILayout.ColorField(script.backgroundColor);
            }
            else
            {
                script.backgroundColor = script.textColor;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            
            script.useCustomBackground = EditorGUILayout.Toggle("Custom BG", script.useCustomBackground);

            script.useGradient = script.useCustomBackground && EditorGUILayout.Toggle("Gradient", script.useGradient);
            
            EditorGUILayout.EndHorizontal();
        }

        public void RenderFont()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Font Style", labelStyle);
            script.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(script.fontStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Alignment", labelStyle);
            script.alignment = (TextAnchor)EditorGUILayout.EnumPopup(script.alignment);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Font", labelStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("font"));
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Font Size", labelStyle);
            script.fontSize = EditorGUILayout.IntField(script.fontSize);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        public void RenderIcon()
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Icon", labelStyle);
            script.icon = EditorGUILayout.ObjectField(script.icon, typeof(Texture), true) as Texture;
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}