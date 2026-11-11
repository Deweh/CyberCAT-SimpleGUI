using System;
using System.Collections.Generic;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Serialized states of the save, oldest first, for undo/redo. A state is the whole save (~4 MB), already compressed
    /// by the save format, so only a limited number is kept.
    /// </summary>
    public class UndoHistory
    {
        private readonly List<byte[]> _states = new();
        private readonly int _maxUndoSteps;
        private int _current = -1;

        public UndoHistory(int maxUndoSteps)
        {
            _maxUndoSteps = Math.Max(0, maxUndoSteps);
        }

        public bool IsEnabled => _maxUndoSteps > 0;
        public bool CanUndo => _current > 0;
        public bool CanRedo => _current >= 0 && _current < _states.Count - 1;

        /// <summary>
        /// Starts over from the state of a freshly loaded save, or clears the history with null.
        /// </summary>
        public void Reset(byte[] initialState)
        {
            _states.Clear();
            _current = -1;
            if (initialState != null && IsEnabled)
            {
                _states.Add(initialState);
                _current = 0;
            }
        }

        /// <summary>
        /// Records the state after an edit. Anything that could be redone is dropped, like in any editor.
        /// </summary>
        public void Push(byte[] state)
        {
            if (_current < 0)
            {
                return;
            }

            _states.RemoveRange(_current + 1, _states.Count - _current - 1);
            _states.Add(state);
            _current++;

            // The oldest state stays reachable as long as there is room
            while (_states.Count > _maxUndoSteps + 1)
            {
                _states.RemoveAt(0);
                _current--;
            }
        }

        public byte[] Undo() => CanUndo ? _states[--_current] : null;

        public byte[] Redo() => CanRedo ? _states[++_current] : null;
    }
}
