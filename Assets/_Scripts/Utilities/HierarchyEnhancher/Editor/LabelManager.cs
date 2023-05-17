using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

[InitializeOnLoad]
public static class LabelManager
{
    public static string LabelsDirectory { get; private set; }

    public static List<HierarchyLabelPreset> Presets = new List<HierarchyLabelPreset>();

    public static readonly Color SelectedColor = new Color(44f / 255f, 93f / 255f, 135f / 255f, 1f);
    public static readonly Color UnselectedColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
    
    static LabelManager()
    {
        AssemblyReloadEvents.afterAssemblyReload += FetchLabels;
    }
    
    public static void FetchLabels()
    {
        if (!Directory.Exists($"{Application.dataPath}/HierarchyLabels"))
            AssetDatabase.CreateFolder("Assets", "HierarchyLabels");
        
        LabelsDirectory = "Assets/HierarchyLabels/";

        var assets = AssetDatabase.FindAssets("", new[] { LabelsDirectory });

        Presets = new List<HierarchyLabelPreset>();
        
        foreach (var asset in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var item = AssetDatabase.LoadAssetAtPath(path, typeof(HierarchyLabelPreset)) as HierarchyLabelPreset;

            if (item)
            {
                AddPreset(item);
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
