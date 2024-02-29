using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

namespace Roguelike.Actor.Stats;

public partial class ActorStatCollection : Node
{
	[Export] public Array<ActorStat> Stats;
}
