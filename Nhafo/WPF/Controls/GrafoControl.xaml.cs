using Nhafo.Code;
using Nhafo.Code.Factories;
using Nhafo.Code.Models;
using Nhafo.Code.Services.Undo;
using Nhafo.Code.Utils;
using Nhafo.WPF.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nhafo.WPF.Controls {
    /// <summary>
    /// Interação lógica para GrafoControl.xam
    /// </summary>
    public partial class GrafoControl : DraggableUserControl, IGrafo<VerticeControl, ArestaControl>, IUndoTarget {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GrafoControl),
            new PropertyMetadata(string.Empty));

        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private string _key = string.Empty;
        public string Key {
            get => _key;
            set {
                _key = value;
                Text = value;
            }
        }

        private List<VerticeControl> _vertices = new List<VerticeControl>();
        public IReadOnlyList<VerticeControl> Vertices => _vertices;

        private List<ArestaControl> _arestas = new List<ArestaControl>();
        public IReadOnlyList<ArestaControl> Arestas => _arestas;

        private Point _location = PointUtils.Zero;
        public Point Location {
            get => _location;
            set {
                _location = value;
                SetPosition(value);
            }
        }

        private ArestaType _arestaType = ArestaType.Common;
        public ArestaType ArestaType {
            get => _arestaType;
            set {
                if(value == _arestaType)
                    return;
                _arestaType = value;

                if(value == ArestaType.Common) {
                    ArestaControl aresta;
                    for(int i = 0; i < Arestas.Count; i++) {
                        aresta = Arestas[i];
                        if(aresta.Direction == ArestaType.Directional) {
                            if(AreConnected(aresta.VerticeB, aresta.VerticeA)) {
                                RemoveAresta(aresta);
                                i--;
                            }
                        }
                        aresta.Direction = ArestaType.Common;
                    }
                }
                else {
                    foreach(ArestaControl aresta in Arestas) {
                        aresta.Direction = ArestaType.Common;
                    }
                }
            }
        }

        public bool ProcessingUndo { get; set; }

        Point offset;

        public GrafoControl() {
            InitializeComponent();

            SubscribeElement(titleBorder);
            Draggable = true;

            ClipToBounds = true;

            Loaded += (sender, args) => Location = _location;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem;

            MenuItem generateWeightMenu = new MenuItem() {
                Header = "Gerar pesos",
            };

            // 0 → 1
            menuItem = new MenuItem() { Header = "Aleatório: 0.0 → 1.0" };
            menuItem.Click += (s, a) => {
                Random random = new Random();
                foreach(ArestaControl aresta in Arestas)
                    aresta.Weight = random.NextDouble();
            };
            generateWeightMenu.Items.Add(menuItem);

            // 0 → 10
            menuItem = new MenuItem() { Header = "Aleatório: 0.0 → 10.0" };
            menuItem.Click += (s, a) => {
                Random random = new Random();
                foreach(ArestaControl aresta in Arestas)
                    aresta.Weight = random.NextDouble() * 10;
            };
            generateWeightMenu.Items.Add(menuItem);

            // 0 → 100
            menuItem = new MenuItem() { Header = "Aleatório: 0 → 100" };
            menuItem.Click += (s, a) => {
                Random random = new Random();
                foreach(ArestaControl aresta in Arestas)
                    aresta.Weight = random.Next(0, 100);
            };
            generateWeightMenu.Items.Add(menuItem);

            // Distancia
            menuItem = new MenuItem() { Header = "Distância" };
            menuItem.Click += (s, a) => {
                foreach(ArestaControl aresta in Arestas) {
                    if(!aresta.IsLoop)
                        aresta.Weight = aresta.VerticeA.Location.Distance(aresta.VerticeB.Location);
                    else
                        aresta.Weight = 1;
                }
            };
            generateWeightMenu.Items.Add(menuItem);

            // Renomear
            menuItem = new MenuItem() { Header = "Renomear" };
            menuItem.Click += async (s, a) => {
                string newName = await GrafoDialogs.ShowRenameDialog(this);
                if(newName != null)
                    Key = newName;
            };
            contextMenu.Items.Add(menuItem);

            contextMenu.Items.Add(generateWeightMenu);
            titleBorder.ContextMenu = contextMenu;
        }

        protected override void OnDragStart() {
            BringToFront();
            offset = Mouse.GetPosition(titleBorder);
        }

        protected override void OnDragMove(MouseEventArgs e) {
            Point position =
                e.GetPosition(ParentPanel).
                Sub(offset)/*.
                Clamp(PointUtils.Zero,
                    new Point(ParentPanel.ActualWidth - Width, ParentPanel.ActualHeight - Height))*/;
            
            Location = GetRelativePosition(position);
        }

        public void AddVertice(VerticeControl vertice) {
            vertice.Grafo = this;
            canvas.Children.Add(vertice);
            _vertices.Add(vertice);

            ArestaFactory.SubscribeVertice(vertice);
        }

        public void RemoveVertice(VerticeControl vertice) {
            for(int i = 0; i < _arestas.Count; i++) {
                if(_arestas[i].ContainsVertice(vertice)) {
                    RemoveAresta(_arestas[i]);
                    i--;
                }
            }

            ArestaFactory.UnsubscribeVertice(vertice);

            vertice.Grafo = null;
            canvas.Children.Remove(vertice);
            _vertices.Remove(vertice);
        }

        public void AddAresta(ArestaControl aresta) {
            aresta.Grafo = this;
            canvas.Children.Add(aresta);
            _arestas.Add(aresta);

            aresta.VerticeA.AddAresta(aresta);
            if(aresta.VerticeA != aresta.VerticeB)
                aresta.VerticeB.AddAresta(aresta);
        }

        public void RemoveAresta(ArestaControl aresta) {
            aresta.Grafo = null;
            canvas.Children.Remove(aresta);
            _arestas.Remove(aresta);

            aresta.VerticeA.RemoveAresta(aresta);
            if(aresta.VerticeA != aresta.VerticeB)
                aresta.VerticeB.RemoveAresta(aresta);
        }

        public bool AreConnected(VerticeControl verticeA, VerticeControl verticeB) {
            if(verticeA.Grafo != verticeB.Grafo)
                return false;

            foreach(ArestaControl aresta in verticeA.Arestas) {
                if(aresta.Direction == ArestaType.Common) {
                    if((aresta.VerticeA == verticeA && aresta.VerticeB == verticeB)
                    || (aresta.VerticeB == verticeA && aresta.VerticeA == verticeB)) {
                        return true;
                    }
                }
                else {
                    if(aresta.VerticeA == verticeA && aresta.VerticeB == verticeB) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
