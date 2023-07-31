using Godot;
using System;

public partial class Chicken : Node2D
{
	private int damage = 1;

	private void _on_attack_area_entered(Area2D area){
		if (area.IsInGroup("enemy")){
			var enemy = area.GetParent<BasicGuardController>();
			enemy.TakeDamage(damage);
		}
	}

	private void _on_timer_timeout(){
		QueueFree();
	}
}
