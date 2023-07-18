using System;
using System.Collections.Generic;
using System.IO;
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

        public static void FetchLabels()
        {
            if (string.IsNullOrEmpty(LabelsDirectory))
            {
                Debug.LogWarning("There is no label directory selected! Pick one in the Label Editor -> Options");
                return;
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
                        AddLabel(item);

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

        private static void AddLabel(Label _preset)
        {
            if (!Labels.Contains(_preset))
            {
                Labels.Add(_preset);
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        private static void RemoveLabel(Label _preset)
        {
            if (Labels.Contains(_preset))
            {
                Labels.Remove(_preset);
            }

            EditorApplication.RepaintHierarchyWindow();
        }
        
         /// <summary>
        /// Create and store in the default folder a new label with a given name.
        /// </summary>
        /// <param name="_name"></param>
        public static void CreateLabel(string _name)
        {
            var label = ScriptableObject.CreateInstance<Label>();
            string labelPath = String.Concat(LabelManager.LabelsDirectory, $"/LabelPreset_{_name}.asset");

            if (!File.Exists(labelPath))
            {
                AssetDatabase.CreateAsset(label, labelPath);

                label.textColor = Color.white;
                label.backgroundColor = label.textColor;

                // label.tooltips = new List<Tooltip>();
                label.gameObjects = new List<ObjectDictionary>();

                AddLabel(label);
                EditorApplication.RepaintHierarchyWindow();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // _activeLabel = label;
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", $"asset already exists at path: {labelPath}", "OK");
            }

            // labelName = String.Empty;
        }

        /// <summary>
        /// Deletes the selected label from the Assets Folder
        /// </summary>
        /// <param name="_label"></param>
        public static void DeleteLabel(Label _label)
        {
            RemoveLabel(_label);

            string assetPath = String.Concat(LabelManager.LabelsDirectory, $"/{_label.name}.asset");

            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // _activeLabel = LabelManager.Labels.Count > 0 ? LabelManager.Labels[0] : null;
                Labels.Remove(_label);
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", $"Could not find asset at path: {assetPath}", "OK");
            }
        }
    }
#endif
}
