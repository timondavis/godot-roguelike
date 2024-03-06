using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Form.CharacterValues;

public partial class MobStatValues : CharacterStatValues
{
	private string _mobName;
	
	public override void SaveStatValues() {
		GameServices.Instance.Stats.SaveMobStatValues(_mobName, StatValues);
	}
	
	public void InitializeMobForm(string mobName)
	{
		_mobName = mobName;
		StatValues = GameServices.Instance.Stats.LoadMobStats(_mobName);
		PopulateStatValues(StatValues);	
	}
} 
