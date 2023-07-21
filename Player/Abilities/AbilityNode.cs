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
	private PackedScene honk = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Honk.tscn");
	private PackedScene gag = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Gag.tscn");


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
			case Player.ABILITIES.GAG:
				Gag();
				break;
			case Player.ABILITIES.HONK:
				Honk();
				break;
			case Player.ABILITIES.DISTRACT:
				Distract();
				break;
			case Player.ABILITIES.SPIN:
				Spin();
				break; 
			case Player.ABILITIES.STUN:
				Stun();
				break;
		}
		// player.fixAbilitiesOrder();

		player.abilities[2] = player.abilities[1];
		player.abilities[1] = player.abilities[0];
		player.abilities[0].ability = Player.ABILITIES.NONE;
	}
	
	// private void Dash(){
	// 	player.dashing = true;
	// }

	private void Honk(){
		Node2D newHonk = honk.Instantiate<Node2D>();
		// newHonk.Position = GlobalPosition;
		newHonk.Rotation = GetAngleTo(GetGlobalMousePosition()) + 1.5708f;
		player.AddChild(newHonk);
	}

	private void Distract(){
		Node2D newDistraction = distraction.Instantiate<Node2D>();
		newDistraction.GlobalPosition = GlobalPosition;
		world.AddChild(newDistraction);
	}

	private void Gag(){
		Node2D newGag = gag.Instantiate<Node2D>();
		// newHonk.Position = GlobalPosition;
		newGag.Rotation = GetAngleTo(GetGlobalMousePosition()) + 1.5708f;
		player.AddChild(newGag);
	}

	private void Spin(){
		
	}

	private void Stun(){

	}

	private void _on_cooldown_timer_timeout(){
		canUseAbility = true;
	}
}
