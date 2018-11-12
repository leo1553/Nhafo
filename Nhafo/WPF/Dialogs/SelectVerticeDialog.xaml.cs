using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using Nhafo.WPF.Controls;
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;

namespace Nhafo.WPF.Dialogs {
    /// <summary>
    /// Interação lógica para SelectVerticeDialog.xam
    /// </summary>
    public partial class SelectVerticeDialog : CustomDialog {
        private static readonly VerticeControl InvalidVerticeControl = new VerticeControl() { Key = '\0' };
        private static readonly SelectVerticeDialog Instance = new SelectVerticeDialog();

        private ObservableCollection<VerticeComboBoxItem> ComboBoxItems { get; set; } = new ObservableCollection<VerticeComboBoxItem>();
        private TaskCompletionSource<VerticeControl> tcs = null;

        private readonly MetroWindow mainWindow;

        public SelectVerticeDialog() {
            InitializeComponent();

            mainWindow = Application.Current.MainWindow as MetroWindow;

            comboBox.ItemsSource = ComboBoxItems;
            comboBox.SelectionChanged += VerticesComboBoxSelectionChanged;
        }

        private void VerticesComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            okButton.IsEnabled = comboBox.SelectedIndex > 0;
        }

        private async void OKButtonClick(object sender, RoutedEventArgs e) {
            await mainWindow.HideMetroDialogAsync(this);

            if(tcs != null)
                tcs.TrySetResult((comboBox.SelectedItem as VerticeComboBoxItem).VerticeControl);
            tcs = null;
        }

        private async void CancelButtonClick(object sender, RoutedEventArgs e) {
            await mainWindow.HideMetroDialogAsync(this);

            if(tcs != null)
                tcs.TrySetResult(null);
            tcs = null;
        }

        public static async Task<VerticeControl> Show(GrafoControl grafo, string title = null) {
            Instance.Title = title ?? string.Empty;
            Instance.ComboBoxItems.Clear();
            Instance.ComboBoxItems.Add(new VerticeComboBoxItem(InvalidVerticeControl));
            foreach(VerticeControl vertice in grafo.Vertices)
                Instance.ComboBoxItems.Add(new VerticeComboBoxItem(vertice));
            Instance.comboBox.SelectedIndex = 0;

            await Instance.mainWindow.ShowMetroDialogAsync(Instance);
            Instance.tcs = new TaskCompletionSource<VerticeControl>();
            return await Instance.tcs.Task;
        }

        private class VerticeComboBoxItem {
            public VerticeControl VerticeControl { get; private set; }

            public VerticeComboBoxItem(VerticeControl control) {
                VerticeControl = control;
            }

            public override string ToString() {
                if(VerticeControl.Key == '\0')
                    return "Selecione um Vértice";
                return VerticeControl.Key.ToString();
            }
        }
    }
}
