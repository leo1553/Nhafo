using System;
using System.Windows;
using System.Windows.Controls;

namespace Nhafo.WPF.Controls {
    /// <summary>
    /// Interação lógica para TextBoxPlaceholder.xam
    /// </summary>
    public partial class TextBoxPlaceholder : UserControl {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBoxPlaceholder),
            new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register("TextBox", typeof(TextBox), typeof(TextBoxPlaceholder),
            new PropertyMetadata(null));
        public static readonly DependencyProperty HideOnFocusProperty = DependencyProperty.Register("HideOnFocus", typeof(bool), typeof(TextBoxPlaceholder),
            new PropertyMetadata(true));

        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextBox TextBox {
            get => (TextBox)GetValue(TextBoxProperty);
            set {
                TextBoxChanged(TextBox, value);
                SetValue(TextBoxProperty, value);
            }
        }

        public bool HideOnFocus {
            get => (bool)GetValue(HideOnFocusProperty);
            set => SetValue(HideOnFocusProperty, value);
        }

        public TextBoxPlaceholder() {
            InitializeComponent();
        }

        private void TextBoxChanged(TextBox oldOne, TextBox newOne) {
            if(oldOne != null) {
                newOne.TextChanged -= AttachedTextBoxTextChanged;
                newOne.GotFocus -= AttachedTextBoxGotFocus;
                newOne.LostFocus -= AttachedTextBoxLostFocus;
            }

            if(newOne != null) {
                newOne.TextChanged += AttachedTextBoxTextChanged;
                newOne.GotFocus += AttachedTextBoxGotFocus;
                newOne.LostFocus += AttachedTextBoxLostFocus;
                AttachedTextBoxTextChanged(newOne, null);
            }
        }

        private void AttachedTextBoxTextChanged(object sender, TextChangedEventArgs e) {
            if(sender is TextBox textBox && textBox == TextBox) {
                if(textBox.IsFocused && HideOnFocus) {
                    Visibility = Visibility.Hidden;
                }
                else {
                    Visibility = TextBox.Text.Length == 0 ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private void AttachedTextBoxGotFocus(object sender, RoutedEventArgs e) {
            if(sender is TextBox textBox && textBox == TextBox) {
                if(!HideOnFocus)
                    return;

                Visibility = Visibility.Hidden;
            }
        }

        private void AttachedTextBoxLostFocus(object sender, RoutedEventArgs e) {
            if(sender is TextBox textBox && textBox == TextBox) {
                if(!HideOnFocus)
                    return;

                Visibility = TextBox.Text.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}
