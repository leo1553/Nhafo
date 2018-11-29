using Nhafo.Code.Factories;
using Nhafo.Code.Utils;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Media;

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
                vertices.Add(vertice, new DijkstraVertice(vertice));

            vertices[this.source].Distance = 0;
            vertices[this.source].Parent = this.source;

            this.source.Color = Colors.Gold;
            this.destiny.Color = Colors.Red;

            foreach(ArestaControl a in this.source.Arestas) {
                vertices[a.GetOposite(this.source)].Distance = a.Weight;
                vertices[a.GetOposite(this.source)].Hook = this.source;
            }

            while(Work(result));

            Dictionary<Tuple<VerticeControl, VerticeControl>, double> weights = new Dictionary<Tuple<VerticeControl, VerticeControl>, double>();

            while(result.Arestas.Count != 0) {
                weights.Add(
                    new Tuple<VerticeControl, VerticeControl>(result.Arestas[0].VerticeA, result.Arestas[0].VerticeB),
                    result.Arestas[0].Weight);

                result.RemoveAresta(result.Arestas[0]);
            }
            
            VerticeControl current = this.destiny;
            DijkstraVertice currentData;
            while(true) {
                currentData = vertices[current];
                if(currentData.Parent == current)
                    break;

                result.AddAresta(new ArestaControl() {
                    VerticeA = current,
                    VerticeB = currentData.Parent,
                    Weight = FindArestaWeight(weights, current, currentData.Parent)
                });

                current = currentData.Parent;
            }
            return result;
        }

        private bool Work(GrafoControl grafo) {
            double minDistance = double.PositiveInfinity;
            VerticeControl nearest = null;
            VerticeControl aux = null;
            DijkstraVertice nearestData;
            DijkstraVertice auxData;

            foreach(VerticeControl z in grafo.Vertices) {
                nearestData = vertices[z];
                if(nearestData.Parent == null && nearestData.Distance < minDistance) {
                    minDistance = nearestData.Distance;
                    nearest = z;
                }
            }
            if(double.IsPositiveInfinity(minDistance)) 
                return false;

            nearestData = vertices[nearest];
            nearestData.Parent = nearestData.Hook;
            
            foreach(ArestaControl a in nearest.Arestas) {
                aux = a.GetOposite(nearest);
                auxData = vertices[aux];
                if(nearestData.Distance + a.Weight < auxData.Distance) {
                    auxData.Distance = nearestData.Distance + a.Weight;
                    auxData.Hook = nearest;
                }
            }
            return true;
        }

        private double FindArestaWeight(Dictionary<Tuple<VerticeControl, VerticeControl>, double> weights,
            VerticeControl verticeA, VerticeControl verticeB) {

            foreach(Tuple<VerticeControl, VerticeControl> a in weights.Keys)
                if((a.Item1 == verticeA && a.Item2 == verticeB)
                || (a.Item2 == verticeA && a.Item1 == verticeB))
                    return weights[a];
            return double.NaN;
        }

        private class DijkstraVertice {
            public VerticeControl Control { get; private set; }

            public double Distance { get; set; } = double.PositiveInfinity;
            public VerticeControl Parent { get; set; } = null;
            public VerticeControl Hook { get; set; } = null;
            
            public bool Processed { get; set; } = false;

            public DijkstraVertice(VerticeControl vertice) {
                Control = vertice;
            }

            public static implicit operator VerticeControl(DijkstraVertice vertice) {
                return vertice.Control;
            }
        }
    }
}
