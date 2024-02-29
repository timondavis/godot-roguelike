using Godot;
using System;
using System.IO;
using Godot.Collections;
using Roguelike.Actor.Stats;
using FileAccess = Godot.FileAccess;

namespace Roguelike.Game.Service.Configuration;

public class StatsConfigurationService
{
	private const string CharacterStatConfigPath = "../../Config/CharacterStats.json";
	private const string ItemStatConfigPath = "../../Config/ItemStats.json";

	private static StatsConfigurationService _instance;

	private StatsConfigurationService()
	{
	}

	public static StatsConfigurationService Instance()
	{
		if (_instance == null)
		{
			_instance = new StatsConfigurationService();
		}

		return _instance;
	}

	public ActorStatCollection CharacterStatCollection => ParseStatCollection(CharacterStatConfigPath);
	public ActorStatCollection ItemStatCollection => ParseStatCollection(ItemStatConfigPath);

	private ActorStatCollection ParseStatCollection(string path)
	{
		var json = LoadAndExtractJson(path);
		var dataDictionary = json.Data.AsGodotDictionary();

		ValidateTopLevelAssociationJsonFile(dataDictionary, path);
		Array<ActorStat> stats = dataDictionary["stats"].AsGodotArray<ActorStat>();

		ActorStatCollection collection = new ActorStatCollection();
		collection.Stats = stats;

		return collection;
	}
	
	private Json LoadAndExtractJson(string configFilePath)
	{
		Json json = new Json();
		FileAccess file = FileAccess.Open(configFilePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			throw new FileNotFoundException("The specified json file could not be found or loaded.", configFilePath);	
		}
		var error = json.Parse(file.GetAsText());
		file.Close();
		
		if (error != Error.Ok)
		{
			throw new IOException("An error occurred while parsing the json file.");	
		}

		return json;
	}
	
	private void ValidateTopLevelAssociationJsonFile(Dictionary dictionary, string path)
	{
		if (!dictionary.ContainsKey("stats"))
		{
			throw new InvalidOperationException("Required key 'stats' is missing from the config file at " + path );
		}	
	}
}
