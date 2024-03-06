using Godot;
using Roguelike.Script.Actor.Stats;

namespace Roguelike.Script.Form;

public abstract partial class ActorStatValuesForm : Node
{
	private const string FormRowScenePath = "res://Form/Control/StatValueRow.tscn";
	private const string FormRowContainerPath = "FormLayout/FieldContainer/Content/Rows";
	private const string FormButtonPath = "FormLayout/Buttons/ButtonContainer/SaveButton";
	protected const string LineEditChangedEvent = "text_changed";

	protected ActorStatCollection Stats = new ActorStatCollection();
	protected ActorStatValues StatValues = new ActorStatValues();
	
	private Button SaveButton
	{
		get
		{
			return GetNode<Button>(FormButtonPath);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SaveButton.Disabled = true;
	}

	public void on_line_edit_changed(string newValue)
	{
		ValidateState();
	}

	public void on_save_button_pressed()
	{
		SaveStatValues();
		SaveButton.Disabled = true;
	}

	public abstract void SaveStatValues();

	public abstract ActorStatCollection GetStatCollection();
		
	protected void PopulateStatValues(ActorStatValues statValues)
	{
		Stats = GetStatCollection();
		StatValues = statValues;
		
		foreach (ActorStat stat in Stats.Stats)
		{
			var newRow = GenerateFormRowInstance();
			var rowsContainer = GetFormRowsContainer();
			rowsContainer.AddChild(newRow);
			AttachListenerForRow(newRow);
			SetLabelForRow(newRow, stat.StatName);

			int value;
			if (StatValues != null && StatValues.TryGetValue(stat.StatName, out value))
			{
				SetValueForRow(newRow, value.ToString());
			}
		}
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
		ActorStatValues values = new ActorStatValues();
		VBoxContainer rowsContainer = GetFormRowsContainer();
		foreach (VBoxContainer row in rowsContainer.GetChildren())
		{
			Label fieldLabel = row.GetChild(1).GetChild(0).GetChild<Label>(0);
			LineEdit fieldValue = row.GetChild(1).GetChild(2).GetChild<LineEdit>(0);
			int valueFound;
			if (int.TryParse(fieldValue.Text, out valueFound))
			{
				values[fieldLabel.Text] = valueFound;
			}
		}

		StatValues = values;
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
		VBoxContainer rowsContainer = GetFormRowsContainer();
		foreach (VBoxContainer row in rowsContainer.GetChildren())
		{
			LineEdit fieldValue = row.GetChild(1).GetChild(2).GetChild<LineEdit>(0);
			if (fieldValue.Text.Trim() == "" || ! int.TryParse(fieldValue.Text, out _))
			{
				return false;
			}
		}

		return true;
	}
}
