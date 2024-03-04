using System.Collections.Generic;
using Godot;
using System.Text.Json;
using Godot.Collections;

namespace Roguelike.Form.Script;

public partial class MobStatValues : Node
{
	private const string FilePath = "user://game-config.json";
	private const string FormRowScenePath = "res://Form/Control/StatValueRow.tscn";
	private const string FormRowContainerPath = "FormLayout/FieldContainer/Content/Rows";
	private const string FormButtonPath = "FormLayout/Buttons/ButtonContainer/SaveButton";
	private const string ConfigSection = "GameConfig";
	private const string CharacterStats = "CharacterStat";
	private const string MobStatDefs = "MobStatDefs";
	private const string MobStatValuesPrefix = "MobStatValues_";
	private const string LineEditChangedEvent = "text_changed";

	[Signal]
	public delegate void RequestMobNameEventHandler();
	
	private Array<string> _statNames;
	private Godot.Collections.Dictionary<string, string> _statValues;
	private string _mobName;
	
	private Button SaveButton
	{
		get
		{
			return GetNode<Button>(FormButtonPath);
		}
	}
	
	public MobStatValues() : base()
	{
		_statNames = new Array<string>();
		_statValues = new Godot.Collections.Dictionary<string, string>();
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SaveButton.Disabled = true;
	}
	
		
	public void PopulateStatValues(string mobName)
	{
		var file = new ConfigFile();
		file.Load(FilePath);
		
		var statNamesString = file.GetValue(ConfigSection, CharacterStats).AsString();
		_statNames = JsonSerializer.Deserialize<Array<string>>(statNamesString);	
		
		var statValuesString = file.GetValue(MobStatDefs, MobStatValuesPrefix + _mobName, "").AsString();
		if (statValuesString.Trim() != "")
		{
			_statValues = JsonSerializer.Deserialize<Godot.Collections.Dictionary<string,string>>(statValuesString);
		}
		
		foreach (string statName in _statNames)
		{
			var newRow = GenerateFormRowInstance();
			var rowsContainer = GetFormRowsContainer();
			rowsContainer.AddChild(newRow);
			AttachListenerForRow(newRow);
			SetLabelForRow(newRow, statName);

			string value;
			_statValues.TryGetValue(statName, out value);

			if (value != null && value.Trim() != "" && int.TryParse(value, out _))
			{
				SetValueForRow(newRow, value);
			}
		}
	}

	public void on_line_edit_changed(string newValue)
	{
		ValidateState();
	}

	public void on_save_button_pressed()
	{
		SaveValues();
	}
	
	private VBoxContainer GenerateFormRowInstance()
	{
		PackedScene rowScene = ResourceLoader.Load<PackedScene>(FormRowScenePath);
		VBoxContainer row = rowScene.Instantiate<VBoxContainer>();
		return row;
	}

	private VBoxContainer GetFormRowsContainer()
	{
		VBoxContainer rowContainer = GetNode<VBoxContainer>(FormRowContainerPath);
		return rowContainer;
	}

	private void SetLabelForRow(VBoxContainer row, string statName)
	{
		Label fieldLabel = row.GetChild(1).GetChild(0).GetChild<Label>(0);
		fieldLabel.Text = statName;
	}

	private void SetValueForRow(VBoxContainer row, string statValue)
	{
		LineEdit lineEdit = row.GetChild(1).GetChild(2).GetChild<LineEdit>(0);
		lineEdit.Text = statValue;
	}

	private void AttachListenerForRow(VBoxContainer row)
	{
		LineEdit lineEdit = row.GetChild(1).GetChild(2).GetChild<LineEdit>(0);
		lineEdit.Connect(LineEditChangedEvent, new Callable( this, nameof (on_line_edit_changed)));
	}

	private void CollectStatValues()
	{
		Godot.Collections.Dictionary<string, string> values = new Godot.Collections.Dictionary<string, string>();
		VBoxContainer rowsContainer = GetFormRowsContainer();
		foreach (VBoxContainer row in rowsContainer.GetChildren())
		{
			Label fieldLabel = row.GetChild(1).GetChild(0).GetChild<Label>(0);
			LineEdit fieldValue = row.GetChild(1).GetChild(2).GetChild<LineEdit>(0);
			values[fieldLabel.Text] = fieldValue.Text;
		}

		_statValues = values;
	}
	
	public void SaveValues()
	{
		string json = JsonSerializer.Serialize(_statValues);

		var file = new Godot.ConfigFile();
		file.Load(FilePath);
		file.SetValue(MobStatDefs, MobStatValuesPrefix + _mobName, json);
		file.Save(FilePath);
	}

	private void ValidateState()
	{
		CollectStatValues();
		if (IsStateValid())
		{
			SaveButton.Disabled = false;
		}
		else
		{
			SaveButton.Disabled = true;
		}
	}

	private bool IsStateValid()
	{
		foreach (KeyValuePair<string, string> pair in _statValues)
		{
			if (pair.Value.Trim() == "" || ! int.TryParse(pair.Value, out _))
			{
				return false;
			}
		}

		return true;
	}
}
