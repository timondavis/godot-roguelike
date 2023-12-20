using System.Collections.Generic;

namespace Roguelike.Map.Model;

public class TileTypeList : List<TileType>
{
    public TileType FindByName(string name)
    {
        return this.Find(type => type.Name == name);
    }
}