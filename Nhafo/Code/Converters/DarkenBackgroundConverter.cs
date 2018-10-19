using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Nhafo.Code.Converters {
    public class DarkenBackgroundConverter : IValueConverter {
        public const double DARKEN_FACTOR = .7;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Console.WriteLine(value);
            Console.WriteLine(targetType);
            if(value is SolidColorBrush brush)
                return new SolidColorBrush(Darken(brush.Color));
            else if(value is Color color)
                return new SolidColorBrush(Darken(color));
            else if(value is string str)
                return new SolidColorBrush(Darken((Color)ColorConverter.ConvertFromString(str)));
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        private static Color Darken(Color color) =>
            new Color() {
                R = (byte)(color.R * DARKEN_FACTOR),
                G = (byte)(color.G * DARKEN_FACTOR),
                B = (byte)(color.B * DARKEN_FACTOR),
                A = color.A
            };
    }
}
