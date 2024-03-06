using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Form.CharacterValues;

public partial class PlayerStatValuesForm : CharacterStatValuesForm
{
	public override void SaveStatValues()
	{
		GameServices.Instance.Stats.SavePlayerStatValues(StatValues);
	}

	public void InitializePlayerForm()
	{
		StatValues = GameServices.Instance.Stats.LoadPlayerStats();
		PopulateStatValues(StatValues);	
	}
} 
