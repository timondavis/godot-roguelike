using Roguelike.Game.Service;

namespace Roguelike.Form.Script.CharacterValues;

public partial class MobStatValues : CharacterStatValues
{
	private string _mobName;
	
	public override void on_save_button_pressed() {
		GameServices.Instance.Stats.SaveMobStatValues(_mobName, StatValues);
	}
	
	public void InitializeMobForm(string mobName)
	{
		_mobName = mobName;
		PopulateStatValues(_mobName);	
	}
} 
