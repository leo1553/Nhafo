using MahApps.Metro.Controls;
using Nhafo.Code.Factories;
using Nhafo.Code.Services.Undo;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using Xceed.Wpf.Toolkit;
using Nhafo.Code.GrafoOperations;
using Nhafo.Code.Utils;
using MahApps.Metro.Controls.Dialogs;
using Nhafo.WPF.Dialogs;
using System.Windows.Media;
using System.Windows.Data;

namespace Nhafo {
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public static readonly DependencyProperty IsToolbarMenuOpenProperty = DependencyProperty.Register("IsToolbarMenuOpen", typeof(bool), typeof(MainWindow));
        public static MainWindow Instance { get; private set; }

        private static readonly GrafoControl InvalidGrafoControl = new GrafoControl() { Key = "Selecione um Grafo" };

        public IReadOnlyList<GrafoControl> GrafoControls {
            get {
                List<GrafoControl> result = new List<GrafoControl>();
                foreach (UIElement elem in cartesianPlane.Children)
                    if (elem is GrafoControl control)
                        result.Add(control);
                return result;
            }
        }

        private ObservableCollection<GrafoComboBoxItem> GrafoComboBoxItems { get; set; } = new ObservableCollection<GrafoComboBoxItem>();
        private Grid openToolbarMenu = null;

        private Storyboard toolbarShowStoryboard = new Storyboard();
        private Storyboard toolbarHideStoryboard = new Storyboard();

        public bool IsToolbarMenuOpen {
            get => (bool)GetValue(IsToolbarMenuOpenProperty);
            set => SetValue(IsToolbarMenuOpenProperty, value);
        }

