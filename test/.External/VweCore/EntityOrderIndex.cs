namespace VweCore
{
    // The values of this enum will be used as array indexes for,
    // thus they must start at 0 and have no gaps in between values.
    public enum EntityOrderIndex
    {
        StorageRows = 0,
        Obstacles = 1,
        Reflectors = 2,
        StorageLocations = 3,
        Nodes = 4,
        NodeLinks = 5,
        VirtualPoints = 6,
        TemporaryItems = 99 // These represent diagram items that have no entity attached (these are usually temporary items like the perpendicular connector when a storage row is inserted using Drag & Drop)
    }
}