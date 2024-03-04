using Godot;
using Roguelike.Form.Script.CharacterValues;
using Roguelike.Game;
using Roguelike.Game.Service;
using Roguelike.Game.Service.Configuration;

namespace Roguelike.Actor.Character;

public partial class Mob : Character
{
	private StatsConfigurationService _statsService;
	private MobStatValues _statsValues;

	public Mob() : base()
	{
		_statsService = GameServices.Instance.Stats;
	}
	
	[Export] 
	public MobStrategy Strategy { get; set; }

	public override void _Ready()
	{
		MobStatValues childNode = GetNode<MobStatValues>("StatForm");
		childNode.InitializeMobForm(ActorName);
	}
}
