using UnityEngine;

[System.Serializable]
public class ObjectIDDictionary
{
    private GameObject _gameObject;

    public GameObject GameObject
    {
        get => _gameObject;
        set
        {
            _gameObject = value;
            if(_gameObject != null) ID = _gameObject.GetInstanceID();
        }
    }
    
    public int ID;

    public bool Contains(GameObject _gameObject)
    {
        return GameObject == _gameObject;
    }
}
