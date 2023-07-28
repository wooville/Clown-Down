using Godot;
using System;

public partial class Chicken : Node2D
{
	private void _on_timer_timeout(){
		QueueFree();
	}
}
