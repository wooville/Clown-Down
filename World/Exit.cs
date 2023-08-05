using Godot;
using System;

public partial class Exit : Interactable
{
	public int currentLevel;
	public int totalClowns;
	private int clownsRemaining;
	
	public override void _Ready()
	{
        base._Ready();
		
		clownsRemaining = totalClowns;
		buttonPromptLabel.Text = clownsRemaining.ToString();
	}

	private void clownFreed(){
		clownsRemaining--;
		if (clownsRemaining > 0){
			buttonPromptLabel.Text = clownsRemaining.ToString();
		} else {
			buttonPromptLabel.Text = "␣";
		}
	}

	public override void Interact(){
		// if (player.hasKey) {
		GetTree().CallGroup("manager", "tryEndLevel", currentLevel);
		
		// GetTree().ChangeSceneToFile("res://GUI/MainMenu.tscn");
		// 
		// } else {
		// 	GD.Print("Get that key!");
		// }
	}
}
