using Nhafo.Code;
using Nhafo.Code.Factories;
using Nhafo.Code.Models;
using Nhafo.Code.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using Nhafo.Code.Services.Undo;
using Nhafo.WPF.Dialogs;

namespace Nhafo.WPF.Controls {
    /// <summary>
    /// Interação lógica para VerticeControl.xam
    /// </summary>
    public partial class VerticeControl : DraggableUserControl, IVertice<VerticeControl, ArestaControl>, IUndoTarget {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(VerticeControl),
            new PropertyMetadata("~"));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(VerticeControl),
            new PropertyMetadata(ColorUtils.AccentColor));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(VerticeControl),
            new PropertyMetadata(Brushes.Black));

        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Brush Fill {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public Brush Stroke {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        private Point _location = PointUtils.Zero;
        public Point Location {
            get => _location;
            set {
                _location = value;
                SetPosition(new Point(value.X - (Width / 2), value.Y - (Height / 2)));

                foreach(ArestaControl aresta in Arestas)
                    aresta.VerticeLocationUpdated(this);
            }
        }

        private Color _color = ColorUtils.AccentColor.Color;
        public Color Color {
            get => _color;
            set {
                _color = value;
                Fill = new SolidColorBrush(value);

                if(value.GetLuminance() > .5)
                    Foreground = Brushes.Black;
                else
                    Foreground = Brushes.White;
            }
        }

        private string _key = "~";
        public string Key {
            get => _key;
            set {
                _key = value;
                Text = value;
            }
        }

        private List<ArestaControl> _arestas = new List<ArestaControl>();
        public IReadOnlyList<ArestaControl> Arestas => _arestas;
        
        private IGrafo<VerticeControl, ArestaControl> _grafo = null;
        public IGrafo<VerticeControl, ArestaControl> Grafo {
            get => _grafo;
            set {
                if(_grafo != null && value != null)
                    throw new Exception();
                _grafo = value;
            }
        }

        public IReadOnlyList<VerticeControl> ConnectedVertices {
            get {
                List<VerticeControl> vertices = new List<VerticeControl>();
                foreach(ArestaControl aresta in Arestas) {
                    if(aresta.IsLoop)
                        vertices.Add(this);
                    else
                        vertices.Add(aresta.GetOposite(this));
                }
                return vertices.AsReadOnly();
            }
        }

        public bool ProcessingUndo { get; set; }

        Point offset;
        Point dragStartLocation;
        ContextMenu contextMenu;
        
        public VerticeControl() {
            InitializeComponent();
            
            SubscribeElement(this);

            contextMenu = new ContextMenu();
            MenuItem menuItem;

            // Renomear
            menuItem = new MenuItem() { Header = "Renomear" };
            menuItem.Click += async (sender, args) => {
                string newName = await VerticeDialogs.ShowRenameDialog(this);
                if(newName != null)
                    Key = newName;
            };
            contextMenu.Items.Add(menuItem);

            // Apagar
            menuItem = new MenuItem() { Header = "Apagar Vertice" };
            menuItem.Click += (sender, args) => {
                UndoService.Instance.RegisterAction(new UndoRemoveVertice(this));
                Grafo.RemoveVertice(this);
            };
            contextMenu.Items.Add(menuItem);

            ContextMenu = contextMenu;

            Panel.SetZIndex(this, 5);

            Loaded += (sender, args) => {
                Location = _location;
                Color = _color;
            };
        }

        protected override void OnDragStart() {
            BringToFront();
            offset = Mouse.GetPosition(this).Sub(new Point(Width / 2, Height / 2));
            dragStartLocation = Location;
        }

        protected override void OnDragMove(MouseEventArgs e) {
            Point position = 
                e.GetPosition(ParentPanel).
                    Sub(offset).
                    Clamp(new Point(Width / 2, Height / 2),
                        new Point(ParentPanel.ActualWidth - (Width / 2), ParentPanel.ActualHeight - (Height / 2)));
            
            Location = GetRelativePosition(position);
        }

        protected override void OnDragStop() {
            if(Location != dragStartLocation)
                UndoService.Instance.RegisterAction(new UndoMoveVertice(this, dragStartLocation, Location));
        }

        public void EnableContextMenu(bool enable) {
            if(enable)
                ContextMenu = contextMenu;
            else
                ContextMenu = null;
        }

        public void AddAresta(ArestaControl aresta) {
            _arestas.Add(aresta);
        }

        public void RemoveAresta(ArestaControl aresta) {
            _arestas.Remove(aresta);
        }

        public VerticeControl Clone() {
            return new VerticeControl() {
                Width = Width,
                Height = Height,
                Key = Key,
                Location = Location,
                Color = Color,
                Draggable = Draggable
            };
        }

        public override string ToString() {
            return Key.ToString();
        }
    }
}
