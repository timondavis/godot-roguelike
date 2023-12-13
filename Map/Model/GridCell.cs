using System;
using Godot;
using Godot.Collections;

namespace Roguelike.Map.Model;

public partial class GridCell : GodotObject
{
    /// <summary>
    /// Gets or sets a value indicating whether the object is active or not.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the position of an object.
    /// </summary>
    public Vector2I Position { get; set; }

    /// <summary>
    /// Gets or sets the data associated with the property.
    /// </summary>
    /// <value>
    /// A dictionary that contains key-value pairs of data.
    /// </value>
    public Dictionary<string, string> Data { get; private set; }

    /// <summary>
    /// Represents a single cell in a grid.
    /// </summary>
    public GridCell(int x = 0, int y = 0)
    {
        Position = new Vector2I(x, y);
        IsActive = false;
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

    /// <summary>
    /// Represents a cell in a grid.
    /// </summary>
    /// <param name="position">The position of the cell in the grid.</param>
    public GridCell(Vector2I position)
    {
        Position = position;
    }
}