using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

namespace Roguelike.Script.Actor.Stats;

public partial class ActorStatCollection : Node
{
	public Array<ActorStat> Stats = new Array<ActorStat>();
}