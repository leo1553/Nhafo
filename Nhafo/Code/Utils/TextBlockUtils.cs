using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nhafo.Code.Utils {
    public static class TextBlockUtils {
        public static Size MeasureString(string candidate, double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch) {
            FormattedText formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                fontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }
        public static Size MeasureString(this TextBlock textBlock, string candidate) =>
            MeasureString(candidate, textBlock.FontSize, textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
    }
}