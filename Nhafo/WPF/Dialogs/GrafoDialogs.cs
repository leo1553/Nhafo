using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Nhafo.WPF.Controls;
using System.Threading.Tasks;

namespace Nhafo.WPF.Dialogs {
    public static class GrafoDialogs {
        public static async Task<string> ShowRenameDialog(GrafoControl grafo, bool error = false) {
            MetroWindow mainWindow = App.Current.MainWindow as MetroWindow;

            MetroDialogSettings settings = new MetroDialogSettings() {
                NegativeButtonText = "Cancelar"
            };

            string result = await mainWindow.ShowInputAsync("Renomear", $"Insira o novo nome para o grafo { grafo.Key }:", settings);
            return result == null || result.Length == 0 ? null : result;
        }
    }
}
