using Godot;
using Godot.Collections;
using Roguelike.Script.Actor.Stats;

namespace Roguelike.Script.Form.Config.Game;

public abstract partial class StatConfig : Godot.Control
{
	private const string FormRowScenePath = "res://Form/Control/FieldNameRow.tscn";
	private const string FormRowContainerPath = "FormLayout/FieldContainer/Content/Rows";
	private const string SaveButtonPath = "FormLayout/Buttons/HBoxContainer/SaveButton";
	protected const string LineEditChangedEvent = "text_changed";

	protected ActorStatCollection StatsCollection = new ActorStatCollection();

	protected abstract ActorStatCollection GetStatsCollection();

	/// <summary>
	/// Writes the character stats to the game services.
	/// </summary>
	protected abstract void WriteStatsCollection();

	/// <summary>
	/// Retrieves the save button node.
	/// </summary>
	/// <returns>The save button node.</returns>
	private Button SaveButton
	{
		get
		{
			return GetNode<Button>(SaveButtonPath);
		}
	}

	/// <summary>
	/// Retrieves the VBoxContainer that holds the form rows.
	/// </summary>
	/// <returns>
	/// The VBoxContainer that represents the container for the form rows.
	/// </returns>
	private VBoxContainer FormRowsContainer
	{
		get
		{
			VBoxContainer rowsContainer = GetNode<VBoxContainer>(FormRowContainerPath);
			return rowsContainer;	
		}	
	}

	/// <summary>
	/// Retrieves the rows from the form.
	/// </summary>
	/// <returns>An array of nodes representing the rows in the form.</returns>
	private Array<Node> FormRows
	{
		get
		{
			VBoxContainer rowsContainer = GetNode<VBoxContainer>("FormLayout/FieldContainer/Content/Rows");
			return rowsContainer.GetChildren();	
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StatsCollection = GetStatsCollection();
		foreach (ActorStat stat in StatsCollection.Stats)
		{
			var newRow = GenerateFormRowInstance();
			FormRowsContainer.AddChild(newRow);
			AttachListenerForRow(newRow);
			SetValueForRow(newRow, stat.StatName);
		}

		SaveButton.Disabled = true;
	}

	/// <summary>
	/// Handles the button press event for adding a new row to the form.
	/// </summary>
	public void _on_add_row_button_pressed()
	{
		var newRow = GenerateFormRowInstance();
		FormRowsContainer.AddChild(newRow);
		AttachListenerForRow(newRow);
	}

	/// <summary>
	/// This method is called when the save button is pressed.
	/// It gathers the names of character statistics from the form and writes them.
	/// </summary>
	public void _on_save_button_pressed()
	{
		Array<string> fieldvalues = GatherStatNamesFromForm();
		Array<ActorStat> actorStats = new Array<ActorStat>();
		foreach (string name in fieldvalues)
		{
			ActorStat newStat = new ActorStat();
			newStat.StatName = name;
			actorStats.Add(newStat);
		}

		StatsCollection.Stats = actorStats;
		
		WriteStatsCollection();
		SaveButton.Disabled = true;
	}

	public void on_line_edit_changed(string newText)
	{
		SaveButton.Disabled = false;
	}

	/// <summary>
	/// Retrieves the value from a row in the VBoxContainer.
	/// </summary>
	/// <param name="row">The row to retrieve the value from.</param>
	/// <returns>The value from the row as a string. If the row is null, returns an empty string.</returns>
	private string GetValueFromRow(VBoxContainer row)
	{
		if (row != null)
		{
			HBoxContainer rowContents = row.GetChild<HBoxContainer>(1);
			MarginContainer fieldContainer = rowContents.GetChild<MarginContainer>(2);
			LineEdit field = fieldContainer.GetChild<LineEdit>(0);
			return field.Text;
		}

		return "";
	}

	/// <summary>
	/// Sets the value for a given row in a VBoxContainer.
	/// </summary>
	/// <param name="row">The VBoxContainer row to set the value for.</param>
	/// <param name="value">The value to set.</param>
	private void SetValueForRow(VBoxContainer row, string value)
	{
		if (row != null)
		{
			HBoxContainer rowContents = row.GetChild<HBoxContainer>(1);
			MarginContainer fieldContainer = rowContents.GetChild<MarginContainer>(2);
			LineEdit field = fieldContainer.GetChild<LineEdit>(0);
			field.Text = value;
		}
	}

	/// <summary>
	/// Gathers the names of statistics from a form.
	/// </summary>
	/// <returns>An array of string containing the names of statistics.</returns>
	private Array<string> GatherStatNamesFromForm()
	{
		Array<string> fieldValues = new Array<string>();
			
		foreach (Node rowNode in FormRows)
		{
			VBoxContainer row = rowNode as VBoxContainer;
			fieldValues.Add(GetValueFromRow(row));
		}

		return fieldValues;
	}

	/// <summary>
	/// Generates a new instance of a form row.
	/// </summary>
	/// <returns>A new instance of VBoxContainer representing a form row.</returns>
	private VBoxContainer GenerateFormRowInstance()
	{
		PackedScene newRowScene = ResourceLoader.Load<PackedScene>(FormRowScenePath);
		VBoxContainer newRow = newRowScene.Instantiate<VBoxContainer>();
		return newRow;
	}

	/// <summary>
	/// Attaches a listener to the specified row in a VBoxContainer.
	/// </summary>
	/// <param name="row">The VBoxContainer row to attach the listener to.</param>
	private void AttachListenerForRow(VBoxContainer row)
	{
		LineEdit lineEdit = row.GetChild(1).GetChild(2).GetChild<LineEdit>(0);
		lineEdit.Connect(LineEditChangedEvent, new Callable( this, nameof (on_line_edit_changed)));
	}
}
