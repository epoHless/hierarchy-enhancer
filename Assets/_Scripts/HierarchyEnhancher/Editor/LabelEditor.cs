using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer
{
    [CustomEditor(typeof(Label))]
    internal class LabelEditor : Editor
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

        // public override void OnInspectorGUI()
        // {
        // }

        internal void ShowTextColorBGColor()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Color", labelStyle);
            script.textColor = EditorGUILayout.ColorField(script.textColor, GUILayout.Width(220));

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            script.useCustomBackground = EditorGUILayout.Toggle("Custom BG", script.useCustomBackground);
            if (script.useCustomBackground)
            {
                EditorGUILayout.LabelField("Background Color", labelStyle);
                script.backgroundColor = EditorGUILayout.ColorField(script.backgroundColor, GUILayout.Width(220));
            }
            else
            {
                script.backgroundColor = script.textColor;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        public void ShowFontStyleAlignment()
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
        }

        public void ShowIdentifierIcon()
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Icon", labelStyle);
            script.icon = EditorGUILayout.ObjectField(script.icon, typeof(Texture), true) as Texture;
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
}