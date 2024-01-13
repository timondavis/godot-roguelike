using Godot;
using System;
using System.Collections;

namespace Roguelike.Map.Model;

public partial class Room : GodotObject
{
    private static int _nextId = 1;
    
    public int Id { get; private set; }
    public Vector2I Center { get; set; }

    public Room()
    {
        Id = _nextId;
        _nextId++;
    }
}
