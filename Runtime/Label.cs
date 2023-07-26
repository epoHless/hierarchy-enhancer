using System.Collections.Generic;
using UnityEngine;

namespace HierarchyEnhancer.Runtime
{
    public class Label : ScriptableObject
    {
        [HideInInspector] public Texture icon;
        
        [HideInInspector] public FontStyle fontStyle;
        [HideInInspector] public Font font;
        [HideInInspector] public int fontSize;
        [HideInInspector] public TextAnchor alignment = TextAnchor.MiddleLeft;
        [HideInInspector] public Color textColor;

        [HideInInspector] public bool useCustomBackground;
        [HideInInspector] public bool useGradient;
        [HideInInspector] public Color backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);

        [HideInInspector] public bool useIcon = true;
        
        [NonReorderable] public List<ObjectDictionary> gameObjects;

        public bool isActive = true;
    }
}