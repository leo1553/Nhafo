using Nhafo.Code.Models;
using Nhafo.WPF.Controls;
using System.Windows;
using Nhafo.Code.Utils;
using System.Collections.Generic;

namespace Nhafo.Code.Services.Undo {
    public class UndoAddVertice : IUndoableAction {
        VerticeControl vertice;
        IGrafo<VerticeControl, ArestaControl> grafo;

        public IUndoTarget Target => vertice;

        public UndoAddVertice(VerticeControl vertice) {
            this.vertice = vertice;
            grafo = vertice.Grafo;
        }

        public void ExecuteUndo() {
            grafo.RemoveVertice(vertice);
        }

        public void ExecuteRedo() {
            grafo.AddVertice(vertice);
        }
    }


    public class UndoMoveVertice : IUndoableAction {
        VerticeControl vertice;
        Point fromLocation;
        Point toLocation;

        public IUndoTarget Target => vertice;

        public UndoMoveVertice(VerticeControl vertice, Point fromLocation, Point toLocation) {
            this.vertice = vertice;
            this.fromLocation = fromLocation;
            this.toLocation = toLocation;
        }

        public void ExecuteUndo() {
            vertice.Location = fromLocation;
        }

        public void ExecuteRedo() {
            vertice.Location = toLocation;
        }
    }

    public class UndoRemoveVertice : IUndoableAction {
        VerticeControl vertice;
        IGrafo<VerticeControl, ArestaControl> grafo;
        List<ArestaControl> arestas;

        public IUndoTarget Target => vertice;

        public UndoRemoveVertice(VerticeControl vertice) {
            this.vertice = vertice;
            grafo = vertice.Grafo;
            arestas = new List<ArestaControl>(vertice.Arestas);
            Debug.Log(arestas.Count);
        }

        public void ExecuteUndo() {
            grafo.AddVertice(vertice);
            foreach(ArestaControl aresta in arestas)
                grafo.AddAresta(aresta);
        }

        public void ExecuteRedo() {
            grafo.RemoveVertice(vertice);
        }
    }
}