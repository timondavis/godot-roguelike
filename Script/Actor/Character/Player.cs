using Godot;
using Roguelike.Script.Form.CharacterValues;
using Roguelike.Script.Game;
using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Actor.Character;

public partial class Player : Character
{
   [Export] 
   public PlayerController Controller { get; set;}
   
   [Export] public PlayerStatValuesForm Form;

   public override void _Ready()
   {
	   var statsService = GameServices.Instance.Stats;
	   CurrentStats = statsService.LoadMobStats(ActorName);
	   TemplateStats = statsService.LoadMobStats(ActorName);
	   Form.InitializePlayerForm();
   }
}
