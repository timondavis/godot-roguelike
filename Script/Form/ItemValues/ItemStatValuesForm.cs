using Roguelike.Script.Actor.Stats;
using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Form.ItemValues;

public partial class ItemStatValuesForm : ActorStatValuesForm
{
	private string _itemName;
	
	public override void SaveStatValues()
	{
		GameServices.Instance.Stats.SaveItemStatValues(_itemName, StatValues);
	}

	public override ActorStatCollection GetStatCollection()
	{
		return GameServices.Instance.Stats.ItemStatCollection;
	}

	public void InitializeItemForm(string itemName)
	{
		_itemName = itemName;
		StatValues = GameServices.Instance.Stats.LoadItemStats(_itemName);
		PopulateStatValues(StatValues);	
	}
}
