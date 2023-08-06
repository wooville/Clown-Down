using Godot;
using System;

public partial class Projectile1 : Area2D
{
	[Export] public float speed = 40.0f;
	[Export] public int damage = 1;
	[Export] public int numBounces = 0;
	private bool parryRedirected = false;

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
	public override void _PhysicsProcess(double delta)
	{
		this.GlobalPosition += direction * (float)delta * speed;
		
	}

	private void _on_visible_on_screen_notifier_2d_screen_exited()
	{
		QueueFree();
	}

	private void _on_area_entered(Area2D area)
	{
		if (area.IsInGroup("enemy") && parryRedirected){
			area.GetParent<BasicGuardController>().TakeDamage(damage);
			QueueFree();
		}
		//GD.Print(area);
		// Replace with function body.
		
	}
	
	private void _on_body_entered(Node2D body)
	{
		
		foreach(String str in body.GetGroups()){
			//GD.Print(str);
			if (str == "player" && !parryRedirected){
				//GD.Print("Hit Player");
				((Player)body).TakeDamage(damage);
				QueueFree();
			}
			else if (str == "enemy" && parryRedirected){
				((BasicGuardController)body).TakeDamage(damage);
				QueueFree();
			}
			else if(str == "wall" && numBounces > 0){
				//GD.Print("Hit wall and bounced");
			}
			else if (str == "wall"){
				//GD.Print("Hit wall and broke");
				QueueFree();
			}
		}
		
	}

	public void redirectProjectile(Player.SIDES side){
		parryRedirected = true;
		GD.Print(side);
		switch (side){
			case Player.SIDES.LEFT:
				direction = Vector2.Left;
				break;
			case Player.SIDES.UP:
				direction = Vector2.Up;
				break;
			case Player.SIDES.RIGHT:
				direction = Vector2.Right;
				break;
			case Player.SIDES.DOWN:
				direction = Vector2.Down;
				break;
		}
		// direction *= -1;
		speed *= 1.5f;
	}
}








