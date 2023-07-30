using Godot;
using System;

public partial class Key : Interactable
{
	private Player player;
	
	public override void _Ready()
	{
        base._Ready();
		player = (Player) GetTree().GetFirstNodeInGroup("player");
	}

	public override void Interact(){
		player.hasKey = true;
		// EmitSignal(SignalName.);
        QueueFree();
    }
}
