using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Globalization;
using System.Threading.Tasks;

namespace Nhafo.WPF.Dialogs {
    public static class ArestaPesoDialog {
        public static async Task<float?> Show(bool error = false) {
            MetroWindow mainWindow = App.Current.MainWindow as MetroWindow;

            MetroDialogSettings settings = new MetroDialogSettings() {
                //AffirmativeButtonText = "Confirmar",
                NegativeButtonText = "Cancelar"
            };
            if(error)
                settings.AnimateShow = false;

            string result = await mainWindow.ShowInputAsync("Inserir Peso", "Insira o peso:", settings);
            if(result == null)
                return null;

            if(float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out float value)
            || float.TryParse(result, NumberStyles.Float, CultureInfo.GetCultureInfo("pt-BR"), out value))
                return value;

            settings = new MetroDialogSettings() {
                AnimateShow = false,
                AnimateHide = false,
            };

            await mainWindow.ShowMessageAsync("Erro", "Valor de entrada inválido.", MessageDialogStyle.Affirmative, settings);
            return await Show(true);
        }
    }
}
