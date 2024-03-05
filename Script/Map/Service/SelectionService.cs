using Godot;
using Roguelike.Script.Map.Model.Shapes;

namespace Roguelike.Script.Map.Generator.Service;

/// <summary>
/// Provides information about zones and allows for general zone queries.
/// </summary>
public class SelectionService
{
    private static SelectionService _instance;

    private SelectionService()
    {
    }

    public static SelectionService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SelectionService();
            }

            return _instance;
        }
    }

    /// <summary>
    /// If this array has contents, only the area within the listed ships should be edited by any editor commands.
    /// </summary>
    public Godot.Collections.Array<Shape> _selectedAreas = new Godot.Collections.Array<Shape>();

    public void ClearSelectedAreas()
    {
        _selectedAreas.Clear();
    }

    public Godot.Collections.Array<Shape> SelectedAreas
    {
        get
        {
            return _selectedAreas;
        }
        set
        {
            _selectedAreas = value;
        }
    }
    
    public bool IsPositionSelected(Vector2I position)
    {
        if (_selectedAreas.Count == 0)
        {
            return true;
        }
        
        foreach (Shape s in _selectedAreas)
        {
            if (s.IsPointWithinShape(position))
            {
                return true;
            }
        }

        return false;
    }
}