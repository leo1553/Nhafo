using Nhafo.Code.Factories;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Nhafo.Code.GrafoOperations {
    public class ComponenteConexa {
        private readonly GrafoControl grafo;
        private List<VerticeControl> connected;
        private List<ArestaControl> arestas;

        public ComponenteConexa(GrafoControl grafo) {
            if (grafo == null)
                throw new ArgumentNullException("grafo");

            this.grafo = grafo;
        }

        public GrafoControl Generate(VerticeControl startAt) {
            GrafoControl grafoControl = GrafoFactory.Create();

            if (grafo.Vertices.Count == 0)
                return grafoControl;

            connected = new List<VerticeControl>();
            arestas = new List<ArestaControl>();

            connected.Add(startAt);
            Work(startAt);

            foreach(VerticeControl vertice in grafo.Vertices) {
                if(!connected.Contains(vertice)) {
                    connected.Add(vertice);
                    Work(vertice);
                }
            }

            foreach(VerticeControl vertice in grafo.Vertices) {
                if(vertice == startAt) {
                    VerticeControl v = vertice.Clone();
                    v.Color = Colors.Gold;
                    grafoControl.AddVertice(v);
                }
                else {
                    grafoControl.AddVertice(vertice.Clone());
                }
            }

            foreach (ArestaControl aresta in arestas)
                grafoControl.AddAresta(
                    new ArestaControl() {
                        VerticeA = grafoControl.Vertices[IndexOf(aresta.VerticeA)],
                        VerticeB = grafoControl.Vertices[IndexOf(aresta.VerticeB)],
                        Weight = aresta.Weight
                    });

            GrafoFactory.CenterGrafoContent(grafoControl);
            return grafoControl;
        }

        private void Work(VerticeControl vertice) { 
            List<VerticeControl> newConnectedVertices = new List<VerticeControl>();
            VerticeControl v;
            foreach(ArestaControl a in vertice.Arestas) {
                v = a.GetOposite(vertice);
                if (!connected.Contains(v)) {
                    newConnectedVertices.Add(v);

                    connected.Add(v);
                    arestas.Add(a);
                }
            }

            foreach (VerticeControl connectedVertice in newConnectedVertices) {
                Work(connectedVertice);
            }
        }

        private int IndexOf(VerticeControl control) {
            for (int i = 0; i < grafo.Vertices.Count; i++)
                if (grafo.Vertices[i] == control)
                    return i;
            return -1;
        }
    }
}
