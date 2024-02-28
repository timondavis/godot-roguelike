using Godot;
using System;

namespace Roguelike.Actor.Stats;

public partial class ActorStat : Node
{
	[Export] 
	public string StatName { get; set; }

	[Export] 
	public int MinValue { get; set; }

	[Export]
	public int MaxValue { get; set; }
	
	[Export]
	public int DefaultValue { get; set; }
}
