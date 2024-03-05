using Godot;
using System;
using Roguelike.Script.Actor.Stats;

namespace Roguelike.Script.Actor.Character;

public partial class Character : Actor
{
	[Export]
	public int Initiative { get; set; }
}
