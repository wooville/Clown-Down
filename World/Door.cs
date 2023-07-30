using Godot;
using System;

public partial class Door : Interactable
{
	private Player player;
	
	public override void _Ready()
	{
        base._Ready();
		player = (Player) GetTree().GetFirstNodeInGroup("player");
	}

	public override void Interact(){
		if (player.hasKey) {
			GetTree().ChangeSceneToFile("res://GUI/MainMenu.tscn");
		} else {
			GD.Print("Get that key!");
		}
	}
}
