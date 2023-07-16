using UnityEngine;

public interface IRenderer
{
    public bool IsEnabled { get; set; }
    public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject);
}
