using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HierarchyEnhancer.Runtime;
using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Editor
{
#if UNITY_EDITOR
    public class LabelEditorWindow : EditorWindow
    {
        private Label _activeLabel = null;

        string labelName = String.Empty;

        private Vector2 labelsScrollPos = Vector2.zero;
        private Vector2 tooltipsScrollPos = Vector2.zero;
        private Vector2 gameObjectsScrollPos = Vector2.zero;

        int labelTab = 0;
        int optionsTab = 0;

        [MenuItem("Enhanced Hierarchy/Labels Manager")]
        private static void ShowWindow()
        {
            var window = GetWindow<LabelEditorWindow>();
            window.titleContent = new GUIContent("Labels Manager");
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

            if (LabelManager.Labels.Count > 0) _activeLabel = LabelManager.Labels[0];
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            optionsTab = GUILayout.Toolbar(optionsTab, new[] { "Labels", "Options" }, GUILayout.Width(225));

            switch (optionsTab) //toggle panels
            {
                case 0: //labels panel
                {
                    minSize = new Vector2(710, 400);
                    maxSize = minSize;

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();

                    RenderPresets();

                    GUILayout.FlexibleSpace();

                    GUI.DrawTexture(new Rect(new Vector2(0, 357), new Vector2(230, 3)),
                        Utilities.DrawCube(new Color(0.16f, 0.16f, 0.16f, 1f)));

                    if (GUILayout.Button("Fetch Labels", GUILayout.Width(223)))
                    {
                        LabelManager.FetchLabels();
                    }

                    GUILayout.BeginHorizontal();

                    labelName = GUILayout.TextField(labelName, GUILayout.Width(110));

                    if (GUILayout.Button("New Label", GUILayout.Width(110)))
                    {
                        if (labelName != String.Empty) AddNewLabel(labelName);
                    }

                    GUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    if (_activeLabel) //Create the editor for the selected Preset
                    {
                        var editor = UnityEditor.Editor.CreateEditor(_activeLabel) as LabelEditor;

                        EditorGUILayout.BeginVertical();

                        // Shows the label properties to modify

                        GUILayout.Space(-20);

                        GUI.DrawTexture(new Rect(new Vector2(230, 0), new Vector2(3, 410)),
                            Utilities.DrawCube(new Color(0.16f, 0.16f, 0.16f, 1f)));

                        RenderLabel(editor);
                    }

                    EditorGUILayout.EndHorizontal();

                    break;
                }

                case 1: //options panel
                {
                    minSize = new Vector2(230, 400);
                    maxSize = minSize;

                    GUILayout.Space(10);

                    LabelManager.ShowFocusButton =
                        GUILayout.Toggle(LabelManager.ShowFocusButton, " Show GameObject Focus Button");
                    LabelManager.ShowToggleButton =
                        GUILayout.Toggle(LabelManager.ShowToggleButton, " Show GameObject Toggle Button");
                    LabelManager.ShowHierarchyLines =
                        GUILayout.Toggle(LabelManager.ShowHierarchyLines, " Show Hierarchy Lines");

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Change Default Directory", GUILayout.Width(220)))
                    {
                        var directory = EditorUtility.OpenFolderPanel("Change Directory", "", "");

                        if (string.IsNullOrEmpty(directory)) return;

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

        private void RenderLabel(LabelEditor _editor)
        {
            labelTab = GUILayout.Toolbar(labelTab, new[] { "Style", "Icons & Info", "GameObjects" });

            GUILayout.Label(_activeLabel.name.Split('_')[1] + " Preset", new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    textColor = _activeLabel.textColor
                },
                fontSize = 14
            });

            switch (labelTab)
            {
                case 0:
                {
                    RenderEditor(_editor);
                    break;
                }

                case 1:
                {
                    RenderTooltips(_editor);
                    break;
                }

                case 2:
                {
                    RenderGameObjects(_editor);
                    break;
                }
            }

            EditorGUILayout.EndVertical();
        }

        #region UI Render Methods

        private static void RenderEditor(LabelEditor _editor)
        {
            GUILayout.BeginVertical(GUILayout.Width(463));

            GUILayout.Space(10);
            _editor!.ShowIdentifierIcon();
            GUILayout.Space(10);
            _editor!.ShowFontStyleAlignment();
            GUILayout.Space(10);
            _editor!.ShowTextColorBGColor();

            GUILayout.EndVertical();
        }

        private void RenderTooltips(LabelEditor _editor)
        {
            GUILayout.Space(20);

            GUILayout.BeginVertical(GUILayout.Width(470));

            tooltipsScrollPos = GUILayout.BeginScrollView(tooltipsScrollPos, false, false);

            GUILayout.BeginHorizontal(GUILayout.Width(460));

            EditorGUILayout.LabelField($"Active Objects : {_editor.script.tooltips.Count}",
                new GUIStyle(GUI.skin.textField));

            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
            {
                _editor.script.tooltips.Add(new Tooltip());
            }

            GUILayout.EndHorizontal();

            for (int i = 0; i < _editor.script.tooltips.Count; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(460));

                _editor.script.tooltips[i].tooltip =
                    EditorGUILayout.TextField(_editor.script.tooltips[i].tooltip);
                _editor.script.tooltips[i].icon =
                    (Texture)EditorGUILayout.ObjectField(_editor.script.tooltips[i].icon, typeof(Texture),
                        true);

                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    if (_editor.script.tooltips.Contains(_editor.script.tooltips[i]))
                        _editor.script.tooltips.Remove(_editor.script.tooltips[i]);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void RenderGameObjects(LabelEditor _editor)
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal(GUILayout.Width(460));

            EditorGUILayout.LabelField($"Active Objects : {_editor.script.gameObjects.Count}",
                new GUIStyle(GUI.skin.textField));

            if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
            {
                _editor.script.gameObjects.Add(new ObjectDictionary());
            }

            GUILayout.EndHorizontal();

            gameObjectsScrollPos = GUILayout.BeginScrollView(gameObjectsScrollPos, false, false);

            for (int i = 0; i < _editor.script.gameObjects.Count; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(460));

                _editor.script.gameObjects[i].GameObject =
                    (GameObject)EditorGUILayout.ObjectField(_editor.script.gameObjects[i].GameObject,
                        typeof(GameObject), true);

                foreach (var preset in LabelManager.Labels)
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

            if (LabelManager.Labels.Count == 0)
            {
                GUILayout.Label("No Presets were found!");
                GUILayout.Label("Click Add Label to create one...");
            }
            else
            {
                for (int i = 0; i < LabelManager.Labels.Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(220), GUILayout.Height(22));

                    GUILayout.FlexibleSpace();
                    GUI.color = LabelManager.Labels[i].textColor;

                    if (GUILayout.Button(LabelManager.Labels[i].name.Split("_")[1], GUILayout.Width(180)))
                    {
                        _activeLabel = LabelManager.Labels[i];
                    }

                    var icon = LabelManager.Labels[i].isActive
                        ? EditorGUIUtility.IconContent("d_scenevis_visible_hover@2x")
                        : EditorGUIUtility.IconContent("d_scenevis_hidden_hover@2x");

                    LabelManager.Labels[i].isActive = GUILayout.Toggle(LabelManager.Labels[i].isActive, "",
                        new GUIStyle()
                        {
                            normal = new GUIStyleState() { background = icon.image as Texture2D }
                        }, GUILayout.Width(16), GUILayout.Height(16));

                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        DeleteLabel(LabelManager.Labels[i]);
                    }

                    GUI.color = guiColor;

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }

        #endregion

        /// <summary>
        /// Create and store in the default folder a new label with a given name.
        /// </summary>
        /// <param name="_name"></param>
        private void AddNewLabel(string _name)
        {
            var label = CreateInstance<Label>();
            string labelPath = String.Concat(LabelManager.LabelsDirectory, $"/LabelPreset_{_name}.asset");

            if (!File.Exists(labelPath))
            {
                AssetDatabase.CreateAsset(label, labelPath);

                label.textColor = Color.white;
                label.backgroundColor = label.textColor;

                label.tooltips = new List<Tooltip>();
                label.gameObjects = new List<ObjectDictionary>();

                LabelManager.AddPreset(label);
                EditorApplication.RepaintHierarchyWindow();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                _activeLabel = label;
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
        private void DeleteLabel(Label _label)
        {
            LabelManager.RemovePreset(_label);

            string assetPath = String.Concat(LabelManager.LabelsDirectory, $"/{_label.name}.asset");

            if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                _activeLabel = LabelManager.Labels.Count > 0 ? LabelManager.Labels[0] : null;
                LabelManager.Labels.Remove(_label);
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", $"Could not find asset at path: {assetPath}", "OK");
            }
        }
    }
#endif
}
