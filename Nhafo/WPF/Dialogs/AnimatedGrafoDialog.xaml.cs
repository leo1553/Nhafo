using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Nhafo.Code.Factories;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Nhafo.WPF.Dialogs {
    /// <summary>
    /// Interação lógica para AnimatedGrafoDialog.xam
    /// </summary>
    public partial class AnimatedGrafoDialog : CustomDialog {
        private static readonly AnimatedGrafoDialog Instance = new AnimatedGrafoDialog();

        private TaskCompletionSource<bool> tcs = null;
        private readonly MetroWindow mainWindow;

        private int stage = 0;
        private List<IGrafoStage> stages = null;
        public VerticeControl SelectedVertice = null;
        public GrafoControl Grafo { get; private set; }
        private readonly DispatcherTimer timer = null;

        public AnimatedGrafoDialog() {
            InitializeComponent();

            mainWindow = Application.Current.MainWindow as MetroWindow;
            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Tick += TimerTick;
        }

        private async void GrafoButtonClick(object sender, RoutedEventArgs e) {
            await Stop();

            if(tcs != null)
                tcs.TrySetResult(true);
            tcs = null;
        }

        private async void CancelButtonClick(object sender, RoutedEventArgs e) {
            await Stop();

            if(tcs != null)
                tcs.TrySetResult(false);
            tcs = null;
        }

        private async Task Stop() {
            await mainWindow.HideMetroDialogAsync(this);

            timer.Stop();
            grid.Children.Clear();
        }

        public static async Task<bool> Show(GrafoControl grafo, List<IGrafoStage> stages, string title = null) {
            foreach(IGrafoStage stage in stages)
                Console.WriteLine("2. " + ((stage as AddArestaStage).Data as ArestaControl).Weight);

            Instance.Title = title ?? string.Empty;
            Instance.stage = 0;

            stages.Insert(0, new SleepStage(1000));
            foreach(IGrafoStage stage in stages) 
                stage.Context = Instance;

            await Instance.mainWindow.Dispatcher.InvokeAsync(() => {
                Instance.Grafo = CloneGrafo(grafo, stages);
            });

            Instance.stages = stages;
            Instance.timer.Interval = new TimeSpan(0, 0, 0, 0, stages[0].Sleep);
            Instance.timer.Start();

            Instance.grid.Children.Add(Instance.Grafo);

            await Instance.mainWindow.ShowMetroDialogAsync(Instance);
            Instance.tcs = new TaskCompletionSource<bool>();
            return await Instance.tcs.Task;
        }

        private void TimerTick(object sender, EventArgs e) {
            NextStage();
            timer.Interval = new TimeSpan(0, 0, 0, 0, stages[stage].Sleep);
        }

        private void NextStage() {
            // End
            if(stage >= 0)
                stages[stage].End();
            
            // Reset
            if(++stage == stages.Count) {
                for(int i = stages.Count - 1; i >= 0; i--)
                    stages[i].Undo();
                stage = 0;
            }

            // Do
            stages[stage].Do();
        }

        private static GrafoControl CloneGrafo(GrafoControl sourceGrafo, List<IGrafoStage> stages) {
            Dictionary<VerticeControl, VerticeControl> vertices = new Dictionary<VerticeControl, VerticeControl>();
            Dictionary<ArestaControl, ArestaControl> arestas = new Dictionary<ArestaControl, ArestaControl>();

            GrafoControl grafo = GrafoFactory.Create();
            VerticeControl vertice;
            ArestaControl aresta;
            
            foreach(VerticeControl v in sourceGrafo.Vertices) {
                vertice = v.Clone();
                vertices.Add(v, vertice);
                grafo.AddVertice(vertice);
            }
            foreach(ArestaControl a in sourceGrafo.Arestas) {
                aresta = new ArestaControl() {
                    VerticeA = vertices[a.VerticeA],
                    VerticeB = vertices[a.VerticeB],
                    Weight = a.Weight
                };
                arestas.Add(a, aresta);
                grafo.AddAresta(aresta);
            }

            foreach(IGrafoStage stage in stages) {
                if(stage.Data is VerticeControl v) {
                    if(vertices.ContainsKey(v))
                        stage.Data = vertices[v];
                    else {
                        vertice = v.Clone();
                        vertices.Add(v, vertice);
                        stage.Data = vertice;
                    }
                }
                else if(stage.Data is ArestaControl a) {
                    if(arestas.ContainsKey(a))
                        stage.Data = arestas[a];
                    else {
                        aresta = new ArestaControl() {
                            VerticeA = vertices[a.VerticeA],
                            VerticeB = vertices[a.VerticeB],
                            Weight = a.Weight
                        };
                        arestas.Add(a, aresta);
                        stage.Data = aresta;
                    }
                }
            }

            GrafoFactory.CenterGrafoContent(grafo);
            return grafo;
        }
    }

    public interface IGrafoStage {
        AnimatedGrafoDialog Context { get; set; }
        object Data { get; set; }
        int Sleep { get; }
        void Do();
        void Undo();
        void End();
    }
    public class SleepStage : IGrafoStage {
        public AnimatedGrafoDialog Context { get; set; }
        public object Data { get; set; } = null;

        public int Sleep { get; private set; }

        public SleepStage(int sleep) {
            Sleep = sleep;
        }

        public void Do() { }
        public void Undo() { }
        public void End() { }
    }
    public class AddVerticeStage : IGrafoStage {
        public const int DefaultSleep = 500;
        public AnimatedGrafoDialog Context { get; set; }
        public object Data { get; set; }
        public int Sleep { get; set; }

        public AddVerticeStage(VerticeControl data, int sleep = DefaultSleep) {
            Data = data;
            Sleep = sleep;
        }

        public void Do() {
            if(Data is VerticeControl control)
                Context.Grafo.AddVertice(control);
        }

        public void Undo() {
            if(Data is VerticeControl control)
                Context.Grafo.RemoveVertice(control);
        }

        public void End() { }
    }
    public class RemoveVerticeStage : IGrafoStage {
        public const int DefaultSleep = 500;
        public AnimatedGrafoDialog Context { get; set; }
        public object Data { get; set; }
        public int Sleep { get; set; }

        public RemoveVerticeStage(VerticeControl data, int sleep = DefaultSleep) {
            Data = data;
            Sleep = sleep;
        }

        public void Do() {
            if(Data is VerticeControl control)
                Context.Grafo.RemoveVertice(control);
        }

        public void Undo() {
            if(Data is VerticeControl control)
                Context.Grafo.AddVertice(control);
        }

        public void End() { }
    }
    public class AddArestaStage : IGrafoStage {
        public const int DefaultSleep = 500;
        public AnimatedGrafoDialog Context { get; set; }
        public object Data { get; set; }
        public int Sleep { get; set; }

        public AddArestaStage(ArestaControl data, int sleep = DefaultSleep) {
            Data = data;
            Sleep = sleep;
        }

        public void Do() {
            if(Data is ArestaControl control)
                Context.Grafo.AddAresta(control);
        }

        public void Undo() {
            if(Data is ArestaControl control)
                Context.Grafo.RemoveAresta(control);
        }

        public void End() { }
    }
    public class RemoveArestaStage : IGrafoStage {
        public const int DefaultSleep = 500;
        public AnimatedGrafoDialog Context { get; set; }
        public object Data { get; set; }
        public int Sleep { get; set; }

        public RemoveArestaStage(ArestaControl data, int sleep = DefaultSleep) {
            Data = data;
            Sleep = sleep;
        }

        public void Do() {
            if(Data is ArestaControl control)
                Context.Grafo.RemoveAresta(control);
        }

        public void Undo() {
            if(Data is ArestaControl control)
                Context.Grafo.AddAresta(control);
        }

        public void End() { }
    }
    public class SelectVerticeStage : IGrafoStage {
        public const int DefaultSleep = 500;
        public static Brush SelectedBrush { get; set; } = Brushes.Magenta;
            
        public AnimatedGrafoDialog Context { get; set; }
        public object Data { get; set; }
        public int Sleep { get; set; }
        private Brush stroke;

        public SelectVerticeStage(VerticeControl data, int sleep = DefaultSleep) {
            Data = data;
            Sleep = DefaultSleep;
            stroke = data.Stroke;
        }

        public void Do() {
            if(Data is VerticeControl control)
                control.Stroke = SelectedBrush;
        }

        public void Undo() {
            if(Data is VerticeControl control)
                control.Stroke = stroke;
        }

        public void End() => Undo();
    }
}
