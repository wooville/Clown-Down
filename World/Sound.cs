using Godot;
using System;

public partial class Sound : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void _on_timer_timeout(){
		QueueFree();
	}
}
