using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Nhafo.WPF.Controls;
using System.Globalization;
using System.Threading.Tasks;

namespace Nhafo.WPF.Dialogs {
    public static class ArestaDialogs {
        public static async Task<float?> ShowWeightDialog(bool error = false) {
            MetroWindow mainWindow = App.Current.MainWindow as MetroWindow;

            MetroDialogSettings settings = new MetroDialogSettings() {
                NegativeButtonText = "Cancelar"
            };

            string result = await mainWindow.ShowInputAsync("Inserir Peso", "Insira o peso:", settings);
            if(result == null)
                return null;

            if(float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out float value)
            || float.TryParse(result, NumberStyles.Float, CultureInfo.GetCultureInfo("pt-BR"), out value))
                return value;

            await mainWindow.ShowMessageAsync("Erro", "Valor de entrada inválido.", MessageDialogStyle.Affirmative, settings);
            return await ShowWeightDialog(true);
        }

        public static async Task<string> ShowDescriptionDialog(ArestaControl aresta, bool error = false) {
            MetroWindow mainWindow = App.Current.MainWindow as MetroWindow;

            MetroDialogSettings settings = new MetroDialogSettings() {
                NegativeButtonText = "Cancelar"
            };

            string result = await mainWindow.ShowInputAsync("Renomear", $"Insira a nova descrição para a aresta { aresta.ToString() }:", settings);
            return result == null || result.Length == 0 ? null : result;
        }
    }
}