        public MainWindow() {
            Instance = this;

            InitializeComponent();

            Loaded += _Loaded;
            MouseDown += MainWindowMouseDown;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, _Undo, _CanUndo));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, _Redo, _CanRedo));

            grafosComboBox.ItemsSource = GrafoComboBoxItems;
            grafosComboBox.SelectedIndex = 0;
            grafosComboBox.SelectionChanged += _GrafosComboBoxSelectionChanged;
            GrafoComboBoxItems.Add(new GrafoComboBoxItem(InvalidGrafoControl));

            CreateAnimations();

            foreach (UIElement elem in toolbar.Children) {
                if (elem is Grid grid) {
                    grid.Margin = new Thickness(0, -250, 0, 0);
                    grid.Visibility = Visibility.Hidden;
                }
            }

            //ColorItem defaultColor = new ColorItem(verticeColorPicker.SelectedColor, "Default");
            //verticeColorPicker.StandardColors[0] = defaultColor;
        }

        private void _Loaded(object sender, RoutedEventArgs e) {
            Loaded -= _Loaded;

            GrafoFactory.CreateCenter(cartesianPlane, new Size(Width * .75, Height * .75));
            grafoNamePlaceholder.TextBox = grafoNameTextBox;
            //verticeNamePlaceholder.TextBox = verticeNameTextBox;
        }

        private void MainWindowMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition(this));
                if (htr.VisualHit != null) {
                    GrafoControl grafo = FindParent<GrafoControl>(htr.VisualHit);
                    if (grafo != null)
                        grafosComboBox.SelectedItem = grafo;
                }
            }
        }

        private T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject {
            DependencyObject parent = dependencyObject.GetParentObject();
            if (parent == null)
                return null;
            else if (parent is T)
                return parent as T;
            return FindParent<T>(parent);
        }

        private void CreateAnimations() {
            ObjectAnimationUsingKeyFrames objAnimation;
            ThicknessAnimation thicknessAnimation;

            // toolbarShowStoryboard
            objAnimation = new ObjectAnimationUsingKeyFrames();
            objAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible) { KeyTime = TimeSpan.Zero });
            toolbarShowStoryboard.Children.Add(objAnimation);
            Storyboard.SetTargetProperty(objAnimation, new PropertyPath("Visibility"));

            thicknessAnimation = new ThicknessAnimation(new Thickness(0), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            toolbarShowStoryboard.Children.Add(thicknessAnimation);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("Margin"));

            // toolbarHideStoryboard
            thicknessAnimation = new ThicknessAnimation(new Thickness(0, -250, 0, 0), new Duration(new TimeSpan(0, 0, 0, 0, 300)));
            toolbarHideStoryboard.Children.Add(thicknessAnimation);
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath("Margin"));

            objAnimation = new ObjectAnimationUsingKeyFrames();
            objAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Hidden) { KeyTime = new TimeSpan(0, 0, 0, 0, 300) });
            toolbarHideStoryboard.Children.Add(objAnimation);
            Storyboard.SetTargetProperty(objAnimation, new PropertyPath("Visibility"));
        }

        public void AddGrafo(GrafoControl control) {
            cartesianPlane.Children.Add(control);
            GrafoControlsChanged(control, true);
        }

        public void RemoveGrafo(GrafoControl control) {
            cartesianPlane.Children.Remove(control);
            GrafoControlsChanged(control, false);
        }

        public void GrafoControlsChanged(GrafoControl control, bool addeded) {
            if (control == InvalidGrafoControl)
                return;

            if (addeded) {
                GrafoComboBoxItems.Add(new GrafoComboBoxItem(control));
            }
            else {
                if (grafosComboBox.SelectedItem is GrafoComboBoxItem selectedItem)
                    if (selectedItem.GrafoControl == control)
                        grafosComboBox.SelectedIndex = 0;

                GrafoComboBoxItem toRemove = null;
                foreach (GrafoComboBoxItem item in GrafoComboBoxItems) {
                    if (item.GrafoControl == control) {
                        toRemove = item;
                        break;
                    }
                }
                if (toRemove != null)
                    GrafoComboBoxItems.Remove(toRemove);
            }
        }

        private void _GrafoButtonClick(object sender, RoutedEventArgs e) => OpenToolbarMenu(grafoToolbarMenu);
        //private void _VerticeButtonClick(object sender, RoutedEventArgs e) => OpenToolbarMenu(verticeToolbarMenu);
        private void _ToolbarCloseButtonClick(object sender, RoutedEventArgs e) => CloseToolbarMenu();

        private void OpenToolbarMenu(Grid toolbarMenu) {
            if (openToolbarMenu == toolbarMenu) {
                CloseToolbarMenu();
                return;
            }

            if (openToolbarMenu != null)
                openToolbarMenu.BeginStoryboard(toolbarHideStoryboard);

            openToolbarMenu = toolbarMenu;
            openToolbarMenu.BeginStoryboard(toolbarShowStoryboard);
            IsToolbarMenuOpen = true;
        }

        private void CloseToolbarMenu() {
            if (openToolbarMenu != null) {
                openToolbarMenu.BeginStoryboard(toolbarHideStoryboard);
                openToolbarMenu = null;
            }

            IsToolbarMenuOpen = false;
        }

        private void _GrafosComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            bool enabled = grafosComboBox.SelectedIndex > 0;
            grafoLocate.IsEnabled = enabled;
            grafoInvert.IsEnabled = enabled;
            grafoDelete.IsEnabled = enabled;
            grafoBipartir.IsEnabled = enabled;
            grafoComponenteConexa.IsEnabled = enabled;
            grafoPrim.IsEnabled = enabled;
            grafoKruskal.IsEnabled = enabled;
            grafoDijkstra.IsEnabled = enabled;
        }

        private void _Undo(object sender, ExecutedRoutedEventArgs e) => UndoService.Instance.Undo();
        private void _Redo(object sender, ExecutedRoutedEventArgs e) => UndoService.Instance.Redo();
        private void _CanUndo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = UndoService.Instance.UndoActions.Count != 0;
        private void _CanRedo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = UndoService.Instance.RedoActions.Count != 0;

        private void AddGrafoButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl control = GrafoFactory.CreateCenter(cartesianPlane, new Size(300, 200));
            if (grafoNameTextBox.Text.Length != 0) {
                control.Key = grafoNameTextBox.Text;
                grafoNameTextBox.Text = string.Empty;
                control.BringToFront();
            }
        }

        private void GrafoInvertButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            GrafoControl result = new InvertGrafo(currentGrafo).Invert();
            result.Location = currentGrafo.Location.Sum(new Point(currentGrafo.ActualWidth * .5, currentGrafo.ActualHeight * .5));
            AddGrafo(result);
            result.BringToFront();
        }

        private async void BipartirGrafoButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            BipartirGrafo operation = new BipartirGrafo(currentGrafo);
            if (operation.CanBeDone()) {
                GrafoControl result = operation.Color();
                result.Location = currentGrafo.Location.Sum(new Point(currentGrafo.ActualWidth * .5, currentGrafo.ActualHeight * .5));
                AddGrafo(result);
                result.BringToFront();
            }
            else {
                await this.ShowMessageAsync("Bipartir", currentGrafo.Key + " não é bipartido.");
            }
        }

        private async void ComponenteConexaButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            VerticeControl vertice = await SelectVerticeDialog.Show(currentGrafo, "Componente Conexa");
            GrafoControl result = new ComponenteConexa(currentGrafo).Generate(vertice);
            result.Location = currentGrafo.Location.Sum(new Point(currentGrafo.ActualWidth * .5, currentGrafo.ActualHeight * .5));
            AddGrafo(result);
            result.BringToFront();
        }

        private async void PrimButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            VerticeControl vertice = await SelectVerticeDialog.Show(currentGrafo, "Algoritmo de Prim");
            if(vertice != null) {
                GrafoControl result = new PrimAlgorithm(currentGrafo).Generate(vertice);
                if(result != null) {
                    result.Location = currentGrafo.Location.Sum(new Point(currentGrafo.ActualWidth * .5, currentGrafo.ActualHeight * .5));
                    AddGrafo(result);
                    result.BringToFront();
                }
            }
        }

        private void KruskalButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            GrafoControl result = new KruskalAlgorithm(currentGrafo).Generate();
            if(result != null) {
                result.Location = currentGrafo.Location.Sum(new Point(currentGrafo.ActualWidth * .5, currentGrafo.ActualHeight * .5));
                AddGrafo(result);
                result.BringToFront();
            }
        }

        private async void DijkstraButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            VerticeControl source = await SelectVerticeDialog.Show(currentGrafo, "Dijkstra - De");
            VerticeControl destiny = await SelectVerticeDialog.Show(currentGrafo, "Dijkstra - Para");

            if(source != null && destiny != null) {
                GrafoControl result = new DijkstraAlgorithm(currentGrafo).Generate(source, destiny);
                if(result != null) {
                    result.Location = currentGrafo.Location.Sum(new Point(currentGrafo.ActualWidth * .5, currentGrafo.ActualHeight * .5));
                    AddGrafo(result);
                    result.BringToFront();
                }
            }
        }

        private async void GrafoDeleteButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            MetroDialogSettings settings = new MetroDialogSettings() {
                AffirmativeButtonText = "Sim",
                NegativeButtonText = "Não"
            };
            if (await this.ShowMessageAsync("Apagar", "Tem certeza que deseja apagar " + currentGrafo.Key + "?", MessageDialogStyle.AffirmativeAndNegative, settings) == MessageDialogResult.Affirmative) {
                RemoveGrafo(currentGrafo);
            }
        }

        private void GrafoLocateButtonClick(object sender, RoutedEventArgs e) {
            GrafoControl currentGrafo = (grafosComboBox.SelectedItem as GrafoComboBoxItem).GrafoControl;
            cartesianPlane.Origin = currentGrafo.Location.Sub(new Point(20, 145));
        }

        private class GrafoComboBoxItem {
            public GrafoControl GrafoControl { get; private set; }

            public GrafoComboBoxItem(GrafoControl control) {
                GrafoControl = control;
            }

            public override string ToString() {
                return GrafoControl.Key;
            }
        }
    }
}

//           n - 1           n * (n - 1)
// Provar -> SIGMA (n - i) = -----------
//           i = 1                2
//             .
//            /|\
// K(n) = -----'
// K(1) = 0
// K(2) = (2 - 1) = 1
// K(3) = (3 - 2) + (3 - 1) = 1 + 2
// K(4) = (4 - 3) + (4 - 2) + (4 - 1) = 1 + 2 + 3
// K(n) = soma da progressão Aritmética de 0 a (n - 1)

// S(n) = (n*(inicial + final)) / 2
// S(n) = (n*(0 + (n - 1))) / 2

// Logo
// K(n) = (n*(n - 1)) / 2