using Godot;
using Roguelike.Script.Actor.Stats;
using Roguelike.Script.Form.CharacterValues;
using Roguelike.Script.Game;
using Roguelike.Script.Game.Service;
using Roguelike.Script.Game.Service.Configuration;

namespace Roguelike.Script.Actor.Character;

public partial class Mob : Character
{
	protected ActorStatValues MobTemplateStats;
	
	public ActorStatValues MobCurrentStats;

	[Export] public MobStatValues Form;
	
	private StatsConfigurationService _statsService;
	
	public Mob() : base()
	{
		_statsService = GameServices.Instance.Stats;
	}
	
	[Export] 
	public MobStrategy Strategy { get; set; }

	public override void _Ready()
	{
		MobCurrentStats = _statsService.LoadMobStats(ActorName);
		MobTemplateStats = _statsService.LoadMobStats(ActorName);
		Form.InitializeMobForm(ActorName);
	}
}
