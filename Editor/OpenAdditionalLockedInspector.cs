using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    public static class OpenAdditionalLockedInspector
    {
        public static void DisplayLockedInspector()
        {
            // Create new inspector window
            var inspectorWindow = ScriptableObject.CreateInstance(GetInspectorWindowType()) as EditorWindow;
            inspectorWindow.Show();

            // Lock new inspector window
            LockInspector(GetInspectorWindowType());
        }

        private static Type GetInspectorWindowType()
        {
            return typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        }

        private static void LockInspector(Type _obj)
        {
            var fields = _obj.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                if (field.Name == "isLocked")
                    field.SetValue(_obj, true);
            }
        }
    }
#endif
}