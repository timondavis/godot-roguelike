using Godot;
using System;

namespace Roguelike.Actor.Stats;

public partial class ActorStat : Node
{
	[Export] 
	public string StatkName { get; set; }
	
	public int Value { get; set; }
}
