﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer
{
    [InitializeOnLoad]

    internal static class LabelManager
    {
        internal static string LabelsDirectory =>
            PlayerPrefs.HasKey("LabelDirectory") ? PlayerPrefs.GetString("LabelDirectory") : null;

        internal static List<Label> Presets = new List<Label>();

        internal static readonly Color SelectedColor = new(0.17f, 0.36f, 0.53f, 1f);
        internal static readonly Color UnselectedColor = new(0.22f, 0.22f, 0.22f, 1f);
        internal static readonly Color HoveredColor = new(0.27f, 0.27f, 0.27f, 1f);

        internal static bool ShowFocusButton = true;
        internal static bool ShowToggleButton = true;

        private static bool _showHierarchyLines = true;
        internal static bool ShowHierarchyLines
        {
            get => _showHierarchyLines;
            set
            {
                _showHierarchyLines = value;
                OnRequestLineDraw?.Invoke(_showHierarchyLines);
            }
        }

        internal delegate void Evt<T>(T _value);

        internal static Evt<bool> OnRequestLineDraw;

        static LabelManager()
        {
            EditorApplication.delayCall += FetchLabels;
            EditorApplication.quitting += SaveAssets;
        }

        public static void FetchLabels()
        {
            if (string.IsNullOrEmpty(LabelsDirectory))
            {
                Debug.LogWarning("There is no label directory selected! Pick one in the Label Editor -> Options");
            }
            else
            {
                var assets = AssetDatabase.FindAssets($"t:{typeof(Label)}", new[] { LabelsDirectory });

                Presets = new List<Label>();

                foreach (var asset in assets)
                {
                    var path = AssetDatabase.GUIDToAssetPath(asset);
                    var item = AssetDatabase.LoadAssetAtPath(path, typeof(Label)) as Label;

                    if (item)
                    {
                        AddPreset(item);

                        foreach (var dictionary in item.gameObjects)
                        {
                            dictionary.GameObject = EditorUtility.InstanceIDToObject(dictionary.ID) as GameObject;
                        }
                    }
                }
            }

            foreach (var preset in Presets)
            {
                foreach (var gameObject in preset.gameObjects)
                {
                    if (!gameObject.GameObject)
                    {
                        gameObject.GameObject = EditorUtility.InstanceIDToObject(gameObject.ID) as GameObject;
                    }
                }
            }
        }
        
        private static void SaveAssets()
        {
            foreach (var preset in LabelManager.Presets)
            {
                EditorUtility.SetDirty(preset);
                AssetDatabase.SaveAssetIfDirty(preset);
                AssetDatabase.Refresh();
            }
        }

        public static void AddPreset(Label _preset)
        {
            if (!Presets.Contains(_preset))
            {
                Presets.Add(_preset);
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        public static void RemovePreset(Label _preset)
        {
            if (Presets.Contains(_preset))
            {
                Presets.Remove(_preset);
            }

            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
