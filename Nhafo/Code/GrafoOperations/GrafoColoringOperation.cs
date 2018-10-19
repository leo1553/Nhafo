using Nhafo.Code.Factories;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nhafo.Code.GrafoOperations {
    public abstract class GrafoColoringOperation {
        public abstract byte MaxColors { get; }

        protected readonly GrafoControl grafo;
        protected readonly List<Vertice> verticeList;

        protected bool? CanBeDoneResult { get; private set; } = null;

        public GrafoColoringOperation(GrafoControl grafo) {
            this.grafo = grafo;
            verticeList = new List<Vertice>(grafo.Vertices.Count);
            verticeList.AddRange(grafo.Vertices.Select(x => new Vertice(this, x)));
            verticeList.ForEach(x => x.SetConnectedVertices());
        }

        public bool CanBeDone() {
            Stack<Vertice> stack = new Stack<Vertice>();
            try {
                Work(verticeList[0], stack);
                CanBeDoneResult = true;
                return true;
            }
            catch {
                CanBeDoneResult = false;
                return false;
            }
        }

        protected Vertice Find(VerticeControl control) {
            return verticeList.Find(x => x.Control == control);
        }

        protected int FindIndex(VerticeControl control) {
            return verticeList.FindIndex(x => x.Control == control);
        }

        protected void Work(Vertice vertice, Stack<Vertice> stack) {
            stack.Push(vertice);

            if(vertice.IsClear) {
                bool[] colors = new bool[MaxColors];

                foreach(Vertice connectedVertice in vertice.ConnectedVertices) {
                    if(connectedVertice.IsSet)
                        colors[connectedVertice.ColorId] = true;
                }

                byte firstAvaliableColor = Vertice.DEFAULT_COLOR_ID;
                for(byte i = 0; i < MaxColors; i++) {
                    if(!colors[i]) {
                        firstAvaliableColor = i;
                        break;
                    }
                }

                if(firstAvaliableColor == Vertice.DEFAULT_COLOR_ID)
                    throw new Exception("Invalid vertice set.");

                vertice.ColorId = firstAvaliableColor;
            }

            if(!vertice.IsAllConnectedVerticesColorSet) {
                foreach(Vertice connectedVertice in vertice.ConnectedVertices) {
                    if(!stack.Contains(connectedVertice) && (connectedVertice.IsClear || !connectedVertice.IsAllConnectedVerticesColorSet))
                        Work(connectedVertice, stack);
                }
            }
            stack.Pop();
        }

        public abstract GrafoControl Color();

        protected class Vertice {
            public const byte DEFAULT_COLOR_ID = byte.MaxValue;

            private readonly GrafoColoringOperation context;

            public VerticeControl Control { get; private set; }
            public Vertice[] ConnectedVertices { get; private set; }
            public byte ColorId { get; set; }
            
            public bool IsAllConnectedVerticesColorSet {
                get {
                    foreach(Vertice connectedVertice in ConnectedVertices) {
                        if(connectedVertice.IsClear)
                            return false;
                    }
                    return true;
                }
            }

            public bool IsClear => ColorId == DEFAULT_COLOR_ID;
            public bool IsSet => ColorId != DEFAULT_COLOR_ID;

            public Vertice(GrafoColoringOperation context, VerticeControl vertice) {
                this.context = context;
                Control = vertice;
                ColorId = DEFAULT_COLOR_ID;
            }

            public void SetConnectedVertices() {
                IReadOnlyList<VerticeControl> connectedVertices = Control.ConnectedVertices;
                ConnectedVertices = new Vertice[connectedVertices.Count];

                Vertice v;
                for(int i = 0; i < connectedVertices.Count; i++) {
                    v = context.Find(connectedVertices[i]);
                    ConnectedVertices[i] = v;
                }
            }
        }
    }
}
