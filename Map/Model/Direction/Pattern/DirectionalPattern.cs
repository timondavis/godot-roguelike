using System.Collections.Generic;

namespace Roguelike.Map.Model.Direction.Pattern;

public abstract class DirectionalPattern
{
    public HashSet<GridDirection> Pattern { get; protected set; }
}