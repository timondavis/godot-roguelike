using Godot;
using Roguelike.Actor.Stats;
using Roguelike.Form.Script.CharacterValues;
using Roguelike.Game;
using Roguelike.Game.Service;
using Roguelike.Game.Service.Configuration;

namespace Roguelike.Actor.Character;

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
