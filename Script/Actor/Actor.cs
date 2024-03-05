using Godot;
using Roguelike.Script.Actor.Stats;
using Godot.Collections;

namespace Roguelike.Script.Actor;

public partial class Actor : Node
{
	[Export] 
	public Texture DetailedView { get; set; }
	
	[Export]
	public Texture WorldView { get; set; }
	
	[Export]
	public string ActorName { get; set; }

	public ActorStatCollection Stats { get; set; }
	
	public Dictionary<ActorStat, int> StatValues = new Dictionary<ActorStat, int>();
}
