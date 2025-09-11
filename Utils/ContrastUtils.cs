using System;
using System.Globalization;

namespace Texta.Utils
{
    public static class ContrastUtils
    {
        public static double? CalculateContrastRatio(string hexColor1, string hexColor2)
        {
            if (!TryHexToRgb(hexColor1, out var c1) || !TryHexToRgb(hexColor2, out var c2))
                return null;

            var l1 = RelativeLuminance(c1.r, c1.g, c1.b);
            var l2 = RelativeLuminance(c2.r, c2.g, c2.b);

            var lighter = Math.Max(l1, l2);
            var darker = Math.Min(l1, l2);
            var ratio = (lighter + 0.05) / (darker + 0.05);
            return ratio;
        }

        public static bool PassesAA(double ratio, bool isLargeText) => isLargeText ? ratio >= 3.0 : ratio >= 4.5;
        public static bool PassesAAA(double ratio, bool isLargeText) => isLargeText ? ratio >= 4.5 : ratio >= 7.0;
        

        private static bool TryHexToRgb(string hex, out (int r, int g, int b) rgb)
        {
            rgb = (0, 0, 0);
            if  (string.IsNullOrWhiteSpace(hex)) return false;
            hex = hex.TrimStart('#');

            if (hex.Length == 3)
            {
                try
                {
                    var r = Convert.ToInt32(new string(hex[0], 2), 16);
                    var g = Convert.ToInt32(new string(hex[1], 2), 16);
                    var b = Convert.ToInt32(new string(hex[2], 2), 16);
                    rgb = (r, g, b);
                    return true;
                }
                catch { return false; }
            }

            if (hex.Length == 6)
            {
                try
                {
                    var r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    var g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    var b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                    rgb = (r, g, b);
                    return true;
                }
                catch { return false; }
            }
            return false;
        }

        private static double RelativeLuminance(int rByte, int gByte, int bByte)
        {
            double RsRgb = rByte / 255.0;
            double GsRgb = gByte / 255.0;
            double BsRgb = bByte / 255.0;

            double R = RsRgb <= 0.03928 ? RsRgb / 12.92 : Math.Pow((RsRgb + 0.055) / 1.055, 2.4);
            double G = GsRgb <= 0.03928 ? GsRgb / 12.92 : Math.Pow((GsRgb + 0.055) / 1.055, 2.4);
            double B = BsRgb <= 0.03928 ? BsRgb / 12.92 : Math.Pow((BsRgb + 0.055) / 1.055, 2.4);

            return 0.2126 * R + 0.7152 * G + 0.0722 * B;
        }
    }
}