using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HierarchyWindowLabelEditorWindow : EditorWindow
{
    private HierarchyLabelPreset selectedPreset = null;
    private List<HierarchyLabelPreset> Presets = new List<HierarchyLabelPreset>();

    private GUIStyle sidebarStyle;
    
    string labelName = String.Empty;

    private Vector2 labelsScrollPos = Vector2.zero;
    private Vector2 tooltipsScrollPos = Vector2.zero;
    
    int labelTab = 0;
    int optionsTab = 0;

    [MenuItem("Utilities/Hierarchy Labels")]
    private static void ShowWindow()
    {
        var window = GetWindow<HierarchyWindowLabelEditorWindow>();
        window.titleContent = new GUIContent("Hierarchy Editor");
        window.Show();
    }

    private void Awake()
    {
        Presets = new List<HierarchyLabelPreset>(LabelManager.Presets);
        
        sidebarStyle = new GUIStyle()
        {
            hover = new GUIStyleState()
            {
                textColor = Color.cyan
            },
            normal = new GUIStyleState()
            {
                background = HierarchyUtilities.DrawCube(1,1, Color.gray)
            }
        };

        minSize = new Vector2(710, 400);
        maxSize = minSize;

        if(Presets.Count > 0) selectedPreset = Presets[0];
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        optionsTab = GUILayout.Toolbar(optionsTab, new[] { "Labels", "Options" }, GUILayout.Width(225));

        switch (optionsTab) //toggle panels
        {
            case 0: //labels panel
            {
                EditorGUILayout.BeginHorizontal(); //1

                EditorGUILayout.BeginVertical(); //2

                RenderPresets();
        
                GUILayout.FlexibleSpace();

                if (Presets.Count > 0)
                {
                    if (GUILayout.Button("Fetch Labels", GUILayout.Width(223)))
                    {
                        LabelManager.FetchLabels();
                        Presets = new List<HierarchyLabelPreset>(LabelManager.Presets);
                    }
        
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Add All", GUILayout.Width(110)))
                    {
                        for (int i = 0; i < Presets.Count; i++)
                        {
                            AddPreset(Presets[i]);
                        }
                    }
        
                    if (GUILayout.Button("Remove All", GUILayout.Width(110)))
                    {
                        for (int i = 0; i < Presets.Count; i++)
                        {
                            RemovePreset(Presets[i]);
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();

                labelName = GUILayout.TextField(labelName, GUILayout.Width(110));
        
                if (GUILayout.Button("New Label", GUILayout.Width(110)))
                {
                    if(labelName != String.Empty) AddNewLabel(labelName);
                }

                GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical(); //2

                if(selectedPreset) //Create the editor for the selected Preset
                {
                    var editor = Editor.CreateEditor(selectedPreset) as LabelColorPresetEditor;

                    EditorGUILayout.BeginVertical();

                    // Shows the label properties to modify

                    GUILayout.Space(10);
            
                    GUI.DrawTexture(new Rect(new Vector2(232, 0), new Vector2(3, 400)), HierarchyUtilities.DrawCube(1,1, new Color(0.16f, 0.16f, 0.16f, 1f)));
            
                    RenderPreset(editor);
                }

                EditorGUILayout.EndHorizontal(); //1
                
                break;
            }

            case 1: //options panel
            {
                GUILayout.Label("Show GameObject Focus Button");
                LabelManager.ShowFocusButton = GUILayout.Toggle(LabelManager.ShowFocusButton, "");
                GUILayout.Label("Show GameObject Toggle Button");
                LabelManager.ShowToggleButton = GUILayout.Toggle(LabelManager.ShowToggleButton, "");
                break;
            }
        }
        
        
        
        EditorGUILayout.EndVertical();
        
        EditorApplication.RepaintHierarchyWindow();
    }

    private void RenderPreset(LabelColorPresetEditor _editor)
    {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label(selectedPreset.name.Split('_')[1], new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState()
            {
                textColor = selectedPreset.textColor
            },
            fontSize = 14
        });

        labelTab = GUILayout.Toolbar(labelTab, new[] { "Style", "Icons & Info" });

        GUILayout.EndHorizontal();
        
        switch (labelTab)
        {
            case 0 :
            {
                _editor.showBase = false;
                GUILayout.Space(10);

                _editor!.ShowIdentifierIcon();
                GUILayout.Space(10);
                _editor!.ShowFontStyleAlignment();
                GUILayout.Space(10);
                _editor!.ShowTextColorBGColor();
                GUILayout.Space(10);
                _editor!.ShowCustomInactiveColors();
                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                _editor!.ShowPresetButtons();
                break;
            }

            case 1:
            {
                _editor.showBase = true;

                if (_editor is Editor _base)
                {
                    GUILayout.BeginVertical(GUILayout.Width(455));
                    tooltipsScrollPos = GUILayout.BeginScrollView(tooltipsScrollPos, false, false);
                    
                    _base.OnInspectorGUI();
                    
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                
                break;
            }
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Creates all the label buttons
    /// </summary>
    private void RenderPresets()
    {
        labelsScrollPos = GUILayout.BeginScrollView(labelsScrollPos, false, false);
        
        var guiColor = GUI.color;

        if (Presets.Count == 0)
        {
            GUILayout.Label("No Presets were found!");
            GUILayout.Label("Click Add Label to create one...");
        }
        else
        {
            for (int i = 0; i < Presets.Count; i++)
            {
                GUILayout.BeginHorizontal();
            
                GUI.color = Presets[i].textColor;

                if (GUILayout.Button(Presets[i].name.Split("_")[1], GUILayout.Width(200)))
                {
                    selectedPreset = Presets[i];
                }
                GUI.color = Color.red;

                if (GUILayout.Button("X" , GUILayout.Width(20)))
                {
                    DeleteLabel(Presets[i]);
                }

                GUI.color = guiColor;
            
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Create and store in the default folder a new label with a given name.
    /// </summary>
    /// <param name="_name"></param>
    private void AddNewLabel(string _name)
    {
        var label = ScriptableObject.CreateInstance<HierarchyLabelPreset>();
        string labelPath = String.Concat(LabelManager.LabelsDirectory, $"/LabelPreset_{_name}.asset");

        if (!File.Exists(labelPath))
        {
            AssetDatabase.CreateAsset(label, labelPath);
            
            label.identifier = labelName;
            label.textColor = Color.white;
            label.inactiveTextColor = Color.white;
            label.backgroundColor = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);
            label.inactiveBackgroundColor = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);
        
            AddPreset(label);
            LabelManager.AddPreset(label);
            EditorApplication.RepaintHierarchyWindow();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            selectedPreset = label;
        }
        else
        {
            EditorUtility.DisplayDialog("ERROR", $"asset already exists at path: {labelPath}", "OK");
        }
        
        labelName = String.Empty;
    }

    /// <summary>
    /// Deletes the selected label from the Assets Folder
    /// </summary>
    /// <param name="_label"></param>
    private void DeleteLabel(HierarchyLabelPreset _label)
    {
        LabelManager.RemovePreset(_label);
        RemovePreset(_label);

        string assetPath = String.Concat(LabelManager.LabelsDirectory, $"/{_label.name}.asset");

        if (File.Exists(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            selectedPreset = Presets[0];
            Presets.Remove(_label);
        }
        else
        {
            EditorUtility.DisplayDialog("ERROR", $"Could not find asset at path: {assetPath}", "OK");
        }
    }

    /// <summary>
    /// Add a preset to the list of presets
    /// </summary>
    /// <param name="_preset"></param>
    private void AddPreset(HierarchyLabelPreset _preset)
    {
        if (!Presets.Contains(_preset))
        {
            Presets.Add(_preset);
        }
        
        LabelManager.AddPreset(_preset);
    }
    
    /// <summary>
    /// Remove a preset from the list of presets
    /// </summary>
    /// <param name="_preset"></param>
    private void RemovePreset(HierarchyLabelPreset _preset)
    {
        LabelManager.RemovePreset(_preset);
    }
}
