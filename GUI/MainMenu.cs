using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : Control
{
	[Export]
	public GeneralChoiceControl[] controlList;
	private int currentSelect = 0;

	public override void _Ready()
	{
		controlList[currentSelect].selected = true;
		controlList[currentSelect].updateControl();
	}

	public override async void _Process(double delta){
		if (Input.IsActionJustPressed("up") || Input.IsActionJustPressed("action_up")){
			controlList[currentSelect].selected = false;
			controlList[currentSelect].updateControl();
			
			currentSelect--;
			if (currentSelect < 0) currentSelect = controlList.Length-1;

			controlList[currentSelect].selected = true;
			controlList[currentSelect].updateControl();
		}

		if (Input.IsActionJustPressed("down") || Input.IsActionJustPressed("action_down")){
			controlList[currentSelect].selected = false;
			controlList[currentSelect].updateControl();
			
			currentSelect++;
			currentSelect %= controlList.Length;

			controlList[currentSelect].selected = true;
			controlList[currentSelect].updateControl();
		}

		if (Input.IsActionJustPressed("dash_interact")){
			if (controlList[currentSelect].Name == "StartControl"){
				GetTree().ChangeSceneToFile("res://World/LevelManager.tscn");
			} else if (controlList[currentSelect].Name == "QuitControl"){
				GetTree().Quit();
			}
		}
	}
}


