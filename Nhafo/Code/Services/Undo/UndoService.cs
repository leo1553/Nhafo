using System.Collections.Generic;

namespace Nhafo.Code.Services.Undo {
    public class UndoService {
        public static readonly UndoService Instance = new UndoService(200);

        public int MaxLength { get; set; }

        List<IUndoableAction> toUndo = new List<IUndoableAction>();
        public IReadOnlyCollection<IUndoableAction> UndoActions => toUndo;

        List<IUndoableAction> toRedo = new List<IUndoableAction>();
        public IReadOnlyCollection<IUndoableAction> RedoActions => toRedo;

        public UndoService(int maxLength = 50) {
            MaxLength = maxLength;
        }

        public void RegisterAction(IUndoableAction action) {
            _RegisterAction(action);
            toRedo.Clear();
        }
        private void _RegisterAction(IUndoableAction action) {
            toUndo.Insert(0, action);
            if(toUndo.Count > MaxLength)
                toUndo.RemoveAt(toUndo.Count - 1);
        }

        public void Undo() {
            if(toUndo.Count == 0)
                return;
            
            IUndoableAction action = toUndo[0];
            toUndo.RemoveAt(0);
            toRedo.Insert(0, action);

            if(action.Target != null) {
                action.Target.ProcessingUndo = true;
                action.ExecuteUndo();
                action.Target.ProcessingUndo = false;
            }
            else
                action.ExecuteUndo();
        }

        public void Redo() {
            if(toRedo.Count == 0)
                return;

            IUndoableAction action = toRedo[0];
            toRedo.RemoveAt(0);
            _RegisterAction(action);

            if(action.Target != null) {
                action.Target.ProcessingUndo = true;
                action.ExecuteRedo();
                action.Target.ProcessingUndo = false;
            }
            else
                action.ExecuteRedo();
        }
    }
}
