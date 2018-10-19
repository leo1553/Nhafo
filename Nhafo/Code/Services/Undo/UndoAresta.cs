using Nhafo.Code.Models;
using Nhafo.WPF.Controls;
using System.Windows;
using System;

namespace Nhafo.Code.Services.Undo {
    public class UndoAddAresta : IUndoableAction {
        ArestaControl aresta;
        IGrafo<VerticeControl, ArestaControl> grafo;

        public IUndoTarget Target => aresta;

        public UndoAddAresta(ArestaControl aresta) {
            this.aresta = aresta;
            grafo = aresta.Grafo;
        }

        public void ExecuteUndo() {
            grafo.RemoveAresta(aresta);
        }

        public void ExecuteRedo() {
            grafo.AddAresta(aresta);
        }
    }

    public class UndoMoveAresta : IUndoableAction {
        ArestaControl aresta;
        Point fromLocation;
        Point toLocation;

        public IUndoTarget Target => aresta;

        public UndoMoveAresta(ArestaControl aresta, Point fromLocation, Point toLocation) {
            this.aresta = aresta;
            this.fromLocation = fromLocation;
            this.toLocation = toLocation;
        }

        public void ExecuteUndo() {
            aresta.MiddlePoint = fromLocation;
            aresta.VerticeLocationUpdated(null);
        }

        public void ExecuteRedo() {
            aresta.MiddlePoint = toLocation;
            aresta.VerticeLocationUpdated(null);
        }
    }

    public class UndoRemoveAresta : IUndoableAction {
        ArestaControl aresta;
        IGrafo<VerticeControl, ArestaControl> grafo;

        public IUndoTarget Target => aresta;

        public UndoRemoveAresta(ArestaControl aresta) {
            this.aresta = aresta;
            grafo = aresta.Grafo;
        }

        public void ExecuteUndo() {
            grafo.AddAresta(aresta);
        }

        public void ExecuteRedo() {
            grafo.RemoveAresta(aresta);
        }
    }

    public class UndoChangeArestaWeight : IUndoableAction {
        ArestaControl aresta;
        double from;
        double to;

        public IUndoTarget Target => aresta;

        public UndoChangeArestaWeight(ArestaControl aresta, double from, double to) {
            this.aresta = aresta;
            this.from = from;
            this.to = to;
        }

        public void ExecuteUndo() {
            aresta.Weight = from;
        }

        public void ExecuteRedo() {
            aresta.Weight = to;
        }
    }
}
