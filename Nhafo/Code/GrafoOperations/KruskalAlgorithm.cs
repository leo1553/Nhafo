using Nhafo.Code.Factories;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nhafo.Code.GrafoOperations {
    public class KruskalAlgorithm {
        private readonly GrafoControl grafo;
        private List<List<VerticeControl>> componentesConexas;
        private ArestaControl[] arestas;

        public KruskalAlgorithm(GrafoControl grafo) {
            if(grafo == null)
                throw new ArgumentNullException("grafo");

            this.grafo = grafo;
        }

        public GrafoControl Generate() {
            GrafoControl result = GrafoFactory.Clone(grafo);

            if(grafo.Arestas.Count == 0)
                return result;

            componentesConexas = new List<List<VerticeControl>>();
            arestas = result.Arestas.OrderBy(x => x.Weight).ToArray();

            Work(result, 0);

            /*bool delete;
            for(int i = 0; i < arestas.Count; i++) {
                delete = true;
                if(!connected.Contains(arestas[i].VerticeA)) {
                    connected.Add(arestas[i].VerticeA);
                    delete = false;
                }
                if(!connected.Contains(arestas[i].VerticeB)) {
                    connected.Add(arestas[i].VerticeB);
                    delete = false;
                }
                if(delete) {
                    result.RemoveAresta(arestas[i]);
                    arestas.RemoveAt(i--);
                }
            }*/

            GrafoFactory.CenterGrafoContent(result);
            return result;
        }

        private void Work(GrafoControl grafo, int index) {
            if(index >= arestas.Length)
                return;

            ArestaControl aresta = arestas[index];

            int[] componentes = FindComponenteConexaID(aresta.VerticeA, aresta.VerticeB);

            bool mesmaComponente = componentes[0] == componentes[1];
            bool aSemComponente = componentes[0] == -1;
            bool bSemComponente = componentes[1] == -1;


            // Mesma Componente
            if(!aSemComponente && mesmaComponente) {
                grafo.RemoveAresta(aresta);
            }

            // Sem Componente
            else if(aSemComponente && mesmaComponente) {
                List<VerticeControl> novaComponente = new List<VerticeControl>() { aresta.VerticeA, aresta.VerticeB };
                componentesConexas.Add(novaComponente);
            }

            // Componentes Diferentes
            else {
                if(aSemComponente) {
                    componentesConexas[componentes[1]].Add(aresta.VerticeA);
                }
                else if(bSemComponente) {
                    componentesConexas[componentes[0]].Add(aresta.VerticeB);
                }
                else {
                    componentesConexas[componentes[0]].AddRange(componentesConexas[componentes[1]]);
                    componentesConexas.RemoveAt(componentes[1]);
                }
            }

            Work(grafo, index + 1);
        }

        private int FindComponenteConexaID(VerticeControl vertice) {
            for(int i = 0; i < componentesConexas.Count; i++) { 
                foreach(VerticeControl v in componentesConexas[i]) {
                    if(v == vertice)
                        return i;
                }
            }
            return -1;
        }

        private int[] FindComponenteConexaID(VerticeControl verticeA, VerticeControl verticeB) {
            int[] result = { -1, -1 };
            for(int i = 0; i < componentesConexas.Count; i++) {
                foreach(VerticeControl v in componentesConexas[i]) {
                    if(v == verticeA) {
                        result[0] = i;

                        if(result[0] != -1 && result[1] != -1)
                            return result;
                    }
                    else if(v == verticeB) {
                        result[1] = i;

                        if(result[0] != -1 && result[1] != -1)
                            return result;
                    }
                }
            }
            return result;
        }
    }
}
