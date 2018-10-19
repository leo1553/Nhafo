namespace Nhafo.Code.Services.Undo {
    public interface IUndoTarget {
        bool ProcessingUndo { get; set; }
    }
}
