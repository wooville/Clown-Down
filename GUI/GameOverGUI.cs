using Godot;
using System;

public partial class GameOverGUI : Control
{
	private Label titleLabel;
	private Label chestsFoundLabel;
	private Label clownsFreedLabel;
	private Label guardsGoofedLabel;
	private Label timeLabel;
	private GeneralChoiceControl quitControl;
	private GeneralChoiceControl retryControl;
	private LevelManager levelManager;
	private string timeString;

	public override void _Ready()
	{
		titleLabel = GetNode<Label>("StatsPanel/VBoxContainer/TitleLabel");
		chestsFoundLabel = GetNode<Label>("StatsPanel/VBoxContainer/ChestsFoundLabel");
		clownsFreedLabel = GetNode<Label>("StatsPanel/VBoxContainer/ClownsFreedLabel");
		guardsGoofedLabel = GetNode<Label>("StatsPanel/VBoxContainer/GuardsGoofedLabel");
		timeLabel = GetNode<Label>("StatsPanel/VBoxContainer/TimeLabel");
		levelManager = (LevelManager) GetTree().GetFirstNodeInGroup("manager");
		quitControl = GetNode<GeneralChoiceControl>("QuitControl");
		retryControl = GetNode<GeneralChoiceControl>("RetryControl");
	}

	public override async void _Process(double delta){
		if (Input.IsActionJustPressed("left") || Input.IsActionJustPressed("action_left")){
			quitControl.selected = true;
			retryControl.selected = false;
			quitControl.updateControl();
			retryControl.updateControl();
		}

		if (Input.IsActionJustPressed("right") || Input.IsActionJustPressed("action_right")){
			quitControl.selected = false;
			retryControl.selected = true;
			quitControl.updateControl();
			retryControl.updateControl();
		}

		if (Input.IsActionJustPressed("dash_interact")){
			if (quitControl.selected){
				CallDeferred(MethodName.SetVisible, false);
				CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.Disabled);
				GetTree().ChangeSceneToFile("res://GUI/MainMenu.tscn");
				GetTree().Paused = false;
			} else if (retryControl.selected){
				CallDeferred(MethodName.SetVisible, false);
				CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.Disabled);
				GetTree().ChangeSceneToFile("res://World/LevelManager.tscn");
				GetTree().Paused = false;
			}
		}
	}

	private void gameOver(){
		CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
		chestsFoundLabel.Text = "FOUND " + levelManager.totalChestsFound.ToString() + " CHESTS";
		clownsFreedLabel.Text = "UNBOUND " + levelManager.totalClownsFreed.ToString() + " FELLOW CLOWNS";
		guardsGoofedLabel.Text = "AND POUNDED " + levelManager.totalGuardsGoofed.ToString() + " GUARDS";
		quitControl.updateControl();
		retryControl.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}

	private void gameWon(){
		CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
		titleLabel.Text = "ALL CLOWNS FREED!";
		chestsFoundLabel.Text = "FOUND " + levelManager.totalChestsFound.ToString() + " CHESTS";
		clownsFreedLabel.Text = "UNBOUND " + levelManager.totalClownsFreed.ToString() + " FELLOW CLOWNS";
		guardsGoofedLabel.Text = "AND POUNDED " + levelManager.totalGuardsGoofed.ToString() + " GUARDS";
		quitControl.updateControl();
		retryControl.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}

	private void setTimeString(string timeString){
		this.timeString = timeString;
		timeLabel.Text = "IN " + timeString + " TOTAL";
	}
}
