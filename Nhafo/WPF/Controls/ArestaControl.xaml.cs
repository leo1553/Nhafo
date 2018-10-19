using Nhafo.Code.Models;
using Nhafo.Code.Services.Undo;
using Nhafo.Code.Utils;
using Nhafo.WPF.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Nhafo.WPF.Controls {
    /// <summary>
    /// Interação lógica para ArestaControl.xam
    /// </summary>
    public partial class ArestaControl : DraggableUserControl, IAresta<VerticeControl, ArestaControl>, IUndoTarget {
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ArestaControl),
            new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ArestaControl),
            new PropertyMetadata(1D));
        public static readonly DependencyProperty PointAProperty = DependencyProperty.Register("PointA", typeof(Point), typeof(ArestaControl));
        public static readonly DependencyProperty PointBProperty = DependencyProperty.Register("PointB", typeof(Point), typeof(ArestaControl));
        public static readonly DependencyProperty MiddlePointProperty = DependencyProperty.Register("MiddlePoint", typeof(Point), typeof(ArestaControl));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ArestaControl),
            new PropertyMetadata(string.Empty));

        public Brush Stroke {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public double StrokeThickness {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public Point PointA {
            get => (Point)GetValue(PointAProperty);
            set {
                if(MiddlePoint == PointA)
                    MiddlePoint = value;

                SetValue(PointAProperty, value);
            }
        }

        public Point PointB {
            get => (Point)GetValue(PointBProperty);
            set => SetValue(PointBProperty, value);
        }

        public Point MiddlePoint {
            get => (Point)GetValue(MiddlePointProperty);
            set => SetValue(MiddlePointProperty, value);
        }

        public string Text {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public VerticeControl _verticeA = null;
        public VerticeControl VerticeA {
            get => _verticeA;
            set {
                _verticeA = value;
                VerticeLocationUpdated(value);
            }
        }

        public VerticeControl _verticeB = null;
        public VerticeControl VerticeB {
            get => _verticeB;
            set {
                _verticeB = value;
                VerticeLocationUpdated(value);
            }
        }

        public double _weight = double.NaN;
        public double Weight {
            get => _weight;
            set {
                _weight = value;
                if(double.IsNaN(value)) {
                    textBlock.Visibility = Visibility.Hidden;
                    ellipse.Visibility = Visibility.Visible;
                    addPesoMenuItem.Header = "Adicionar Peso";
                }
                else {
                    Text = _weight.ToString();
                    textBlock.Visibility = Visibility.Visible;
                    ellipse.Visibility = Visibility.Hidden;
                    addPesoMenuItem.Header = "Remover Peso";
                }
                UpdateBezierRelated();
            }
        }

        public ArestaType _type = ArestaType.Common;
        public ArestaType Direction {
            get => _type;
            set {
                _type = value;
                TypeUpdate();
            }
        }

        private IGrafo<VerticeControl, ArestaControl> _grafo = null;
        public IGrafo<VerticeControl, ArestaControl> Grafo {
            get => _grafo;
            set {
                if(_grafo != null && value != null)
                    throw new Exception();
                _grafo = value;
            }
        }

        MenuItem addPesoMenuItem;
        MenuItem tipoSimplesMenuItem;
        MenuItem tipoDirecionalMenuItem;
        MenuItem removerCurvaMenuItem;

        bool draggable = true;
        public override bool Draggable {
            get => draggable && !IsLoop;
            set {
                draggable = value;
                removerCurvaMenuItem.IsEnabled = Draggable;
            }
        }
        public bool IsLoop => VerticeA == VerticeB;

        public bool ProcessingUndo { get; set; }

        Point dragStartLocation;

        public ArestaControl() {
            InitializeComponent();

            SubscribeElement(ellipse);
            SubscribeElement(textBlock);

            ContextMenu contextMenu = new ContextMenu();

            addPesoMenuItem = new MenuItem() {
                Header = "Adicionar Peso",
            };
            addPesoMenuItem.Click += async (sender, args) => {
                if(double.IsNaN(Weight)) {
                    /*PesoDialog dialog = new PesoDialog(MainWindow.Instance);
                    if(dialog.ShowDialog() == true) {
                        UndoService.Instance.RegisterAction(new UndoChangeArestaWeight(this, Weight, dialog.Result));
                        Weight = dialog.Result;
                    }*/
                    float? result;
                    if((result = await ArestaPesoDialog.Show()) != null) {
                        UndoService.Instance.RegisterAction(new UndoChangeArestaWeight(this, Weight, result.Value));
                        Weight = result.Value;
                    }
                }
                else {
                    UndoService.Instance.RegisterAction(new UndoChangeArestaWeight(this, Weight, float.NaN));
                    Weight = float.NaN;
                }
            };
            contextMenu.Items.Add(addPesoMenuItem);

            MenuItem menuItem = new MenuItem() {
                Header = "Tipo de Aresta"
            };

            tipoSimplesMenuItem = new MenuItem() {
                Header = "Simples",
                IsCheckable = true,
                IsChecked = Direction == ArestaType.Common
            };
            tipoSimplesMenuItem.Click += (sender, args) => Direction = ArestaType.Common;

            tipoDirecionalMenuItem = new MenuItem() {
                Header = "Direcional",
                IsCheckable = true,
                IsChecked = Direction == ArestaType.Directional
            };
            tipoDirecionalMenuItem.Click += (sender, args) => Direction = ArestaType.Directional;

            menuItem.Items.Add(tipoSimplesMenuItem);
            menuItem.Items.Add(tipoDirecionalMenuItem);
            contextMenu.Items.Add(menuItem);
            contextMenu.Items.Add(new Separator());

            removerCurvaMenuItem = new MenuItem() {
                Header = "Remover Curva"
            };
            removerCurvaMenuItem.Click += (sender, args) => {
                UndoService.Instance.RegisterAction(new UndoMoveAresta(this, MiddlePoint, PointA));
                MiddlePoint = PointA;
                UpdateBezierRelated();
            };
            contextMenu.Items.Add(removerCurvaMenuItem);

            menuItem = new MenuItem() {
                Header = "Apagar Aresta"
            };
            menuItem.Click += (sender, args) => {
                UndoService.Instance.RegisterAction(new UndoRemoveAresta(this));
                Grafo.RemoveAresta(this);
            };
            contextMenu.Items.Add(menuItem);

            ellipse.ContextMenu = contextMenu;
            textBlock.ContextMenu = contextMenu;
            path.ContextMenu = contextMenu;
            backgroundPath.ContextMenu = contextMenu;

            Loaded += (sender, args) => {
                VerticeLocationUpdated(null);
            };

            Panel.SetZIndex(this, 0);
        }

        public void VerticeLocationUpdated(VerticeControl vertice) {
            if(IsLoop) {
                PointA = new Point(VerticeA.Location.X + 10, VerticeA.Location.Y);
                PointB = new Point(VerticeA.Location.X, VerticeA.Location.Y + 10);
                MiddlePoint = new Point(VerticeA.Location.X + 25, VerticeA.Location.Y + 25);
            }
            else if(vertice == VerticeA)
                PointA = VerticeA.Location;
            else
                PointB = VerticeB.Location;

            UpdateBezierRelated();
            UpdateArrow();
        }

        private void TypeUpdate() {
            if(Direction == ArestaType.Common) {
                tipoSimplesMenuItem.IsChecked = true;
                tipoDirecionalMenuItem.IsChecked = false;
                image.Visibility = Visibility.Hidden;
            }
            else {
                tipoSimplesMenuItem.IsChecked = false;
                tipoDirecionalMenuItem.IsChecked = true;
                image.Visibility = Visibility.Visible;
                UpdateArrow();
            }
        }

        private void UpdateArrow() {
            if(Direction == ArestaType.Directional)
                image.RenderTransform = new RotateTransform(GetArrowAngle(), image.Width / 2, image.Height / 2);
        }

        private void UpdateBezierRelated() {
            Point middle;
            if(PointA == MiddlePoint)
                middle = new Point((PointA.X + PointB.X) / 2, (PointA.Y + PointB.Y) / 2);
            else {
                // Bezier Curve Formula
                // P(t) = P0*t^2 + P1*2*t*(1-t) + P2*(1-t)^2

                /*middle = new Point(
                    (PointA.X * Math.Pow(0.5, 2)) + (MiddlePoint.X * 2 * 0.5 * (1 - 0.5)) + (PointB.X * Math.Pow(1 - 0.5, 2)),
                    (PointA.Y * Math.Pow(0.5, 2)) + (MiddlePoint.Y * 2 * 0.5 * (1 - 0.5)) + (PointB.Y * Math.Pow(1 - 0.5, 2)));*/

                middle = new Point(
                    (PointA.X * .25) + (MiddlePoint.X * (.5)) + (PointB.X * .25),
                    (PointA.Y * .25) + (MiddlePoint.Y * (.5)) + (PointB.Y * .25));
            }
            
            FrameworkElement elem = 
                double.IsNaN(Weight) ? 
                    ellipse as FrameworkElement : 
                    textBlock as FrameworkElement;
            
            SetPosition(elem, 
                new Point(middle.X - (elem.ActualWidth / 2), middle.Y - (elem.ActualHeight / 2)));

            UpdateArrow();
        }

        protected override void OnDragStart() {
            dragStartLocation = MiddlePoint;
        }

        protected override void OnDragMove(MouseEventArgs e) {
            FrameworkElement element = LastSender as FrameworkElement;
            Point aux = new Point(element.ActualWidth / 2, element.ActualHeight / 2);

            Point position =
                e.GetPosition(canvas).
                    Clamp(aux,
                        new Point(ParentPanel.ActualWidth - element.ActualWidth + aux.X, ParentPanel.ActualHeight - element.ActualHeight + aux.Y));

            // middle = (pos - (a*t^2) - (b*(1-t)^2)) / 2*t*(1-t)
            MiddlePoint = new Point(
                ((position.X) - (PointA.X * .25) - (PointB.X * .25)) * 2,
                ((position.Y) - (PointA.Y * .25) - (PointB.Y * .25)) * 2);

            UpdateBezierRelated();
            UpdateArrow();
        }

        protected override void OnDragStop() {
            if(MiddlePoint != dragStartLocation)
                UndoService.Instance.RegisterAction(new UndoMoveAresta(this, dragStartLocation, MiddlePoint));
        }

        private double GetArrowAngle() {
            Point a = new Point(
                    (PointA.X * .0001) + (MiddlePoint.X * 0.0198) + (PointB.X * 0.9801),
                    (PointA.Y * .0001) + (MiddlePoint.Y * 0.0198) + (PointB.Y * 0.9801));
            
            Point b = PointB;
            return Math.Atan2(b.Y - a.Y, b.X - a.X) * 180 / Math.PI;
        }

        public bool ContainsVertice(VerticeControl vertice) {
            return VerticeA == vertice || VerticeB == vertice;
        }

        public VerticeControl GetOposite(VerticeControl vertice) {
            if(IsLoop)
                return VerticeA;
            if(VerticeA == vertice)
                return VerticeB;
            if(VerticeB == vertice)
                return VerticeA;
            return null;
        }
    }
}
