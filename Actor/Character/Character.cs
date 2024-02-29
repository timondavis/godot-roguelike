using Godot;
using System;
using Roguelike.Actor.Stats;

namespace Roguelike.Actor.Character;

public partial class Character : Actor
{
	[Export]
	public int Initiative { get; set; }
}
