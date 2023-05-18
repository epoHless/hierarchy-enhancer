using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HierarchyWindowLabelEditorWindow : EditorWindow
{
    private HierarchyLabelPreset selectedPreset = null;

    string labelName = String.Empty;

    private Vector2 labelsScrollPos = Vector2.zero;
    private Vector2 tooltipsScrollPos = Vector2.zero;
    private Vector2 gameObjectsScrollPos = Vector2.zero;
    
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
        InitUI();
    }

    private void InitUI()
    {
        minSize = new Vector2(710, 400);
        maxSize = minSize;

        if(LabelManager.Presets.Count > 0) selectedPreset = LabelManager.Presets[0];
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

                if (GUILayout.Button("Fetch Labels", GUILayout.Width(223)))
                {
                    LabelManager.FetchLabels();
                }
                
                if (LabelManager.Presets.Count > 0)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Add All", GUILayout.Width(110)))
                    {
                        for (int i = 0; i < LabelManager.Presets.Count; i++)
                        {
                            LabelManager.AddPreset(LabelManager.Presets[i]);
                        }
                    }
        
                    if (GUILayout.Button("Remove All", GUILayout.Width(110)))
                    {
                        for (int i = 0; i < LabelManager.Presets.Count; i++)
                        {
                            LabelManager.RemovePreset(LabelManager.Presets[i]);
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

                    GUILayout.Space(-20);
            
                    GUI.DrawTexture(new Rect(new Vector2(230, 0), new Vector2(3, 400)), HierarchyUtilities.DrawCube(1,1, new Color(0.16f, 0.16f, 0.16f, 1f)));
            
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
                GUILayout.Label("Show Hierarchy Lines");
                LabelManager.ShowHierarchyLines = GUILayout.Toggle(LabelManager.ShowHierarchyLines, "");

                if (GUILayout.Button("Change Default Directory"))
                {
                    var directory = EditorUtility.OpenFolderPanel("Change Directory", "", "");

                    directory = directory.Substring(directory.IndexOf("Assets"));
                    PlayerPrefs.SetString("LabelDirectory", directory);
                    
                    LabelManager.FetchLabels();
                    InitUI();
                }
                
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

        labelTab = GUILayout.Toolbar(labelTab, new[] { "Style", "Icons & Info", "GameObjects" });

        GUILayout.EndHorizontal();
        
        switch (labelTab)
        {
            case 0 :
            {
                GUILayout.BeginVertical(GUILayout.Width(463));

                GUILayout.Space(10);
                _editor!.ShowIdentifierIcon();
                GUILayout.Space(10);
                _editor!.ShowFontStyleAlignment();
                GUILayout.Space(10);
                _editor!.ShowTextColorBGColor();
                GUILayout.Space(10);
                _editor!.ShowCustomInactiveColors();
                
                GUILayout.EndVertical();
                break;
            }

            case 1:
            {
                GUILayout.Space(20);
                
                GUILayout.BeginVertical(GUILayout.Width(468));
                
                tooltipsScrollPos = GUILayout.BeginScrollView(tooltipsScrollPos, false, false);

                GUILayout.BeginHorizontal(GUILayout.Width(460));

                EditorGUILayout.LabelField($"Tooltips Count = {_editor.script.tooltips.Count}");
    
                if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    _editor.script.tooltips.Add(new ImageTooltip());
                }

                GUILayout.EndHorizontal();
                
                for (int i = 0; i < _editor.script.tooltips.Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(460));

                    _editor.script.tooltips[i].tooltip = EditorGUILayout.TextField(_editor.script.tooltips[i].tooltip);
                    _editor.script.tooltips[i].icon = (Texture)EditorGUILayout.ObjectField(_editor.script.tooltips[i].icon, typeof(Texture), true);

                    if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        if (_editor.script.tooltips.Contains(_editor.script.tooltips[i]))
                            _editor.script.tooltips.Remove(_editor.script.tooltips[i]);
                    }

                    GUILayout.EndHorizontal();
                }
                
                GUILayout.EndScrollView();
                
                GUILayout.EndVertical();

                break;
            }

            case 2:
            {
                GUILayout.Space(20);
                RenderGameObjects(_editor);

                break;
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void RenderGameObjects(LabelColorPresetEditor _editor)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(460));

        EditorGUILayout.LabelField($"GameObjects Count = {_editor.script.gameObjects.Count}");
        
        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
        {
            _editor.script.gameObjects.Add(new ObjectIDDictionary());
        }

        GUILayout.EndHorizontal();

        gameObjectsScrollPos = GUILayout.BeginScrollView(gameObjectsScrollPos, false, false);

        for (int i = 0; i < _editor.script.gameObjects.Count; i++)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(460));

            _editor.script.gameObjects[i].GameObject =
                (GameObject)EditorGUILayout.ObjectField(_editor.script.gameObjects[i].GameObject, typeof(GameObject), true);

            foreach (var preset in LabelManager.Presets)
            {
                var gameObjects = preset.gameObjects.Where(_o => _o != null);

                if (_editor.script != preset && gameObjects.Contains(_editor.script.gameObjects[i]))
                {
                    _editor.script.gameObjects[i] = null;
                }
            }

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (_editor.script.gameObjects.Contains(_editor.script.gameObjects[i]))
                    _editor.script.gameObjects.Remove(_editor.script.gameObjects[i]);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Creates all the label buttons
    /// </summary>
    private void RenderPresets()
    {
        labelsScrollPos = GUILayout.BeginScrollView(labelsScrollPos, false, false);
        
        var guiColor = GUI.color;

        if (LabelManager.Presets.Count == 0)
        {
            GUILayout.Label("No Presets were found!");
            GUILayout.Label("Click Add Label to create one...");
        }
        else
        {
            for (int i = 0; i < LabelManager.Presets.Count; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(220));
            
                GUI.color = LabelManager.Presets[i].textColor;

                if (GUILayout.Button(LabelManager.Presets[i].name.Split("_")[1], GUILayout.Width(180)))
                {
                    selectedPreset = LabelManager.Presets[i];
                }
                
                LabelManager.Presets[i].isActive = GUILayout.Toggle(LabelManager.Presets[i].isActive, "");

                GUI.color = Color.red;

                if (GUILayout.Button("X" , GUILayout.Width(20)))
                {
                    DeleteLabel(LabelManager.Presets[i]);
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

            label.tooltips = new List<ImageTooltip>();
            label.gameObjects = new List<ObjectIDDictionary>();
            
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

        string assetPath = String.Concat(LabelManager.LabelsDirectory, $"/{_label.name}.asset");

        if (File.Exists(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            selectedPreset = LabelManager.Presets[0];
            LabelManager.Presets.Remove(_label);
        }
        else
        {
            EditorUtility.DisplayDialog("ERROR", $"Could not find asset at path: {assetPath}", "OK");
        }
    }
}
