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
		titleLabel = GetNode<Label>("StatsPanel/TitleLabel");
		chestsFoundLabel = GetNode<Label>("StatsPanel/StatsControl/ChestsControl/ChestsFoundLabel");
		clownsFreedLabel = GetNode<Label>("StatsPanel/StatsControl/ClownsControl/ClownsFreedLabel");
		guardsGoofedLabel = GetNode<Label>("StatsPanel/StatsControl/GuardsControl/GuardsGoofedLabel");
		timeLabel = GetNode<Label>("StatsPanel/TimeLabel");
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
		chestsFoundLabel.Text = levelManager.totalChestsFound.ToString();
		clownsFreedLabel.Text = levelManager.totalClownsFreed.ToString();
		guardsGoofedLabel.Text =  levelManager.totalGuardsGoofed.ToString();
		quitControl.updateControl();
		retryControl.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}

	private void gameWon(){
		CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
		titleLabel.Text = "ALL CLOWNS FREED!";
		chestsFoundLabel.Text = levelManager.totalChestsFound.ToString();
		clownsFreedLabel.Text = levelManager.totalClownsFreed.ToString();
		guardsGoofedLabel.Text = levelManager.totalGuardsGoofed.ToString();
		quitControl.updateControl();
		retryControl.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}

	private void setTimeString(string timeString){
		this.timeString = timeString;
		timeLabel.Text = "IN " + timeString + " TOTAL";
	}
}
