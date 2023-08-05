using Godot;
using System;

public partial class Pie : Area2D
{
	private int damage = 1;
	public float speed {get;set;} = 150.0f;
	public Vector2 direction {get;set;} = Vector2.Up;

	private void _on_area_entered(Area2D area){
		if (area.IsInGroup("enemy")){
			var enemy = area.GetParent<BasicGuardController>();
			enemy.TakeDamage(damage);
		}
		else if (area.IsInGroup("projectile")){
			area.QueueFree();
		}
	}

	private void _on_body_entered(Node2D body){
		if (body.IsInGroup("wall")){
			QueueFree();
		}
	}

	private void _on_timer_timeout(){
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		this.GlobalPosition += direction * (float)delta * speed;
	}
}
