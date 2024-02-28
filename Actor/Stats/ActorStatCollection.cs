using Godot;
using System;
using System.Collections.Generic;

namespace Roguelike.Actor.Stats;

public partial class ActorStatCollection : Node
{
    [Export] public List<ActorStat> Stats;
}
