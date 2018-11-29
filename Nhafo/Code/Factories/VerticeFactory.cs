using Nhafo.Code.Models;
using Nhafo.Code.Services.Undo;
using Nhafo.WPF.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Nhafo.Code.Factories {
    public class VerticeFactory {
        public const double VERTICE_DIAMETER = 25;
        public const double VERTICE_RADIUS = 12.5;

        public static readonly VerticeFactory Instance = new VerticeFactory();

        private VerticeFactory() { }

        public static void SubscribeGrafo(GrafoControl control) {
            control.MouseDoubleClick += Instance._MouseDoubleClick;
        }

        public static void UnsubscribeGrafo(GrafoControl control) {
            control.MouseDoubleClick -= Instance._MouseDoubleClick;
        }

        private void _MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton != MouseButton.Left)
                return;

            if(sender is GrafoControl control) {
                HitTestResult htr = VisualTreeHelper.HitTest(control, e.GetPosition((UIElement)sender));
                if(htr != null)
                    if(htr.VisualHit != control.canvas)
                        return;

                VerticeControl vertice = new VerticeControl() {
                    Width = VERTICE_DIAMETER,
                    Height = VERTICE_DIAMETER,
                    Draggable = true,

                    Location = e.GetPosition(control.canvas),
                    Key = FindChar(control).ToString(),
                };
                control.AddVertice(vertice);
                UndoService.Instance.RegisterAction(new UndoAddVertice(vertice));
            }
        }

        public static char FindChar<V, A>(IGrafo<V, A> grafo) where V : IVertice<V, A> where A : IAresta<V, A> {
            List<char> chars = new List<char>();
            foreach(V vertice in grafo.Vertices)
                if(vertice.Key.Length == 1)
                    chars.Add(vertice.Key[0]);

            char result;
            for(result = 'A'; result <= 'Z'; result++)
                if(!chars.Contains(result))
                    return result;
            for(result = '0'; result <= '9'; result++)
                if(!chars.Contains(result))
                    return result;
            for(result = 'a'; result <= 'z'; result++)
                if(!chars.Contains(result))
                    return result;
            return '-';
        }
    }
}
