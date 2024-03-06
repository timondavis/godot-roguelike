using System;
using System.Linq;
using System.Text.Json;
using Godot.Collections;
using Roguelike.Script.Actor.Stats;

namespace Roguelike.Script.Game.Service;

public class JsonService
{
    private static JsonService _instance;
    
    private JsonService() {}

    public static JsonService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JsonService();
            }

            return _instance;
        }
    }

    public string StringifyActorStatCollection(ActorStatCollection actorStatCollection)
    {
        Array<string> stats = new Array<string>();
        foreach (ActorStat stat in actorStatCollection.Stats)
        {
            stats.Add(stat.StatName);
        }

        Dictionary<string, Array<string>> dict = new Dictionary<string, Array<string>>();
        dict["Stats"] = stats;
        return JsonSerializer.Serialize(dict);
    }

    public ActorStatCollection ParseActorStatCollection(string actorStatCollectionString)
    {
        Dictionary<string,Array<string>> dict = JsonSerializer.Deserialize<Dictionary<string,Array<string>>>(actorStatCollectionString);
        ActorStatCollection collection = new ActorStatCollection();
        Array<string> statNames;
        if (dict.TryGetValue("Stats", out statNames))
        {
            foreach (string name in statNames)
            {
                ActorStat stat = new ActorStat();
                stat.StatName = name;
                collection.Stats.Add(stat);
            }
        }
        return collection;
    }

    public string StringifyActorStatValues(ActorStatValues values)
    {
        return JsonSerializer.Serialize(values);
    }

    public ActorStatValues ParseActorStatValues(string json)
    {
        return JsonSerializer.Deserialize<ActorStatValues>(json);
    }
}