using UnityEngine;

namespace HierarchyEnhancer.Runtime
{
    [System.Serializable]
    public class Tooltip
    {
        public string tooltip;
        public Texture icon;
        [HideInInspector] public string info;
    }
}