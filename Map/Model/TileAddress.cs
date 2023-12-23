using System;
using Godot;

public class TileAddress : Tuple<int, Vector2I>
{
    /// <summary>
    /// Gets the AtlasId of the tile.
    /// </summary>
    /// <remarks>
    /// This property returns the AtlasId associated with the tile.
    /// </remarks>
    public int AtlasId
    {
        get
        {
            return Item1;
        }
    }

    /// <summary>
    /// Gets the position of the tile.
    /// </summary>
    /// <value>
    /// The position of the object within the TileSet's Atlas.
    /// </value>
    public Vector2I Position
    {
        get
        {
            return Item2;
        }
    }

    /// <summary>
    /// Represents a tile address with an atlas ID and its position on the Atlas's 2D grid.
    /// </summary>
    /// <param name="atlasId">The ID of the atlas that the tile belongs to.</param>
    /// <param name="position">The position of the tile in 2D space.</param>
    public TileAddress(int atlasId, Vector2I position) : base(atlasId, position)
    {
    }
}