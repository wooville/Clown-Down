using Godot;
using System;

public partial class LevelEndGUI : Control
{
	private Label titleLabel;
	private Label chestsFoundLabel;
	private Label clownsFreedLabel;
	private Label guardsGoofedLabel;
	private Label timeLabel;
	private GeneralChoiceControl continueControl;
	// private LevelManager levelManager;
	private string timeString;
	int currentLevel = 0;

	public override void _Ready()
	{
		titleLabel = GetNode<Label>("StatsPanel/TitleLabel");
		chestsFoundLabel = GetNode<Label>("StatsPanel/StatsControl/ChestsControl/ChestsFoundLabel");
		clownsFreedLabel = GetNode<Label>("StatsPanel/StatsControl/ClownsControl/ClownsFreedLabel");
		guardsGoofedLabel = GetNode<Label>("StatsPanel/StatsControl/GuardsControl/GuardsGoofedLabel");
		timeLabel = GetNode<Label>("StatsPanel/TimeLabel");
		// levelManager = (LevelManager) GetTree().GetFirstNodeInGroup("manager");
		continueControl = GetNode<GeneralChoiceControl>("ContinueControl");
	}

	public override async void _Process(double delta){

		if (Input.IsActionJustPressed("dash_interact")){
			CallDeferred(MethodName.SetVisible, false);
			CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.Disabled);
			// GetTree().ChangeSceneToFile("res://World/LevelManager.tscn");
			GetTree().CallGroup("manager", "proceedLevel", currentLevel);
			GetTree().Paused = false;
		}
	}

	private void endLevel(int currentLevel, int chests, int clowns, int guards){
		this.currentLevel = currentLevel;
		CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
		titleLabel.Text = "CLOWN IN UNDERGROUND " + currentLevel;
		chestsFoundLabel.Text = chests.ToString();
		clownsFreedLabel.Text = clowns.ToString();
		guardsGoofedLabel.Text = guards.ToString();
		continueControl.selected = true;
		continueControl.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}

	private void setTimeString(string timeString){
		this.timeString = timeString;
		timeLabel.Text = "IN " + timeString;
	}
}
