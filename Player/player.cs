using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public const float Speed = 100.0f;
	[Export]
	public const float DashSpeed = 3.0f;
	private bool dashing = false;
	private bool canDash = true;
	// private bool canInteract = false;

	private Interactable nearbyInteractable = null;


	private Sprite2D sprite;
	private Timer dashLengthTimer;
	private Timer dashCooldownTimer;

	public AbilityNode[] abilities {get; set;}= new AbilityNode[3];

	public enum ABILITIES {NONE, DISTRACT, STUN, BOX}

	[Signal]
	public delegate void HealthChangedEventHandler(int newHealth);
	[Signal]
	public delegate void DiedSignalEventHandler();
	[Signal]
	public delegate void AbilitySwappedSignalEventHandler(Player.ABILITIES ability);

    public override void _Ready()
    {
		sprite = GetNode<Sprite2D>("Sprite2D");

        abilities[0] = GetNode<AbilityNode>("Ability1");
		abilities[1] = GetNode<AbilityNode>("Ability2");
		abilities[2] = GetNode<AbilityNode>("Ability3");

		dashLengthTimer = GetNode<Timer>("DashLengthTimer");
		dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");

		// signals
    }

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 dashDirection = Vector2.Zero;

		// if ability key is pressed, swap corresponding ability with ability pickup if one is nearby
		// if not, use ability
		if (Input.IsActionJustPressed("ability_1")){
			if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
				trySwapAbility(0);
			}
			else if (abilities[0].canUseAbility){
				abilities[0].UseAbility();
			}
		}
		
		if (Input.IsActionJustPressed("ability_2")){
			if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
				trySwapAbility(1);
			}
			else if (abilities[1].canUseAbility){
				abilities[1].UseAbility();
			}
		}

		if (Input.IsActionJustPressed("ability_3")){
			if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
				trySwapAbility(2);
			}
			else if (abilities[2].canUseAbility){
				abilities[2].UseAbility();
			}
		}

		// Get the input direction and handle the movement/deceleration
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		

		// interact if possible, else dash
		if (Input.IsActionJustPressed("dash_interact")){
			if (nearbyInteractable != null && !nearbyInteractable.IsInGroup("ability_pickup")){
				nearbyInteractable.Interact();
			}
			else if (canDash){
				velocity = direction.Normalized() * Speed * DashSpeed;
				if (dashLengthTimer.IsStopped()) dashLengthTimer.Start();
				if (dashCooldownTimer.IsStopped()) dashCooldownTimer.Start();			
				
				// velocity = GlobalPosition.Lerp(dashDirection, 0.5f);
				canDash = false;
				dashing = true;
			}
		} else if (!dashing) {
			velocity = direction.Normalized() * Speed;
		}

		if (direction.X < 0){
			sprite.FlipH = true;
		} else {
			sprite.FlipH = false;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// swap ability if standing near ability pickup
	private void trySwapAbility(int index){
		if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
			var nearbyPickup = (AbilityPickup) nearbyInteractable;
			var temp = abilities[index].ability;
			abilities[index].ability = nearbyPickup.ability;
			nearbyPickup.Interact(temp);
			EmitSignal(SignalName.AbilitySwappedSignal);
		}
	}

	private void _on_dash_length_timer_timeout(){
		dashing = false;
	}

	private void _on_dash_cooldown_timer_timeout(){
		canDash = true;
	}

	private void _on_interact_area_entered(Area2D area){
		if (area.IsInGroup("interactable")) {
			var interactable = (Interactable) area;
			interactable.isInteractable = true;
			interactable.ToggleButtonPrompt();
			// canInteract = true;
			nearbyInteractable = interactable;
		}
	}

	private void _on_interact_area_exited(Area2D area){
		if (area.IsInGroup("interactable")) {
			var interactable = (Interactable) area;
			interactable.isInteractable = false;
			interactable.ToggleButtonPrompt();
			// canInteract = false;
			nearbyInteractable = null;
		}
	}
}



