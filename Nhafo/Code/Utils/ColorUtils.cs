using System.Windows;
using System.Windows.Media;

namespace Nhafo.Code.Utils {
    public static class ColorUtils {
        public static readonly SolidColorBrush BaseColor = new SolidColorBrush((Color)Application.Current.Resources["BaseColor"]);
        public static readonly SolidColorBrush AccentColor = new SolidColorBrush((Color)Application.Current.Resources["AccentBaseColor"]);

        public static float GetBrightness(this Color color) {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            float max, min;

            max = r; min = r;

            if(g > max) max = g;
            if(b > max) max = b;

            if(g < min) min = g;
            if(b < min) min = b;

            return (max + min) / 2;
        }

        public static float GetSaturation(this Color color) {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;

            float max, min;
            float l, s = 0;

            max = r; min = r;

            if(g > max) max = g;
            if(b > max) max = b;

            if(g < min) min = g;
            if(b < min) min = b;

            // if max == min, then there is no color and
            // the saturation is zero.
            //
            if(max != min) {
                l = (max + min) / 2;

                if(l <= .5) {
                    s = (max - min) / (max + min);
                }
                else {
                    s = (max - min) / (2 - max - min);
                }
            }
            return s;
        }

        public static float GetLuminance(this Color color) {
            return (0.299F * color.R + 0.587F * color.G + 0.114F * color.B) / 255;
        }

        public static Color Inverse(this Color color) {
            return new Color() {
                R = (byte)(255 - color.R),
                G = (byte)(255 - color.G),
                B = (byte)(255 - color.B),
                A = color.A
            };
        }
    }
}