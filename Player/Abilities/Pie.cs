using Godot;
using System;

public partial class Pie : Node2D
{
	private int damage = 1;
	public float speed {get;set;} = 60.0f;
	public Vector2 direction {get;set;} = Vector2.Up;

	private void _on_attack_area_entered(Area2D area){
		if (area.IsInGroup("enemy")){
			var enemy = area.GetParent<BasicGuardController>();
			enemy.TakeDamage(damage);
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
