using Godot;
using System;
using System.Collections;

namespace Roguelike.Map.Model;

public partial class Room : GodotObject
{
    public Vector2I Center { get; set; }
}
