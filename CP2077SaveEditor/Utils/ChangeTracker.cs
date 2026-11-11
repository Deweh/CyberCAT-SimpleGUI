using System;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Tracks whether the loaded save was edited since it was loaded or saved, for the "*" in the title.
    /// Edit actions call MarkChanged; hashing the whole save after every edit would take seconds.
    /// The hash comparison in MainForm stays the authority when closing or loading another save.
    /// </summary>
    public static class ChangeTracker
    {
        public static bool HasChanges { get; private set; }

        public static event EventHandler Changed;

        /// <summary>
        /// Raised for every edit, not only the first one, e.g. to record undo points.
        /// </summary>
        public static event EventHandler Edited;

        public static void MarkChanged()
        {
            Edited?.Invoke(null, EventArgs.Empty);

            if (HasChanges)
            {
                return;
            }

            HasChanges = true;
            Changed?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Called once the save is loaded or written, which also drops marks made while the pages initialized.
        /// </summary>
        public static void Reset()
        {
            HasChanges = false;
            Changed?.Invoke(null, EventArgs.Empty);
        }
    }
}
