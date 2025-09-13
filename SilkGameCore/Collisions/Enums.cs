namespace SilkGameCore.Collisions
{
    /// <summary>
    /// Defines the intersection between a <see cref="Plane"/> and a bounding volume.
    /// </summary>
    public enum PlaneIntersectionType
    {
        Front,
        Back,
        Intersecting
    }
    /// <summary>
    /// Defines how the bounding volumes intersects or contain one another.
    /// </summary>
    public enum ContainmentType
    {
        Disjoint,
        Contains,
        Intersects
    }
}
