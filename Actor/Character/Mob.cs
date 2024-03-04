using Godot;
using Roguelike.Form.Script;
using Roguelike.Game;
using Roguelike.Game.Service;
using Roguelike.Game.Service.Configuration;

namespace Roguelike.Actor.Character;

public partial class Mob : Character
{
	private StatsConfigurationService _statsService;

	public Mob() : base()
	{
		_statsService = GameServices.Instance().StatsConfiguration;
	}
	
	[Export] 
	public MobStrategy Strategy { get; set; }

	public override void _Ready()
	{
		MobStatValues childNode = GetNode<MobStatValues>("MobStatValuesForm");
		childNode.PopulateStatValues(ActorName);
	}
}
