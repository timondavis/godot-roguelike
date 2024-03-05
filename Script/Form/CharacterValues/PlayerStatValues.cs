using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Form.CharacterValues;

public partial class PlayerStatValues : CharacterStatValues
{

	public override void on_save_button_pressed()
	{
		GameServices.Instance.Stats.SavePlayerStatValues(StatValues);
	}

	public void InitializePlayerForm()
	{
		StatValues = GameServices.Instance.Stats.LoadPlayerStats();
		PopulateStatValues(StatValues);	
	}
} 
