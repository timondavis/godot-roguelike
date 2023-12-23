using System.Collections.Generic;

namespace Roguelike.Map.Model;

public class TileTypeList : List<Roguelike.Map.Model.TileType>
{
	/// <summary>
	/// Finds a TileType by its name.
	/// </summary>
	/// <param name="name">The name of the TileType to find.</param>
	/// <returns>The TileType with the specified name, or null if not found.</returns>
	public Roguelike.Map.Model.TileType FindByName(string name)
	{
		return this.Find(type => type.Name == name);
	}
}
