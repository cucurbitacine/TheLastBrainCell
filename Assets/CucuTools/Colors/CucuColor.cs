using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace CucuTools.Colors
{
    /// <summary>
    /// Basic logic for working with color
    /// </summary>
    public static class CucuColor
    {
        public static readonly Color Color00 = new Color(0.45f, 0.35f, 0.65f);
        public static readonly Color Color01 = new Color(0.00f, 0.70f, 0.70f);
        public static readonly Color Color11 = new Color(1.00f, 0.90f, 0.15f);
        public static readonly Color Color10 = new Color(0.95f, 0.35f, 0.25f);
        
        public static Color Empty => (empty ?? (empty = Alpha(Color.black, 0f))).Value;
        private static Color? empty;

        public static Color Scale(Color color, float scale)
        {
            return new Color(color.r * scale, color.g * scale, color.b * scale, color.a);
        }

        public static Color Alpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static string Color2Hex(Color color)
        {
            var result = "";
            for (var i = 0; i < 4; i++)
            {
                var val = (int) Mathf.Clamp(color[i] * 255, 0, 255);
                var hex = val.ToString("X");
                if (hex.Length < 2) hex = "0" + hex;
                result += hex;
            }

            return result;
        }

        public static bool TryGetColorFromHex(string hex, out Color color)
        {
            color = Scale(Color.black, 0f);

            if (string.IsNullOrWhiteSpace(hex) || (hex.Length != 6 && hex.Length != 8))
            {
                return false;
            }

            var intR = 0;
            var intG = 0;
            var intB = 0;
            var intA = 0;

            try
            {
                intR = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                intG = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                intB = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                intA = hex.Length == 8
                    ? int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber)
                    : 255;
            }
            catch
            {
                return false;
            }

            color = new Color(intR / 255f, intG / 255f, intB / 255f, intA / 255f);
            return true;
        }

        public static Color Hex2Color(string hex)
        {
            TryGetColorFromHex(hex, out var color);
            return color;
        }

        #region Lerp & Blend

        /// <summary>
        /// Lerping color.
        /// Wrapper around Lerp function of <see cref="Color"/>
        /// </summary>
        /// <param name="origin">From</param>
        /// <param name="target">To</param>
        /// <param name="value">Lerp value</param>
        /// <returns></returns>
        public static Color Lerp(Color origin, Color target, float value)
        {
            return Color.Lerp(origin, target, value);
        }

        /// <summary>
        /// Blending color in queue of colors. Lerping only between two neighbor colors
        /// </summary>
        /// <param name="value">Blend value</param>
        /// <param name="colors">Colors</param>
        /// <returns>Color</returns>
        public static Color Blend(float value, params Color[] colors)
        {
            if (colors == null || colors.Length == 0) return Alpha(Color.black, 0f);
            if (colors.Length == 1) return colors.First();

            value = Mathf.Clamp01(value);

            var dt = 1f / (colors.Length - 1);

            for (var i = 0; i < colors.Length - 1; i++)
            {
                var t = dt * i;
                if (t <= value && value <= t + dt)
                    return colors[i].LerpTo(colors[i + 1], Mathf.Clamp01((value - t) / dt));
            }

            return colors.Last();
        }

        #endregion

        #region Gradients

        public static Gradient Colors2Gradient(params Color[] colors)
        {
            if (colors.Length > 8)
            {
                var linSpace = Cucu.LinSpace(8);
                colors = linSpace.Select(t => CucuColor.Blend(t, colors)).ToArray();
            }

            var times = Cucu.LinSpace(colors.Length);

            return new Gradient
            {
                mode = GradientMode.Blend,
                colorKeys = times.Select((t, i) => new GradientColorKey(colors[i], t)).ToArray(),
                alphaKeys = times.Select((t, i) => new GradientAlphaKey(colors[i].a, t)).ToArray()
            };
        }

        public static Color[] Gradient2Colors(Gradient gradient)
        {
            return gradient.colorKeys
                .Select((c, i) => new Color(c.color.r, c.color.g, c.color.b, gradient.alphaKeys[i].alpha))
                .ToArray();
        }

        /// <summary>
        /// Map of palettes
        /// </summary>
        public static Dictionary<CucuColorMap, Gradient> GradientSample => grdSmpl ?? (grdSmpl =
            new Dictionary<CucuColorMap, Gradient>
            {
                {CucuColorMap.Rainbow, Rainbow},
                {CucuColorMap.Jet, Jet},
                {CucuColorMap.Hot, Hot},
                {CucuColorMap.BlackToWhite, BlackToWhite},
                {CucuColorMap.WhiteToBlack, WhiteToBlack}
            });
        private static Dictionary<CucuColorMap, Gradient> grdSmpl;

        /// <summary>
        /// Rainbow palette
        /// </summary>
        public static Gradient Rainbow => CucuColor.Colors2Gradient(
            Color.red,
            Color.red.LerpTo(Color.yellow),
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.blue,
            "CC00FF".ToColor()
        );

        /// <summary>
        /// Jet palette
        /// </summary>
        public static Gradient Jet => CucuColor.Colors2Gradient(
            new Color(0.000f, 0.000f, 0.666f, 1.000f),
            new Color(0.000f, 0.000f, 1.000f, 1.000f),
            new Color(0.000f, 0.333f, 1.000f, 1.000f),
            new Color(0.000f, 0.666f, 1.000f, 1.000f),
            new Color(0.000f, 1.000f, 1.000f, 1.000f),
            new Color(0.500f, 1.000f, 0.500f, 1.000f),
            new Color(1.000f, 1.000f, 0.000f, 1.000f),
            new Color(1.000f, 0.666f, 0.000f, 1.000f),
            new Color(1.000f, 0.333f, 0.000f, 1.000f),
            new Color(1.000f, 0.000f, 0.000f, 1.000f),
            new Color(0.666f, 0.000f, 0.000f, 1.000f)
        );

        /// <summary>
        /// Hot palette
        /// </summary>
        public static Gradient Hot => CucuColor.Colors2Gradient(Color.black, Color.red, Color.yellow, Color.white);

        /// <summary>
        /// Black to white palette
        /// </summary>
        public static Gradient BlackToWhite => CucuColor.Colors2Gradient(Color.black, Color.white);


        /// <summary>
        /// White to black palette
        /// </summary>
        public static Gradient WhiteToBlack => CucuColor.Colors2Gradient(Color.white, Color.black);

        #endregion
        
        public static Color ColorUV(Vector2 uv, Color color00, Color color10, Color color11, Color color01)
        {
            return Color.Lerp(
                Color.Lerp(
                    Color.Lerp(color00, color10, uv.x), Color.Lerp(color01, color11, uv.x), uv.y),
                Color.Lerp(
                    Color.Lerp(color00, color01, uv.y), Color.Lerp(color10, color11, uv.y), uv.x),
                0.5f);
        }

        public static Color ColorUV(Vector2 uv)
        {
            return ColorUV(uv, Color00, Color10, Color11, Color01);
        }
    }

    /// <summary>
    /// Color extentions
    /// </summary>
    public static class CucuColorExtentions
    {
        /// <summary>
        /// Get string color like FFFFFF
        /// Like 
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Hex string</returns>
        public static string ToHex(this Color color)
        {
            return CucuColor.Color2Hex(color);
        }

        /// <summary>
        /// Get color from hex string
        /// </summary>
        /// <param name="hex">String hex color</param>
        /// <returns>Color</returns>
        public static Color ToColor(this string hex)
        {
            return CucuColor.Hex2Color(hex);
        }

        /// <summary>
        /// Lerp color to <param name="target"></param>
        /// </summary>
        /// <param name="color">Origin</param>
        /// <param name="target">Target</param>
        /// <param name="t">Lerp value</param>
        /// <returns>Color</returns>
        public static Color LerpTo(this Color color, Color target, float t = 0.5f)
        {
            return CucuColor.Lerp(color, target, t);
        }

        /// <summary>
        /// Lerping color in queue colors
        /// </summary>
        /// <param name="value">Lerp value</param>
        /// <param name="colors">Colors</param>
        /// <returns>Color</returns>
        public static Color BlendColor(this float value, params Color[] colors)
        {
            return CucuColor.Blend(value, colors);
        }

        /// <summary>
        /// Blending color in queue colors
        /// </summary>
        /// <param name="colors">Colors</param>
        /// <param name="value">Blend value</param>
        /// <returns>Color</returns>
        public static Color BlendColor(this IEnumerable<Color> colors, float value)
        {
            return value.BlendColor(colors.ToArray());
        }

        /// <summary>
        /// Set color alpha
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="value">Alpha</param>
        /// <returns>Color</returns>
        public static Color AlphaTo(this Color color, float value)
        {
            return CucuColor.Alpha(color, value);
        }

        /// <summary>
        /// Set color intensity
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="value">Intensity</param>
        /// <returns>Color</returns>
        public static Color ScaleTo(this Color color, float value)
        {
            return CucuColor.Scale(color, value);
        }

        /// <summary>
        /// Convert vector3 to color
        /// </summary>
        /// <param name="vector3">Vector3</param>
        /// <param name="alpha">Alpha value</param>
        /// <returns>Color</returns>
        public static Color ToColor(this Vector3 vector3, float alpha = 1f)
        {
            return new Color(vector3.x, vector3.y, vector3.z, alpha);
        }

        /// <summary>
        /// Convert vector4 to color
        /// </summary>
        /// <param name="vector4">Vector4</param>
        /// <returns>Color</returns>
        public static Color ToColor(this Vector4 vector4)
        {
            return new Color(vector4.x, vector4.y, vector4.z, vector4.w);
        }

        /// <summary>
        /// Convert color to vectro3
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        /// <summary>
        /// Convert color to vectro4
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Vector4</returns>
        public static Vector4 ToVector4(this Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        public static Gradient ToGradient(this IEnumerable<Color> colors)
        {
            return CucuColor.Colors2Gradient(colors.ToArray());
        }

        public static Color[] ToColors(this Gradient gradient)
        {
            return CucuColor.Gradient2Colors(gradient);
        }
        
        public static Color ColorUV(this Vector2 uv, Color color00, Color color10, Color color11, Color color01)
        {
            return CucuColor.ColorUV(uv, color00, color10, color11, color01);
        }

        public static Color ColorUV(this Vector2 uv)
        {
            return ColorUV(uv, CucuColor.Color00, CucuColor.Color10, CucuColor.Color11, CucuColor.Color01);
        }
    }

    /// <summary>
    /// Color map list
    /// </summary>
    public enum CucuColorMap
    {
        Rainbow,
        Jet,
        Hot,
        BlackToWhite,
        WhiteToBlack
    }
}