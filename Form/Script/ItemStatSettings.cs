using Godot;
using System.Text.Json;
using Godot.Collections;

namespace Roguelike.Form.Script;

public partial class ItemStatSettings : Control
{
	private const string FilePath = "user://game-config.json";
	private const string FormRowScenePath = "res://Form/Control/FieldNameRow.tscn";
	private const string FormRowContainerPath = "FormLayout/FieldContainer/Content/Rows";
	private const string ConfigSection = "GameConfig";
	private const string ItemStats = "ItemStats";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var file = new Godot.ConfigFile();
		file.Load(FilePath);
		var statNamesString = file.GetValue(ConfigSection, ItemStats).AsString();
		var statNames = JsonSerializer.Deserialize<Array<string>>(statNamesString);	
		foreach (string statName in statNames)
		{
			var newRow = GenerateFormRowInstance();
			var rowsContainer = GetFormRowsContainer();
			rowsContainer.AddChild(newRow);
			SetValueForRow(newRow, statName);
		}
	}

	/// <summary>
	/// Handles the button press event for adding a new row to the form.
	/// </summary>
	public void _on_add_row_button_pressed()
	{
		var newRow = GenerateFormRowInstance();
		var rowsContainer = GetFormRowsContainer();
		rowsContainer.AddChild(newRow);
	}

	/// <summary>
	/// This method is called when the save button is pressed.
	/// It gathers the names of character statistics from the form and writes them.
	/// </summary>
	public void _on_save_button_pressed()
	{
		Array<string> fieldvalues = GatherStatNamesFromForm();
		WriteCharacterStatNames(fieldvalues);
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
			
		foreach (Node rowNode in GetRowsFromForm())
		{
			VBoxContainer row = rowNode as VBoxContainer;
			fieldValues.Add(GetValueFromRow(row));
		}

		return fieldValues;
	}

	/// <summary>
	/// Retrieves the rows from the form.
	/// </summary>
	/// <returns>An array of nodes representing the rows in the form.</returns>
	private Array<Node> GetRowsFromForm()
	{
		VBoxContainer rowsContainer = GetNode<VBoxContainer>("FormLayout/FieldContainer/Content/Rows");
		return rowsContainer.GetChildren();
	}

	/// <summary>
	/// Writes the character stat names to a JSON file.
	/// </summary>
	/// <param name="statNames">An array of string containing the character stat names.</param>
	private void WriteCharacterStatNames(Array<string> statNames)
	{
		string json = JsonSerializer.Serialize(statNames);

		var file = new Godot.ConfigFile();
		file.SetValue(ConfigSection, ItemStats, json);
		file.Save(FilePath);
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
	/// Retrieves the VBoxContainer that holds the form rows.
	/// </summary>
	/// <returns>
	/// The VBoxContainer that represents the container for the form rows.
	/// </returns>
	private VBoxContainer GetFormRowsContainer()
	{
		VBoxContainer rowsContainer = GetNode<VBoxContainer>(FormRowContainerPath);
		return rowsContainer;
	}
}
