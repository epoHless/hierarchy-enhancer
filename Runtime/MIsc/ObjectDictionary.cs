using UnityEngine;

namespace HierarchyEnhancer
{
    [System.Serializable]

    public class ObjectDictionary
    {
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

        public bool Contains(GameObject _gameObject)
        {
            return GameObject == _gameObject;
        }
    }
}
