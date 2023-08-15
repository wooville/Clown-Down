using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : Control
{
	[Export]
	public GeneralChoiceControl[] controlList;
	private int currentSelect = 0;
	private PointLight2D selectorPointLight;

	public override void _Ready()
	{
		controlList[currentSelect].selected = true;
		controlList[currentSelect].updateControl();

		selectorPointLight = GetNode<PointLight2D>("SelectorPointLight");
		// selectorPointLight.Position = new Vector2(selectorPointLight.Position.X, controlList[currentSelect].GlobalPosition.Y+controlList[currentSelect].Size.Y/2);
		// GD.Print(selectorPointLight.Position);
	}

	public override async void _Process(double delta){
		if (Input.IsActionJustPressed("up") || Input.IsActionJustPressed("action_up")){
			controlList[currentSelect].selected = false;
			controlList[currentSelect].updateControl();
			
			currentSelect--;
			if (currentSelect < 0) currentSelect = controlList.Length-1;

			controlList[currentSelect].selected = true;
			controlList[currentSelect].updateControl();

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(selectorPointLight, "position", new Vector2(selectorPointLight.Position.X, controlList[currentSelect].GlobalPosition.Y+controlList[currentSelect].Size.Y/2), 0.25f);

			// selectorPointLight.Position = ;
			// GD.Print(selectorPointLight.Position);
		}

		if (Input.IsActionJustPressed("down") || Input.IsActionJustPressed("action_down")){
			controlList[currentSelect].selected = false;
			controlList[currentSelect].updateControl();
			
			currentSelect++;
			currentSelect %= controlList.Length;

			controlList[currentSelect].selected = true;
			controlList[currentSelect].updateControl();

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(selectorPointLight, "position", new Vector2(selectorPointLight.Position.X, controlList[currentSelect].GlobalPosition.Y+controlList[currentSelect].Size.Y/2), 0.25f);


			// selectorPointLight.Position = new Vector2(selectorPointLight.Position.X, controlList[currentSelect].GlobalPosition.Y+controlList[currentSelect].Size.Y/2);
			// GD.Print(selectorPointLight.Position);
		}

		if (Input.IsActionJustPressed("dash_interact")){
			if (controlList[currentSelect].Name == "StartControl"){
				GetTree().ChangeSceneToFile("res://World/LevelManager.tscn");
			} else if (controlList[currentSelect].Name == "HowToPlayControl"){
				GetTree().ChangeSceneToFile("res://GUI/HowToPlay.tscn");
			} else if (controlList[currentSelect].Name == "CreditsControl"){
				GetTree().ChangeSceneToFile("res://GUI/Credits.tscn");
			} else if (controlList[currentSelect].Name == "QuitControl"){
				GetTree().Quit();
			}
		}
	}
}


