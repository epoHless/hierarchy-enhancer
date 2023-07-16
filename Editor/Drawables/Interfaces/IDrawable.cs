using UnityEngine;

public interface IDrawable
{
    public bool IsEnabled { get; set; }
    public void OnGUI(int _instanceID, Rect _selectionRect, GameObject _gameObject);
}
