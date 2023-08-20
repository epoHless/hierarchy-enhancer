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
        private static string LabelsDirectory =>
            PlayerPrefs.HasKey("LabelDirectory") ? PlayerPrefs.GetString("LabelDirectory") : null;

        internal static List<Label> Labels = new List<Label>();

        #region Renderer Toggles

        private static bool _showFocusButton = true;
        internal static bool ShowFocusButton
        {
            get => _showFocusButton;
            set
            {
                _showFocusButton = value;
                HierarchyEnhancer.ToggleRender<FocusRenderer>(value);
            }
        }

        private static bool _showToggleButton = true;
        internal static bool ShowToggleButton
        {
            get => _showToggleButton;
            set
            {
                _showToggleButton = value;
                HierarchyEnhancer.ToggleRender<ToggleRenderer>(value);
            }
        }

        private static bool _showHierarchyLines = true;
        internal static bool ShowHierarchyLines
        {
            get => _showHierarchyLines;
            set
            {
                _showHierarchyLines = value;
                HierarchyEnhancer.ToggleRender<ParentRenderer>(value);
            }
        }

        private static bool _showLabels = true;
        public static bool ShowLabels
        {
            get => _showLabels;
            set
            {
                _showLabels = value;
                HierarchyEnhancer.ToggleRender<LabelRenderer>(value);
            }
        }
        
        private static bool _showComponents = true;
        public static bool ShowComponents
        {
            get => _showComponents;
            set
            {
                _showComponents = value;
                HierarchyEnhancer.ToggleRender<ComponentRenderer>(value);
            }
        }
        
        private static bool _showIcons = true;
        public static bool ShowIcons
        {
            get => _showIcons;
            set
            {
                _showIcons = value;
                HierarchyEnhancer.ToggleRender<IconRenderer>(value);
            }
        }

        #endregion

        static LabelManager()
        {
            EditorApplication.quitting += OnQuit;
        }

        [UnityEditor.Callbacks.DidReloadScripts, InitializeOnLoadMethod]
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
        
        private static void OnQuit()
        {
            foreach (var preset in LabelManager.Labels)
            {
                EditorUtility.SetDirty(preset);
                AssetDatabase.SaveAssetIfDirty(preset);
                AssetDatabase.Refresh();
            }

            EditorApplication.quitting -= OnQuit;
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
        
        public static void CreateLabel(string _name)
        {
            var label = ScriptableObject.CreateInstance<Label>();
            string labelPath = String.Concat(LabelManager.LabelsDirectory, $"/LabelPreset_{_name}.asset");

            if (!File.Exists(labelPath))
            {
                AssetDatabase.CreateAsset(label, labelPath);

                label.textColor = Color.white;
                label.backgroundColor = label.textColor;

                label.gameObjects = new List<ObjectDictionary>();

                AddLabel(label);
                EditorApplication.RepaintHierarchyWindow();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", $"asset already exists at path: {labelPath}", "OK");
            }
        }
        
        public static void DeleteLabel(Label _label)
        {
            RemoveLabel(_label);

            string assetPath = String.Concat(LabelManager.LabelsDirectory, $"/{_label.name}.asset");

            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

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