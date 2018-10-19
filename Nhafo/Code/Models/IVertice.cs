using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Nhafo.Code.Models {
    public interface IVertice<V, A> where V : IVertice<V, A> where A : IAresta<V, A> {
        Point Location { get; }
        char Key { get; }
        IReadOnlyList<A> Arestas { get; }
        IGrafo<V, A> Grafo { get; }
        Color Color { get; }
        IReadOnlyList<V> ConnectedVertices { get; }

        void AddAresta(A aresta);
        void RemoveAresta(A aresta);
    }
}
