using Godot;
using System;

public partial class ClownJail : Interactable
{
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		player = (Player) GetTree().GetFirstNodeInGroup("player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void Interact()
	{
		if (player.keys > 0){
			player.keys--;
			GetTree().CallGroup("manager", "clownFreed");
			GetTree().CallGroup("exit", "clownFreed");
			base.Interact();
		}
	}
}
