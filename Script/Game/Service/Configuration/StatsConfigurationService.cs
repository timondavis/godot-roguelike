using Godot;
using System.Text.Json;
using Roguelike.Script.Actor.Stats;

namespace Roguelike.Script.Game.Service.Configuration;

public class StatsConfigurationService
{
	private const string ConfigFilePath = "user://game-config.cfg";
	private const string Section_StatLists = "StatLists";
	private const string Section_PlayerStatValues = "PlayerStatValues";
	private const string Section_MobStatValues = "MobStatValues";
	private const string Section_ItemStatValues = "ItemStatValues";
	private const string Key_CharacterStatList = "CharacterStatList";
	private const string Key_ItemStatList = "ItemStatList";
	private const string Key_PlayerStatValues = "PlayerStatValues";

	private static StatsConfigurationService _instance;

	private StatsConfigurationService()
	{
	}

	/// <summary>
	/// Gets the singleton instance of the StatsConfigurationService class.
	/// </summary>
	/// <remarks>
	/// This property provides a thread-safe way to access an instance of the StatsConfigurationService class. If an instance
	/// does not yet exist, a new instance will be created. Subsequent calls to this property will return the existing instance.
	/// </remarks>
	public static StatsConfigurationService Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new StatsConfigurationService();
			}

			return _instance;
		}
	}

	/// <summary>
	/// Represents a collection of character stats.
	/// </summary>
	/// <value>
	/// The character stat collection.
	/// </value>
	public ActorStatCollection CharacterStatCollection => ParseStatCollection(Section_StatLists, Key_CharacterStatList);

	/// <summary>
	/// Represents a collection of item statistics.
	/// </summary>
	/// <remarks>
	/// The ItemStatCollection is responsible for parsing the item statistics from the
	/// given section and key in the configuration file.
	/// </remarks>
	/// <returns>
	/// An instance of the ActorStatCollection representing the parsed item statistics.
	/// </returns>
	public ActorStatCollection ItemStatCollection => ParseStatCollection(Section_StatLists, Key_ItemStatList);

	/// <summary>
	/// Saves the provided character stat collection.
	/// </summary>
	/// <param name="characterStatCollection">The character stat collection to be saved.</param>
	public void SaveCharacterStatCollection(ActorStatCollection characterStatCollection)
	{
		StoreStatCollection(Section_StatLists, Key_CharacterStatList, characterStatCollection);
	}

	/// <summary>
	/// Saves the given ActorStatCollection to the storage.
	/// </summary>
	/// <param name="itemStatCollection">The ActorStatCollection to be saved.</param>
	public void SaveItemStatCollection(ActorStatCollection itemStatCollection)
	{
		StoreStatCollection(Section_StatLists, Key_ItemStatList, itemStatCollection);
	}

	/// <summary>
	/// Saves the stat values for a given mob.
	/// </summary>
	/// <param name="mobName">The name of the mob.</param>
	/// <param name="values">The stat values to be saved.</param>
	public void SaveMobStatValues(string mobName, ActorStatValues values)
	{
		SaveStatValues(Section_MobStatValues, mobName, values);
	}

	/// <summary>
	/// Saves the player's stat values.
	/// </summary>
	/// <param name="values">
	/// The stat values to be saved.
	/// </param>
	public void SavePlayerStatValues(ActorStatValues values)
	{
		SaveStatValues(Section_PlayerStatValues, Key_PlayerStatValues, values);
	}

	/// <summary>
	/// Saves the stat values for an item.
	/// </summary>
	/// <param name="itemName">The name of the item.</param>
	/// <param name="values">The stat
	public void SaveItemStatValues(string itemName, ActorStatValues values)
	{
		SaveStatValues(Section_ItemStatValues, itemName, values);
	}

	/// <summary>
	/// Loads the statistical values for a given mob.
	/// </summary>
	/// <param name="mobName">The name of the mob.</param>
	/// <returns>The statistical values for the specified mob.</returns>
	public ActorStatValues LoadMobStats(string mobName)
	{
		return LoadStatValues(Section_MobStatValues, mobName);
	}

	/// <summary>
	/// Loads the statistics for a specific item.
	/// </summary>
	/// <param name="itemName">The name of the item.</param>
	/// <returns>An ActorStatValues object containing the statistics of the item.</returns>
	public ActorStatValues LoadItemStats(string itemName)
	{
		return LoadStatValues(Section_ItemStatValues, itemName);
	}

	/// <summary>
	/// Loads the player's statistics.
	/// </summary>
	/// <returns>
	/// The actor's statistics as an instance of the ActorStatValues class.
	/// </returns>
	public ActorStatValues LoadPlayerStats()
	{
		return LoadStatValues(Section_PlayerStatValues, Key_PlayerStatValues);
	}

	/// <summary>
	/// Saves the specified <paramref name="values"/> for a given <paramref name="section"/> and <paramref name="key"/>
	/// in the configuration file.
	/// </summary>
	/// <param name="section">The section name in the configuration file.</param>
	/// <param name="key">The key name in the configuration file.</param>
	/// <param name="values">The <see cref="ActorStatValues"/> object to be saved.</param>
	private void SaveStatValues(string section, string key, ActorStatValues values)
	{
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		file.SetValue(section, key, 
			JsonService.Instance.StringifyActorStatValues(values));
	}

	/// <summary>
	/// Loads the stat values for an actor from a configuration file.
	/// </summary>
	/// <param name="section">The section name in the configuration file.</param>
	/// <param name="key">The key name in the configuration file.</param>
	/// <param name="strDefault">The default value to use if the key is not found.</param>
	/// <returns>The stat values for the actor.</returns>
	private ActorStatValues LoadStatValues(string section, string key, string strDefault = "")
	{
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		var json = file.GetValue(section, key, strDefault).ToString();
		if (json.Trim() != "")
		{
			return  JsonService.Instance.ParseActorStatValues(json);
		}
		
		return new ActorStatValues();
	}

	/// <summary>
	/// Parses an actor stat collection from a configuration file using the specified section and key.
	/// </summary>
	/// <param name="section">The section name in the configuration file.</param>
	/// <param name="key">The key name in the specified section of the configuration file.</param>
	/// <returns>An instance of ActorStatCollection parsed from the configuration file.</returns>
	private ActorStatCollection ParseStatCollection(string section, string key)
	{
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		var json = file.GetValue(section, key, "").AsString();
		ActorStatCollection collection = JsonService.Instance.ParseActorStatCollection(json);
		return collection;
	}

	/// <summary>
	/// Stores the given ActorStatCollection object into a config file.
	/// </summary>
	/// <param name="section">The section in the config file.</param>
	/// <param name="key">The key for the value in the section.</param>
	/// <param name="collection">The ActorStatCollection to be stored.</param>
	private void StoreStatCollection(string section, string key, ActorStatCollection collection)
	{
		string json = JsonService.Instance.StringifyActorStatCollection(collection);
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		file.SetValue(section, key, json);
		file.Save(ConfigFilePath);		
	}
}
