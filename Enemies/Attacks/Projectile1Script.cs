using Godot;
using System;

public partial class Projectile1 : Area2D
{
	[Export] public float speed = 1.0f;
	[Export] public float damage = 1.0f;
	[Export] public int numBounces = 0;

	private Vector2 direction = new Vector2(0,1);


	public Vector2 Direction{
		get{return direction;}
		set{direction = value.Normalized();}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.GlobalPosition += direction * (float)delta * speed;
		
	}

	private void _on_visible_on_screen_notifier_2d_screen_exited()
	{
		QueueFree();
	}

	private void _on_area_entered(Area2D area)
	{
		// Replace with function body.
		QueueFree();
	}
	
	private void _on_body_entered(Node2D body)
	{
		// Replace with function body.
		QueueFree();
	}
}








