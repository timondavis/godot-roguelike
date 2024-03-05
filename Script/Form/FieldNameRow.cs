using Godot;
using System;

public partial class FieldNameRow : VBoxContainer
{
	private void _on_remove_row_pressed()
	{
		QueueFree();
	}
}

