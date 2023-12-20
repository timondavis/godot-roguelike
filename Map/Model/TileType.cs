using Godot;
using System;
using System.Collections.Generic;

public partial class TileType : GodotObject, IEquatable<TileType>
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Represents a dictionary containing key-value pairs of strings.
    /// </summary>
    public Dictionary<string, string> Data;

    /// <summary>
    /// Represents a tile type.
    /// </summary>
    public TileType()
    {
        Data = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Represents an indexer that allows accessing elements of the cell's data dictionary using property names as keys.
    /// </summary>
    /// <param name="propertyName">The property name used as the key to access the value in the dictionary.</param>
    /// <returns>The value associated with the specified property name.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the property with the specified key is not found in the dictionary.</exception>
    public string this[string propertyName]
    {
        get
        {
            if (Data.ContainsKey(propertyName))
            {
                return Data[propertyName];
            }

            throw new InvalidOperationException($"Property with key '{propertyName}' not found.");
        }

        set 
        {
            Data[propertyName] = value;
        }
    }

    public bool Equals(TileType other)
    {
        if (other != null && other.Name == Name)
        {
            return true;
        }

        return false;
    }
}
