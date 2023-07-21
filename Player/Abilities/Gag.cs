using Godot;
using System;

public partial class Gag : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_timer_timeout(){
		QueueFree();
	}

	private void _on_area_2d_area_entered(Area2D area){
		if (area.IsInGroup("enemy")){
			var enemy = (BasicGuardController) area.GetParent();
			enemy.IsAsleep = true;
			GD.Print(enemy.IsAsleep);
		}
	}
}
