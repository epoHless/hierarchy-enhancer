using System.Collections.Generic;
using UnityEngine;

namespace HierarchyEnhancer.Runtime
{
    [System.Serializable]

    public class ObjectDictionary
    {
        public ObjectDictionary()
        {
            tooltips = new List<Tooltip>();
        }
        
        private GameObject _gameObject;

        public GameObject GameObject
        {
            get => _gameObject;
            set
            {
                _gameObject = value;
                if (_gameObject != null) ID = _gameObject.name;
            }
        }

        public string ID;

        public List<Tooltip> tooltips;
        
        public bool Contains(GameObject _gameObject)
        {
            return GameObject == _gameObject;
        }
    }
}
