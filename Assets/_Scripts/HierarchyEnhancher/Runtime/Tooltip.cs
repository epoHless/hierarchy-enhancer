using UnityEngine;

namespace HierarchyEnhancer
{
    [System.Serializable]
    public class Tooltip
    {
        public string tooltip;
        public Texture icon;
        [HideInInspector] public string info;
    }
}