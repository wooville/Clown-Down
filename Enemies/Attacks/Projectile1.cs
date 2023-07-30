using Godot;
using System;

public partial class Projectile1 : Area2D
{
	[Export] public float speed = 40.0f;
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
		foreach(String str in area.GetGroups()){
			GD.Print(str);
			/*
			if(str == "wall" && numBounces > 0){
				GD.Print("Hit wall and bounced");
			}
			else if (str == "Wall"){
				GD.Print("Hit wall and broke");
				QueueFree();
			}
			*/
		}
		//GD.Print(area);
		// Replace with function body.
		
	}
	
	private void _on_body_entered(Node2D body)
	{
		
		foreach(String str in body.GetGroups()){
			//GD.Print(str);
			if (str == "player"){
				GD.Print("Hit Player");
				//body.TakeDamage(damage);
				QueueFree();
			}
			else if(str == "wall" && numBounces > 0){
				GD.Print("Hit wall and bounced");
			}
			else if (str == "wall"){
				GD.Print("Hit wall and broke");
				QueueFree();
			}
		}
		
	}
}








