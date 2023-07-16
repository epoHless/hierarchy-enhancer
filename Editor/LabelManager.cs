using System.Collections.Generic;
using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    
    [InitializeOnLoad]
    public static class LabelManager
    {
        internal static string LabelsDirectory =>
            PlayerPrefs.HasKey("LabelDirectory") ? PlayerPrefs.GetString("LabelDirectory") : null;

        internal static List<Label> Labels = new List<Label>();

        internal static readonly Color SelectedColor = new(0.17f, 0.36f, 0.53f, 1f);
        internal static readonly Color UnselectedColor = new(0.22f, 0.22f, 0.22f, 1f);
        internal static readonly Color HoveredColor = new(0.27f, 0.27f, 0.27f, 1f);

        internal static bool ShowFocusButton = true;
        internal static bool ShowToggleButton = true;
        internal static bool ShowHierarchyLines { get; set; } = true;

        static LabelManager()
        {
            EditorApplication.quitting += SaveAssets;
        }

        public static List<Label> FetchLabels()
        {
            if (string.IsNullOrEmpty(LabelsDirectory))
            {
                Debug.LogWarning("There is no label directory selected! Pick one in the Label Editor -> Options");
                return null;
            }
            else
            {
                var assets = AssetDatabase.FindAssets($"t:{typeof(Label)}", new[] { LabelsDirectory });

                Labels = new List<Label>();

                foreach (var asset in assets)
                {
                    var path = AssetDatabase.GUIDToAssetPath(asset);
                    var item = AssetDatabase.LoadAssetAtPath(path, typeof(Label)) as Label;

                    if (item)
                    {
                        AddPreset(item);

                        foreach (var dictionary in item.gameObjects)
                        {
                            dictionary.GameObject = GameObject.Find(dictionary.ID);
                        }
                    }
                }
            }

            foreach (var preset in Labels)
            {
                foreach (var gameObject in preset.gameObjects)
                {
                    if (!gameObject.GameObject)
                    {
                        gameObject.GameObject = GameObject.Find(gameObject.ID);
                    }
                }
            }

            return Labels;
        }
        
        private static void SaveAssets()
        {
            foreach (var preset in LabelManager.Labels)
            {
                EditorUtility.SetDirty(preset);
                AssetDatabase.SaveAssetIfDirty(preset);
                AssetDatabase.Refresh();
            }

            EditorApplication.quitting -= SaveAssets;
        }

        public static void AddPreset(Label _preset)
        {
            if (!Labels.Contains(_preset))
            {
                Labels.Add(_preset);
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        public static void RemovePreset(Label _preset)
        {
            if (Labels.Contains(_preset))
            {
                Labels.Remove(_preset);
            }

            EditorApplication.RepaintHierarchyWindow();
        }
    }
#endif
}
