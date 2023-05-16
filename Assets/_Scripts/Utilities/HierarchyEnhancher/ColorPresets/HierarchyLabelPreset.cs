﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HierarchyLabelPreset : ScriptableObject
{
    [HideInInspector] public string identifier;

    [HideInInspector] public Texture icon;
    [HideInInspector] public FontStyle fontStyle;
    [HideInInspector] public TextAnchor alignment = TextAnchor.MiddleLeft;

    [HideInInspector] public Color textColor;
    [HideInInspector] public Color inactiveTextColor;

    [HideInInspector] public bool useCustomInactiveColors;
    [HideInInspector] public Color backgroundColor = new (0.2196079f, 0.2196079f, 0.2196079f, 1);
    [HideInInspector] public Color inactiveBackgroundColor = new (0.2196079f, 0.2196079f, 0.2196079f, 1);
    [HideInInspector] public string tooltip;
    [HideInInspector] public string info;

    [NonReorderable] public List<ImageTooltip> tooltips;
}

[System.Serializable]
public class ImageTooltip
{
    public string tooltip;
    public Texture icon;
    [HideInInspector] public string info;
}