using Godot;
using Roguelike.Script.Form.ItemValues;
using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Actor.Character;

public partial class Item : Actor
{

	[Export]
	private ItemStatValuesForm Form { get; set; }
	
	public override void _Ready()
	{
		Form.InitializeItemForm(ActorName);
	}
}
