namespace Nhafo.Code.Models {
    public interface IAresta<V, A> where V : IVertice<V, A> where A : IAresta<V, A> {
        V VerticeA { get; }
        V VerticeB { get; }
        double Weight { get; }
        ArestaType Direction { get; }
        IGrafo<V, A> Grafo { get; }
        
        void VerticeLocationUpdated(V vertice);
        bool ContainsVertice(V vertice);
        V GetOposite(V vertice);
    }
}
