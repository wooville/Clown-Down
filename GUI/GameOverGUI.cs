using Godot;
using System;

public partial class GameOverGUI : Control
{
	private GameOverChoiceControl quitControl;
	private GameOverChoiceControl retryControl;

	public override void _Ready()
	{
		quitControl = GetNode<GameOverChoiceControl>("QuitControl");
		retryControl = GetNode<GameOverChoiceControl>("RetryControl");
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
				GetTree().ChangeSceneToFile("res://World/MockGeneratedRoom.tscn");
				GetTree().Paused = false;
			}
		}
	}

	private void _on_player_died(){
		CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
		quitControl.updateControl();
		retryControl.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}
}
