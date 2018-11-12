using Nhafo.Code.Factories;
using Nhafo.WPF.Controls;
using Nhafo.WPF.Dialogs;
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
        private List<IGrafoStage> stages;

        public PrimAlgorithm(GrafoControl grafo) {
            if(grafo == null)
                throw new ArgumentNullException("grafo");

            this.grafo = grafo;
        }

        public async Task<GrafoControl> Generate(VerticeControl startAt, bool showDialog = true) {
            GrafoControl grafo = GrafoFactory.Clone(this.grafo);
            GrafoControl grafoControl = GrafoFactory.Create();

            if(grafo.Vertices.Count == 0)
                return grafoControl;

            connected = new List<VerticeControl>();
            arestas = new List<ArestaControl>();
            stages = new List<IGrafoStage>();

            VerticeControl v;

            connected.Add(grafo.Vertices[IndexOf(this.grafo, startAt)]);
            Work(grafo);

            Dictionary<VerticeControl, VerticeControl> verticeDictionary = new Dictionary<VerticeControl, VerticeControl>();

            foreach(VerticeControl vertice in grafo.Vertices) {
                v = vertice.Clone();
                verticeDictionary.Add(vertice, v);

                if(vertice == startAt) {
                    v.Color = Colors.Gold;
                    grafoControl.AddVertice(v);
                }
                else {
                    grafoControl.AddVertice(v);
                }
            }

            GrafoFactory.CenterGrafoContent(grafoControl);

            foreach(ArestaControl aresta in grafo.Arestas)
                grafoControl.AddAresta(
                    new ArestaControl() {
                        VerticeA = verticeDictionary[aresta.VerticeA],
                        VerticeB = verticeDictionary[aresta.VerticeB],
                        Weight = aresta.Weight
                    });

            /*if(showDialog) {
                bool result = await AnimatedGrafoDialog.Show(grafoControl, stages, "Algoritmo de Prim");
                if(!result)
                    return null;
            }*/

            return grafoControl;
        }

        private void Work(GrafoControl grafo) {
            double lighter = double.MaxValue;
            ArestaControl lighterAresta = null;
            VerticeControl vertice = null;

            foreach(VerticeControl v in connected) {
                stages.Add(new SelectVerticeStage(v));

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

            ArestaControl aresta;
            for(int i = 0; i < grafo.Arestas.Count; i++) {
                aresta = grafo.Arestas[i];
                if(aresta.ContainsVertice(vertice) && aresta != lighterAresta) {
                    grafo.RemoveAresta(aresta);
                    i--;
                }
            }

            connected.Add(vertice);
            arestas.Add(lighterAresta);

            Work(grafo);
        }

        private int IndexOf(GrafoControl grafo, VerticeControl control) {
            for(int i = 0; i < grafo.Vertices.Count; i++)
                if(grafo.Vertices[i] == control)
                    return i;
            return -1;
        }
    }
}
