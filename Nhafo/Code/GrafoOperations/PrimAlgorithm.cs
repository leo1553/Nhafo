using Nhafo.Code.Factories;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Nhafo.Code.GrafoOperations {
    public class PrimAlgorithm {
        private readonly GrafoControl grafo;
        private List<VerticeControl> connected;
        private List<ArestaControl> arestas;

        public PrimAlgorithm(GrafoControl grafo) {
            if(grafo == null)
                throw new ArgumentNullException("grafo");

            this.grafo = grafo;
        }

        public GrafoControl Generate() {
            GrafoControl grafoControl = GrafoFactory.Create();

            if(grafo.Vertices.Count == 0)
                return grafoControl;

            int random = new Random().Next(grafo.Vertices.Count);
            VerticeControl verticeInicial = grafo.Vertices[random];

            connected = new List<VerticeControl>();
            arestas = new List<ArestaControl>();
            
            connected.Add(verticeInicial);
            Work();

            foreach(VerticeControl vertice in grafo.Vertices) {
                if(vertice == verticeInicial) {
                    VerticeControl v = vertice.Clone();
                    v.Color = Colors.Green;
                    grafoControl.AddVertice(v);
                }
                else {
                    grafoControl.AddVertice(vertice.Clone());
                }
            }

            foreach(ArestaControl aresta in arestas)
                grafoControl.AddAresta(
                    new ArestaControl() {
                        VerticeA = grafoControl.Vertices[IndexOf(aresta.VerticeA)],
                        VerticeB = grafoControl.Vertices[IndexOf(aresta.VerticeB)],
                        Weight = aresta.Weight
                    });

            GrafoFactory.CenterGrafoContent(grafoControl);
            return grafoControl;
        }

        private void Work() {
            double lighter = double.MaxValue;
            ArestaControl lighterAresta = null;
            VerticeControl vertice = null;

            foreach(VerticeControl v in connected) {
                foreach(ArestaControl a in v.Arestas) {
                    if(!connected.Contains(a.GetOposite(v))) {
                        if(a.Weight < lighter) {
                            lighter = a.Weight;
                            lighterAresta = a;
                            vertice = a.GetOposite(v);
                        }
                    }
                }
            }

            if(lighterAresta == null)
                return;

            arestas.Add(lighterAresta);
            connected.Add(vertice);

            Work();
        }

        private int IndexOf(VerticeControl control) {
            for(int i = 0; i < grafo.Vertices.Count; i++)
                if(grafo.Vertices[i] == control)
                    return i;
            return -1;
        }
    }
}
