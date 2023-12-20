using System;
using Godot;

namespace Roguelike.Map.Model;

public class TileAddress : Tuple<int, Vector2I>
{
    public int AtlasId
    {
        get
        {
            return Item1;
        }
    }

    public Vector2I Position
    {
        get
        {
            return Item2;
        }
    }
    
    public TileAddress(int atlasId, Vector2I position) : base(atlasId, position)
    {
    }
}