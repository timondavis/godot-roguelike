using System.Collections.Generic;
using Godot;
using Godot.Bridge;
using Roguelike.Actor.Stats;
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
}
