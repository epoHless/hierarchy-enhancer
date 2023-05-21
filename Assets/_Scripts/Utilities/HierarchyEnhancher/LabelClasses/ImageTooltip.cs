using UnityEngine;

namespace HierarchyEnhancer
{
    [System.Serializable]

    public class ImageTooltip
    {
        public string tooltip;
        public Texture icon;
        [HideInInspector] public string info;
    }
}