using System.Windows;

namespace Nhafo.WPF.Dialogs {
    /// <summary>
    /// Lógica interna para PesoDialog.xaml
    /// </summary>
    public partial class PesoDialog : Window {
        public float Result { get; private set; } = float.NaN;

        public PesoDialog(Window owner) {
            InitializeComponent();
            button.Click += _Click;
            Loaded += (s, a) => textBox.Focus();

            Owner = owner;
        }

        private void _Click(object sender, RoutedEventArgs e) {
            if(!float.TryParse(textBox.Text, out float result)) {
                MessageBox.Show("Valor inválido no campo 'Peso'.", "Opa!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Result = result;
            DialogResult = true;
        }
    }
}
