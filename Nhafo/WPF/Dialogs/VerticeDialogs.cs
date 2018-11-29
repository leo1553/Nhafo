using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Nhafo.WPF.Controls;
using System.Threading.Tasks;

namespace Nhafo.WPF.Dialogs {
    public static class VerticeDialogs {
        public static async Task<string> ShowRenameDialog(VerticeControl vertice, bool error = false) {
            MetroWindow mainWindow = App.Current.MainWindow as MetroWindow;

            MetroDialogSettings settings = new MetroDialogSettings() {
                NegativeButtonText = "Cancelar"
            };

            string result = await mainWindow.ShowInputAsync("Renomear", $"Insira o novo nome para o vértice { vertice.Key }:", settings);
            return result == null || result.Length == 0 ? null : result;
        }
    }
}
