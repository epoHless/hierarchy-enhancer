using System;
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
        private Vector2 infoScrollPos = Vector2.zero;

        private GUIStyle labelStyle;

        private int labelTab = 0;
        private int optionsTab = 0;

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
            minSize = new Vector2(740, 400);
            maxSize = minSize;

            if (LabelManager.Labels.Count > 0) _activeLabel = LabelManager.Labels[0];
        }

        private void OnGUI()
        {
            optionsTab = GUILayout.Toolbar(optionsTab, new[] { "Labels", "Options" }, GUILayout.Width(225));

            switch (optionsTab) //toggle panels
            {
                case 0: //labels panel
                {
                    RenderLabelPanel();
                    break;
                }

                case 1: //options panel
                {
                    if (RenderSettingsPanel()) return;
                    break;
                }
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        #region UI Render Methods

        private void RenderEditor(LabelEditor _editor)
        {
            GUILayout.BeginVertical(GUILayout.Width(463));

            GUILayout.Space(10);
            _editor!.RenderIcon();
            GUILayout.Space(10);
            _editor!.RenderFont();
            GUILayout.Space(10);
            _editor!.RenderColors();

            GUILayout.EndVertical();
        }

        private void RenderGameObjects(LabelEditor _editor)
        {
            var color = GUI.color;

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(470), GUILayout.Height(20)))
            {
                labelStyle = new GUIStyle(GUI.skin.label) 
                { 
                    fontStyle = FontStyle.Bold
                };
            
                EditorGUILayout.LabelField($"Active Objects : {_editor.script.gameObjects.Count}",
                    labelStyle);

                GUI.color = Color.green;
                
                if (GUILayout.Button("Add GameObject", GUILayout.Width(110), GUILayout.Height(20)))
                {
                    _editor.script.gameObjects.Add(new ObjectDictionary());
                }

                GUI.color = color;
            }
            
            GUILayout.Space(10);

            using (var scrollView = new EditorGUILayout.ScrollViewScope(infoScrollPos, false, false))
            {
                infoScrollPos = scrollView.scrollPosition;
                
                for (int i = 0; i < _editor.script.gameObjects.Count; i++)
                {
                    var gameObjectInfo = _editor.script.gameObjects[i];

                    using (new EditorGUILayout.VerticalScope(GUI.skin.window, GUILayout.Width(460)))
                    {
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(460)))
                        {
                            gameObjectInfo.GameObject =
                                (GameObject)EditorGUILayout.ObjectField(gameObjectInfo.GameObject,
                                    typeof(GameObject), true);

                            foreach (var label in LabelManager.Labels)
                            {
                                var gameObjects = label.gameObjects.Where(_o => _o != null);

                                if (_editor.script != label && gameObjects.Contains(gameObjectInfo))
                                {
                                    gameObjectInfo = null;
                                }
                            }

                            GUI.color = Color.red;
                    
                            if (GUILayout.Button("Remove", GUILayout.Width(55),GUILayout.Height(20)))
                            {
                                if (_editor.script.gameObjects.Contains(_editor.script.gameObjects[i]))
                                {
                                    _editor.script.gameObjects.Remove(_editor.script.gameObjects[i]);
                                    return;
                                }
                            }

                            GUI.color = color;
                        }

                        GUI.color = Color.green;
                    
                        if (GUILayout.Button("Add Tooltip", GUILayout.Width(75)))
                        {
                            gameObjectInfo.tooltips.Add(new Tooltip());
                        }

                        GUI.color = color;
                    
                        if(gameObjectInfo != null)
                            RenderTooltips(gameObjectInfo);
                    }
                
                    GUILayout.Space(10);
                }
            }
        }

        private void RenderTooltips(ObjectDictionary _gameObjectInfo)
        {
            Color color = GUI.color;
            
            for (int j = 0; j < _gameObjectInfo.tooltips.Count; j++)
            {
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    _gameObjectInfo.tooltips[j].icon =
                        EditorGUILayout.ObjectField(_gameObjectInfo.tooltips[j].icon, typeof(Texture2D), false) as
                            Texture2D;

                    _gameObjectInfo.tooltips[j].tooltip =
                        EditorGUILayout.TextField(_gameObjectInfo.tooltips[j].tooltip);

                    GUI.color = Color.red;

                    if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        _gameObjectInfo.tooltips.Remove(_gameObjectInfo.tooltips[j]);
                    }

                    GUI.color = color;
                }
            }
        }

        /// <summary>
        /// Creates all the label buttons
        /// </summary>
        private void RenderLabels()
        {
            labelsScrollPos = GUILayout.BeginScrollView(labelsScrollPos, false, false);

            var guiColor = GUI.color;

            if (LabelManager.Labels.Count == 0)
            {
                LabelManager.FetchLabels();
                
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

                    if (GUILayout.Button(LabelManager.Labels[i].name.Split("_")[1], GUILayout.Width(180), GUILayout.Height(20)))
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
                        }, GUILayout.Width(20), GUILayout.Height(20));

                    GUI.color = Color.red;

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        LabelManager.DeleteLabel(LabelManager.Labels[i]);
                    }

                    GUI.color = guiColor;

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
        }
        
        private bool RenderSettingsPanel()
        {
            minSize = new Vector2(230, 400);
            maxSize = minSize;

            GUILayout.Space(10);

            LabelManager.ShowFocusButton =
                GUILayout.Toggle(LabelManager.ShowFocusButton, " Show GameObject Focus Button");
            LabelManager.ShowToggleButton =
                GUILayout.Toggle(LabelManager.ShowToggleButton, " Show GameObject Toggle Button");
            LabelManager.ShowHierarchyLines =
                GUILayout.Toggle(LabelManager.ShowHierarchyLines, " Show Hierarchy Parent Lines");
            LabelManager.ShowLabels =
                GUILayout.Toggle(LabelManager.ShowLabels, " Show Hierarchy Labels");
            LabelManager.ShowComponents =
                GUILayout.Toggle(LabelManager.ShowComponents, " Show Hierarchy Components");
            LabelManager.ShowIcons =
                GUILayout.Toggle(LabelManager.ShowIcons, " Show Hierarchy Icons");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Change Default Directory", GUILayout.Width(220)))
            {
                var directory = EditorUtility.OpenFolderPanel("Change Directory", "", "");

                if (string.IsNullOrEmpty(directory)) return true;

                directory = directory.Substring(directory.IndexOf("Assets"));
                PlayerPrefs.SetString("LabelDirectory", directory);

                LabelManager.FetchLabels();
                InitUI();
            }

            return false;
        }

        private void RenderLabelPanel()
        {
            minSize = new Vector2(740, 400);
            maxSize = minSize;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    RenderLabels();

                    GUILayout.FlexibleSpace();

                    DrawSeparatorLine(new Vector2(230, 0), new Vector2(3, 410),new Color(0.16f, 0.16f, 0.16f, 1f));

                    if (GUILayout.Button("Fetch Labels", GUILayout.Width(223)))
                    {
                        LabelManager.FetchLabels();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        labelName = GUILayout.TextField(labelName, GUILayout.Width(200));

                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            if (labelName != String.Empty) LabelManager.CreateLabel(labelName);
                        }
                    }
                }

                if (_activeLabel) //Create the editor for the selected Preset
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        var editor = UnityEditor.Editor.CreateEditor(_activeLabel) as LabelEditor;

                        GUILayout.Space(-20);

                        DrawSeparatorLine(new Vector2(230, 0), 
                            new Vector2(3, 410), 
                            new Color(0.16f, 0.16f, 0.16f, 1f));

                        RenderLabel(editor);
                    }
                }
            }
        }

        private void RenderLabel(LabelEditor _editor)
        {
            labelTab = GUILayout.Toolbar(labelTab, new[] { "Style", "GameObjects" });

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
                    RenderGameObjects(_editor);
                    break;
                }
            }
        }

        #endregion

        #region Utilities

        private void DrawSeparatorLine(Vector2 _position, Vector2 _size, Color _color)
        {
            GUI.DrawTexture(new Rect(_position, _size),
                Utilities.CreateColoredTexture(_color));
        }

        #endregion
        
        
    }
#endif
}
