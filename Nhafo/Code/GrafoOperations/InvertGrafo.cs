using Nhafo.Code.Factories;
using Nhafo.Code.Utils;
using Nhafo.WPF.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Nhafo.Code.GrafoOperations {
    public class InvertGrafo {
        private readonly GrafoControl grafo;

        public InvertGrafo(GrafoControl grafo) {
            this.grafo = grafo;
        }

        public GrafoControl Invert() {
            GrafoControl grafoControl = GrafoFactory.Create();

            VerticeControl[] vertices = new VerticeControl[grafo.Vertices.Count];
            int i = 0, j;
            foreach(VerticeControl v in grafo.Vertices) {
                vertices[i] = v.Clone();
                //vertices[i].Color = v.Color.Inverse();

                grafoControl.AddVertice(vertices[i]);
                i++;
            }

            IReadOnlyList<VerticeControl> connectedVerticesA;
            IReadOnlyList<VerticeControl> connectedVerticesB;
            for(i = 0; i < vertices.Length; i++) {
                connectedVerticesA = grafo.Vertices[i].ConnectedVertices;
                connectedVerticesB = vertices[i].ConnectedVertices;
                for(j = 0; j < vertices.Length; j++) {
                    if(j == i)
                        continue;

                    if(!connectedVerticesA.Contains(grafo.Vertices[j]) && !connectedVerticesB.Contains(vertices[j])) {
                        grafoControl.AddAresta(new ArestaControl() {
                            VerticeA = vertices[i],
                            VerticeB = vertices[j]
                        });
                    }
                }
            }

            GrafoFactory.CenterGrafoContent(grafoControl);
            return grafoControl;
        }
    }
}
