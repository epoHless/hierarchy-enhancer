using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LabelManager
{
    public static string LabelsDirectory =>
        PlayerPrefs.HasKey("LabelDirectory") ? PlayerPrefs.GetString("LabelDirectory") : null;

    public static List<HierarchyLabelPreset> Presets = new List<HierarchyLabelPreset>();

    public static readonly Color SelectedColor = new (0.17f, 0.36f, 0.53f, 1f);
    public static readonly Color UnselectedColor = new(0.22f, 0.22f, 0.22f, 1f);
    public static readonly Color HoveredColor = new (0.27f, 0.27f, 0.27f, 1f);

    public static bool ShowFocusButton = true;
    public static bool ShowToggleButton = true;
    public static bool ShowHierarchyLines = true;
    
    static LabelManager()
    {
        EditorApplication.delayCall += FetchLabels;
    }
    
    public static void FetchLabels()
    {
        if (string.IsNullOrEmpty(LabelsDirectory))
        {
            Debug.LogWarning("There is no label directory selected! Pick one in the Label Editor -> Options");
        }
        else
        {
            var assets = AssetDatabase.FindAssets("t:HierarchyLabelPreset", new[] { LabelsDirectory });

            Presets = new List<HierarchyLabelPreset>();
        
            foreach (var asset in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);
                var item = AssetDatabase.LoadAssetAtPath(path, typeof(HierarchyLabelPreset)) as HierarchyLabelPreset;

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
    }
    
    public static void AddPreset(HierarchyLabelPreset _preset)
    {
        if (!Presets.Contains(_preset))
        {
            Presets.Add(_preset);
        }
        
        EditorApplication.RepaintHierarchyWindow();
    }
    
    public static void RemovePreset(HierarchyLabelPreset _preset)
    {
        if (Presets.Contains(_preset))
        {
            Presets.Remove(_preset);
        }
        
        EditorApplication.RepaintHierarchyWindow();
    }
}
