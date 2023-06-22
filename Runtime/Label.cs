using System.Collections.Generic;
using UnityEngine;

namespace HierarchyEnhancer
{
    public class Label : ScriptableObject
    {
        [HideInInspector] public Texture icon;
        [HideInInspector] public FontStyle fontStyle;
        [HideInInspector] public TextAnchor alignment = TextAnchor.MiddleLeft;

        [HideInInspector] public Color textColor;

        [HideInInspector] public bool useCustomBackground;
        [HideInInspector] public Color backgroundColor;

        [NonReorderable] public List<Tooltip> tooltips;
        [NonReorderable] public List<ObjectDictionary> gameObjects;

        public bool isActive = true;
    }
}