namespace Iso.Cells
{
    public enum CellType
    {
        Blocked, // cant build neither pass
        Buildable, // can build and pass
        Traversable // can't build, pass only
    }
}