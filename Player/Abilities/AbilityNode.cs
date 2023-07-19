using Godot;
using System;

public partial class AbilityNode : Node2D
{
	[Export]
	public Player.ABILITIES ability;
	public bool canUseAbility = true;
	private Timer cooldownTimer;
	private Player player;
	private Node2D world;
	private PackedScene distraction = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Distraction.tscn");

	[Signal]
	public delegate void AbilityUsedEventHandler(Player.ABILITIES ability, float cooldown);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<Player>();
		world = player.GetParent<Node2D>();

		cooldownTimer = GetNode<Timer>("CooldownTimer");
	}

	public void UseAbility(){
		canUseAbility = false;
		cooldownTimer.Start();
		switch (ability){
			// case Player.ABILITIES.DASH:
			// 	Dash();
			// 	break;
			case Player.ABILITIES.DISTRACT:
				Distract();
				
				break;
			case Player.ABILITIES.STUN:
				Stun();
				break;
		}
	}
	
	// private void Dash(){
	// 	player.dashing = true;
	// }

	private void Distract(){
		Node2D newDistraction = (Node2D) distraction.Instantiate();
		newDistraction.GlobalPosition = GlobalPosition;
		world.AddChild(newDistraction);
	}

	private void Stun(){
		
	}

	private void _on_cooldown_timer_timeout(){
		canUseAbility = true;
	}
}
