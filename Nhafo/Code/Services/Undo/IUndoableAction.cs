namespace Nhafo.Code.Services.Undo {
    public interface IUndoableAction {
        IUndoTarget Target { get; }

        void ExecuteUndo();
        void ExecuteRedo();
    }
}
