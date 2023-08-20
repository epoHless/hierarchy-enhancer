using UnityEditor;
using UnityEngine;

namespace HierarchyEnhancer.Runtime
{
    public static class Utilities
    {
        public static Color SelectedColor = new Color(0.17f, 0.36f, 0.53f, 1f);
        public static Color UnselectedColor => GetGUIColor();
        public static Color HoveredColor => GetHoveredGUIColor();
        
        private static Color GetHoveredGUIColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.27f, 0.27f, 0.27f, 1f) : new (0.7f, 0.7f, 0.7f, 1f);
        }
        
        public static Color GetGUIColor()
        {
            return EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.78f, 0.78f, 0.78f, 1f);
        }
        
        public static Texture2D CreateColoredTexture(Color _col)
        {
            Color[] pix = new Color[1 * 1];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = _col;

            Texture2D result = new Texture2D(1, 1);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
        
        public static Color ChangeColorBrightness(Color _color, float _correctionFactor)
        {
            return new Color(_color.r * _correctionFactor, _color.g * _correctionFactor, _color.b * _correctionFactor,
                1f);
        }

        public static Texture2D CreateGradientTexture(int _width, int _height, Color _leftColor, Color _rightColor)
        {
            Texture2D texture2D = new Texture2D(_width, _height, TextureFormat.ARGB32, false)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            Color[] array = new Color[_width * _height];

            for (int i = 0; i < _width; i++)
            {
                Color color = Color.Lerp(_leftColor, _rightColor, (float)i / (_width - 1));
                
                for (int j = 0; j < _height; j++)
                {
                    array[j * _width + i] = color;
                }
            }

            texture2D.SetPixels(array);
            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.Apply();
            
            return texture2D;
        }
    }
}
