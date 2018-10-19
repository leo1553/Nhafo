using System.Collections.Generic;
using System.Windows;

namespace Nhafo.Code.Models {
    public interface IGrafo<V, A> where V : IVertice<V, A> where A : IAresta<V, A> {
        string Key { get; }
        IReadOnlyList<V> Vertices { get; }
        IReadOnlyList<A> Arestas { get; }
        Point Location { get; }
        ArestaType ArestaType { get; }

        bool AreConnected(V verticeA, V verticeB);

        void AddVertice(V vertice);
        void RemoveVertice(V vertice);
        void AddAresta(A aresta);
        void RemoveAresta(A aresta);
    }
}
