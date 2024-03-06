using Godot;
using Roguelike.Script.Form.CharacterValues;
using Roguelike.Script.Game;
using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Actor.Character;

public partial class Mob : Character
{
	[Export] public MobStatValuesForm Form;
	
	[Export] 
	public MobStrategy Strategy { get; set; }

	public override void _Ready()
	{
		var statsService = GameServices.Instance.Stats;
		CurrentStats = statsService.LoadMobStats(ActorName);
		TemplateStats = statsService.LoadMobStats(ActorName);
		Form.InitializeMobForm(ActorName);
	}
}
