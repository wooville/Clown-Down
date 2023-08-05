using Godot;
using System;

public partial class Chicken : Area2D
{
	private int damage = 1;

	private void _on_area_entered(Area2D area){
		if (area.IsInGroup("enemy")){
			var enemy = area.GetParent<BasicGuardController>();
			enemy.TakeDamage(damage);
		}
		else if (area.IsInGroup("projectile")){
			area.QueueFree();
		}
	}

	private void _on_timer_timeout(){
		QueueFree();
	}
}
