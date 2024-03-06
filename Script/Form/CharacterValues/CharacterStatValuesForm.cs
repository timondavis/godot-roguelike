using Roguelike.Script.Actor.Stats;
using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Form.CharacterValues;

public abstract partial class CharacterStatValuesForm : ActorStatValuesForm
{
	public override ActorStatCollection GetStatCollection()
	{
		return GameServices.Instance.Stats.CharacterStatCollection;
	}
}
