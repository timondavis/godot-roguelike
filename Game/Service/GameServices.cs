using Roguelike.Game.Service.Configuration;

namespace Roguelike.Game.Service;

public class GameServices
{
	private static GameServices _instance;

	private GameServices()
	{
	}

	public static GameServices Instance()
	{
		if (_instance == null)
		{
			_instance = new GameServices();
		}

		return _instance;
	}

	public StatsConfigurationService StatsConfiguration => StatsConfigurationService.Instance();
}
