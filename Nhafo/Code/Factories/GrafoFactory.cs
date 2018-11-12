using Nhafo.Code.Models;
using Nhafo.Code.Utils;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Nhafo.Code.Factories {
    public class GrafoFactory {
        public static readonly GrafoFactory Instance = new GrafoFactory();

        public int GrafoID { get; set; } = 1;

        private GrafoFactory() { }

        public static GrafoControl CreateCenter(CartesianPlane plane, Size size) {
            return Instance._CreateCenter(plane, size);
        }
        private GrafoControl _CreateCenter(CartesianPlane plane, Size size) {
            GrafoControl control = new GrafoControl() {
                Location = new Point((plane.ActualWidth - size.Width) / 2, (plane.ActualHeight - size.Height) / 2).Sum(plane.Origin),
                Key = "Grafo " + GrafoID++,
                Width = size.Width,
                Height = size.Height
            };
            VerticeFactory.SubscribeGrafo(control);
            plane.Children.Add(control);

            MainWindow.Instance.GrafoControlsChanged(control, true);
            return control;
        }

        public static GrafoControl Create() => Instance._Create();
        private GrafoControl _Create() {
            GrafoControl control = new GrafoControl() {
                Key = "Grafo " + GrafoID++,
            };
            VerticeFactory.SubscribeGrafo(control);
            return control;
        }

        /*public static GrafoControl Create(IGrafo<VerticeControl, ArestaControl> grafo) {
            return Instance._Create(grafo);
        }
        private GrafoControl _Create(IGrafo<VerticeControl, ArestaControl> grafo) {
            Size size = MeasureGrafo(grafo);
            GrafoControl control = new GrafoControl() {
                Width = size.Width,
                Height = size.Height
            };

            foreach(VerticeControl v in grafo.Vertices)
                control.AddVertice(v);
            foreach(ArestaControl a in grafo.Arestas)
                control.AddAresta(a);

            VerticeFactory.SubscribeGrafo(control);
            return control;
        }*/

        public static Size MeasureGrafo<V, A>(IGrafo<V, A> grafo) where V : IVertice<V, A> where A : IAresta<V, A> {
            //double minX = 0;
            //double minY = 0;
            double maxX = 200;
            double maxY = 100;

            foreach(V v in grafo.Vertices) {
                //minX = Math.Min(minX, v.Location.X);
                //minY = Math.Min(minY, v.Location.Y);
                maxX = Math.Max(maxX, v.Location.X + VerticeFactory.VERTICE_DIAMETER);
                maxY = Math.Max(maxY, v.Location.Y + VerticeFactory.VERTICE_DIAMETER);
            }

            return new Size(maxX, maxY);
        }

        public static void CenterGrafoContent(GrafoControl grafo) {
            if(grafo.Vertices.Count == 0)
                return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach(VerticeControl v in grafo.Vertices) {
                minX = Math.Min(minX, v.Location.X);
                minY = Math.Min(minY, v.Location.Y);
                maxX = Math.Max(maxX, v.Location.X + VerticeFactory.VERTICE_DIAMETER);
                maxY = Math.Max(maxY, v.Location.Y + VerticeFactory.VERTICE_DIAMETER);
            }

            minX -= VerticeFactory.VERTICE_DIAMETER;
            minY -= VerticeFactory.VERTICE_DIAMETER;

            foreach(VerticeControl v in grafo.Vertices) 
                v.Location = new Point(v.Location.X - minX, v.Location.Y - minY);

            maxX -= minX;
            maxY -= minY;

            if(maxX < grafo.MinWidth) {
                Point p = new Point(((grafo.MinWidth - maxX - VerticeFactory.VERTICE_RADIUS) * .5), 0);
                foreach(VerticeControl v in grafo.Vertices)
                    v.Location = v.Location.Sum(p);
            }

            grafo.Width = maxX + VerticeFactory.VERTICE_RADIUS;
            grafo.Height = maxY + VerticeFactory.VERTICE_DIAMETER;
        }

        public static GrafoControl Clone(GrafoControl sourceGrafo) {
            Dictionary<VerticeControl, VerticeControl> vertices = new Dictionary<VerticeControl, VerticeControl>();

            GrafoControl grafo = Create();
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
                grafo.AddAresta(aresta);
            }

            CenterGrafoContent(grafo);
            return grafo;
        }

        public static string FindName(IReadOnlyList<GrafoControl> grafos) {
            List<string> usedNames = new List<string>(grafos.Select(x => x.Key));

            int i = 1;
            string current;
            while(i != int.MaxValue) {
                current = "Grafo " + i;
                if(usedNames.Contains(current))
                    continue;
                return current;
            }
            return "Wuut";
        }
    }
}
