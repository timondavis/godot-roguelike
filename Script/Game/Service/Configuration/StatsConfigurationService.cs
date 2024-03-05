using Godot;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Text.Json;
using Godot.Collections;
using Roguelike.Script.Actor.Stats;
using FileAccess = Godot.FileAccess;

namespace Roguelike.Script.Game.Service.Configuration;

public class StatsConfigurationService
{
	private const string ConfigFilePath = "user://game-config.cfg";
	private const string Section_StatLists = "StatLists";
	private const string Section_MobStatValues = "MobStatValues";
	private const string Key_CharacterStatList = "CharacterStatList";
	private const string Key_ItemStatList = "ItemStatList";

	private static StatsConfigurationService _instance;

	private StatsConfigurationService()
	{
	}

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

	public ActorStatCollection CharacterStatCollection => ParseStatCollection(Section_StatLists, Key_CharacterStatList);
	public ActorStatCollection ItemStatCollection => ParseStatCollection(Section_StatLists, Key_ItemStatList);

	private ActorStatCollection ParseStatCollection(string section, string key)
	{
		
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		var json = file.GetValue(section, key, "").AsString();
		ActorStatCollection collection = JsonService.Instance.ParseActorStatCollection(json);
		return collection;
	}

	public void SaveCharacterStatCollection(ActorStatCollection characterStatCollection)
	{
		StoreStatCollection(Section_StatLists, Key_CharacterStatList, characterStatCollection);
	}

	public void SaveItemStatCollection(ActorStatCollection itemStatCollection)
	{
		StoreStatCollection(Section_StatLists, Key_ItemStatList, itemStatCollection);
	}

	private void StoreStatCollection(string section, string key, ActorStatCollection collection)
	{
		string json = JsonService.Instance.StringifyActorStatCollection(collection);
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		file.SetValue(section, key, json);
		file.Save(ConfigFilePath);		
	}

	public void SaveMobStatValues(string mobName, ActorStatValues values)
	{
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		file.SetValue(Section_MobStatValues, mobName, JsonSerializer.Serialize(values));
		file.Save(ConfigFilePath);	
	}

	public ActorStatValues LoadMobStats(string mobName)
	{
		var file = new ConfigFile();
		file.Load(ConfigFilePath);
		var json = file.GetValue(Section_MobStatValues, mobName, "").ToString();
		return JsonSerializer.Deserialize<ActorStatValues>(json);
	}
}