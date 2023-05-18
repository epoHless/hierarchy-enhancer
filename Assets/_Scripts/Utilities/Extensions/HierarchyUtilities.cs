using UnityEngine;

public static class HierarchyUtilities
{
    /// <summary>
    /// Create a texture with a given color
    /// </summary>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    /// <param name="_col"></param>
    /// <returns></returns>
    public static Texture2D DrawCube(int _width, int _height, Color _col)
    {
        Color[] pix = new Color[_width*_height];
 
        for(int i = 0; i < pix.Length; i++)
            pix[i] = _col;
 
        Texture2D result = new Texture2D(_width, _height);
        result.SetPixels(pix);
        result.Apply();
 
        return result;
    }
    
    /// <summary>
    /// Set the parameter color to a brighter or darker one
    /// </summary>
    /// <param name="_color">the color to modify</param>
    /// <param name="_correctionFactor">| 1 = default | < 1 = darker | > 1 = Lighter |</param>
    /// <returns></returns>
    public static Color ChangeColorBrightness(Color _color, float _correctionFactor)
    {
        return new Color(_color.r * _correctionFactor, _color.g * _correctionFactor, _color.b * _correctionFactor, 1f);
    }
}
