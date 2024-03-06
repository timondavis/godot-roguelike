using Roguelike.Script.Actor.Stats;
using Roguelike.Script.Game.Service;

namespace Roguelike.Script.Form.Config.Game.Stat;

public partial class ItemStatConfig : Form.Config.Game.StatConfig
{
	protected override ActorStatCollection GetStatsCollection()
	{
		return GameServices.Instance.Stats.ItemStatCollection;
	}

	protected override void WriteStatsCollection()
	{
		GameServices.Instance.Stats.SaveItemStatCollection(StatsCollection);
	}
}
