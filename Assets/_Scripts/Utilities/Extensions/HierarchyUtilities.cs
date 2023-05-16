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
    
    public static Texture2D DrawCircle(this Texture2D _tex, Color _color, int _x, int y, int _radius = 3)
    {
        float rSquared = _radius * _radius;

        for (int u = _x - _radius; u < _x + _radius + 1; u++)
        for (int v = y - _radius; v < y + _radius + 1; v++)
            if ((_x - u) * (_x - u) + (y - v) * (y - v) < rSquared)
                _tex.SetPixel(u, v, _color);

        return _tex;
    }
    
    /// <summary>
    /// Set the parameter color to a brighter or darker one
    /// </summary>
    /// <param name="color">the color to modify</param>
    /// <param name="correctionFactor">|1 = default | <1 = darker | >1 = Lighter |</param>
    /// <returns></returns>
    public static Color ChangeColorBrightness(Color color, float correctionFactor)
    {
        return new Color(color.r * correctionFactor, color.g * correctionFactor, color.b * correctionFactor, 1f);
    }
}
