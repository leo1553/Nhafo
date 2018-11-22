using Nhafo.Code.Factories;
using Nhafo.Code.Utils;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nhafo.Code.GrafoOperations {
    public class DijkstraAlgorithm {
        private readonly GrafoControl grafo;
        
        private Dictionary<VerticeControl, DijkstraVertice> vertices;
        private VerticeControl source;
        private VerticeControl destiny;

        public DijkstraAlgorithm(GrafoControl grafo) {
            if(grafo == null)
                throw new ArgumentNullException("grafo");

            this.grafo = grafo;
        }

        public GrafoControl Generate(VerticeControl source, VerticeControl destiny) {
            GrafoControl result = GrafoFactory.Clone(grafo);

            if(grafo.Vertices.Count == 0)
                return result;

            this.source = result.Vertices[grafo.Vertices.IndexOf(source)];
            this.destiny = result.Vertices[grafo.Vertices.IndexOf(destiny)];
            vertices = new Dictionary<VerticeControl, DijkstraVertice>();
            foreach(VerticeControl vertice in result.Vertices)
                vertices.Add(vertice, new DijkstraVertice(vertice, vertice == this.source));

            Work(0);

            while(result.Arestas.Count != 0)
                result.RemoveAresta(result.Arestas[0]);

            VerticeControl current = this.destiny;
            DijkstraVertice currentData;
            while(true) {
                currentData = vertices[current];
                if(currentData.Previous == null)
                    break;

                result.AddAresta(new ArestaControl() {
                    VerticeA = current,
                    VerticeB = currentData.Previous,
                });

                current = currentData.Previous;
            }
            return result;
        }

        private void Work(int index) {
            if(index >= vertices.Count)
                return;

            DijkstraVertice data = vertices.ElementAt(index).Value;
            VerticeControl vertice = data.Control;

            data.Processed = true;

            // Pegar vertice de menor distancia
            VerticeControl vizinho;
            DijkstraVertice vizinhoData;

            VerticeControl menorDistancia;
            double distancia = double.MaxValue;
            foreach(ArestaControl aresta in vertice.Arestas) {
                vizinho = aresta.GetOposite(vertice);
                if(vertices[vizinho].Processed)
                    continue;

                if(aresta.Weight < distancia) {
                    distancia = aresta.Weight;
                    menorDistancia = aresta.GetOposite(vertice);
                }
            }

            double alt;
            foreach(ArestaControl aresta in vertice.Arestas) {
                vizinho = aresta.GetOposite(vertice);
                if(vertices[vizinho].Processed)
                    continue;

                vizinhoData = vertices[vizinho];

                alt = distancia + aresta.Weight;
                if(alt < vizinhoData.Distance) {
                    vizinhoData.Distance = alt;
                    vizinhoData.Previous = vertice;
                }
            }

            Work(index + 1);
        }

        private class DijkstraVertice {
            public VerticeControl Control { get; private set; }

            public double Distance { get; set; } = double.PositiveInfinity;
            public VerticeControl Previous { get; set; } = null;

            public bool Processed { get; set; } = false;

            public DijkstraVertice(VerticeControl vertice, bool isSource) {
                Control = vertice;

                if(isSource)
                    Distance = 0;
            }

            public static implicit operator VerticeControl(DijkstraVertice vertice) {
                return vertice.Control;
            }
        }
    }
}
