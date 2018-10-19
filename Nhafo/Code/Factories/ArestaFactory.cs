using Nhafo.Code.Utils;
using Nhafo.WPF.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows;
using Nhafo.Code.Models;
using Nhafo.Code.Services.Undo;

namespace Nhafo.Code.Factories {
    public class ArestaFactory {
        public static readonly ArestaFactory Instance = new ArestaFactory();

        public bool Adding { get; private set; } = false;
        public bool HoldRightMouseButton { get; set; } = false;

        Point rightMouseStartPos;
        Line mainLine;
        VerticeControl verticeA = null;
        Panel ParentPanel => mainLine.Parent as Panel;

        private ArestaFactory() {
            mainLine = new Line() {
                Stroke = Brushes.DarkGray,
                StrokeThickness = 1,
                IsHitTestVisible = false
            };

            Panel.SetZIndex(mainLine, 1);
        }
        
        public void Begin(VerticeControl control) {
            if(verticeA != null)
                Stop();

            if(control.Parent is Panel panel) {
                Adding = true;

                mainLine.X1 = control.Location.X;
                mainLine.X2 = control.Location.X;
                mainLine.Y1 = control.Location.Y;
                mainLine.Y2 = control.Location.Y;

                panel.Children.Add(mainLine);
                verticeA = control;
                VerticeFix(false);
                MainWindow.Instance.MouseMove += _MouseMove;
            }
        }

        public void Finish(VerticeControl control) {
            if(verticeA == null)
                return;

            if(control.Grafo == verticeA.Grafo) {
                IGrafo<VerticeControl, ArestaControl> grafo = verticeA.Grafo;
                if(!grafo.AreConnected(verticeA, control)) {
                    ArestaControl aresta = new ArestaControl() {
                        VerticeA = verticeA,
                        VerticeB = control
                    };
                    grafo.AddAresta(aresta);
                    UndoService.Instance.RegisterAction(new UndoAddAresta(aresta));
                }
            }
            Stop();
        }

        public void Stop() {
            if(verticeA == null)
                return;
            if(HoldRightMouseButton)
                MainWindow.Instance.MouseUp -= _WindowMouseUp;

            MainWindow.Instance.MouseMove -= _MouseMove;
            VerticeFix(true);
            verticeA = null;
            ParentPanel.Children.Remove(mainLine);
            Adding = false;
            HoldRightMouseButton = false;
        }

        public static void SubscribeVertice(VerticeControl control) {
            control.MouseDown += Instance._MouseDown;
            control.MouseUp += Instance._MouseUp;
            control.ContextMenuOpening += Instance._ContextMenuOpening;
        }

        public static void UnsubscribeVertice(VerticeControl control) {
            control.MouseDown -= Instance._MouseDown;
            control.MouseUp -= Instance._MouseUp;
            control.ContextMenuOpening -= Instance._ContextMenuOpening;
        }

        private void _ContextMenuOpening(object sender, ContextMenuEventArgs e) {
            if(sender is VerticeControl control) 
                if(Adding && HoldRightMouseButton) 
                    if(rightMouseStartPos.Distance(Mouse.GetPosition(MainWindow.Instance)) > 25)
                        e.Handled = true;
        }

        private void _MouseDown(object sender, MouseButtonEventArgs e) {
            if(sender is VerticeControl control) {
                if(e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Pressed) {
                    if(!Adding) {
                        MainWindow.Instance.MouseUp += _WindowMouseUp;
                        rightMouseStartPos = Mouse.GetPosition(MainWindow.Instance);
                        HoldRightMouseButton = true;
                        Begin(control);
                    }
                }
                else if(e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed) {
                    if(Adding) {
                        Finish(control);
                    }
                }
            }
        }

        private void _MouseUp(object sender, MouseButtonEventArgs e) {
            if(sender is VerticeControl control) {
                if(Adding && HoldRightMouseButton
                && e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released) {
                    if(rightMouseStartPos.Distance(Mouse.GetPosition(MainWindow.Instance)) > 25)
                        Finish(control);
                    else {
                        Stop();
                        control.EnableContextMenu(true);
                    }
                }
            }
        }

        private void _WindowMouseUp(object sender, MouseButtonEventArgs e) {
            if(Adding && HoldRightMouseButton
            && e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released)
                Stop();
        }

        private void _MouseMove(object sender, MouseEventArgs e) {
            if(verticeA == null)
                return;
            if(HoldRightMouseButton == true 
            && e.RightButton != MouseButtonState.Pressed) {
                Stop();
                return;
            }

            Panel parentPanel = ParentPanel;
            Point point =
                Mouse.GetPosition(parentPanel).
                Clamp(PointUtils.Zero, new Point(parentPanel.ActualWidth, parentPanel.ActualHeight));

            mainLine.X2 = point.X;
            mainLine.Y2 = point.Y;
        }

        private void VerticeFix(bool adding) {
            foreach(UIElement element in ParentPanel.Children) {
                if(element is VerticeControl control) {
                    control.Draggable = adding;
                    control.EnableContextMenu(!adding);
                }
            }
        }
    }
}
