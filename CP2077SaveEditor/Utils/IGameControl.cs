namespace CP2077SaveEditor.Utils;

public interface IGameControl
{
    public string GameControlName { get; }

    /// <summary>
    /// Glyph of the Windows icon font (Segoe Fluent Icons / Segoe MDL2 Assets) shown next to the name in the sidebar.
    /// </summary>
    public string Icon => null;
}