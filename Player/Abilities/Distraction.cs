using Godot;
using System;

public partial class Distraction : Node2D
{
	private void _on_timer_timeout(){
		QueueFree();
	}
}
