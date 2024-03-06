using Godot;

namespace Roguelike.Script.Form.Control;

public partial class FieldNameRow : VBoxContainer
{
	private void _on_remove_row_pressed()
	{
		QueueFree();
	}
}

