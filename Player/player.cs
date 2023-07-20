using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public const float Speed = 75.0f;
	[Export]
	public const float DashSpeed = 3.0f;
	private bool dashing = false;
	private bool canDash = true;
	// private bool canInteract = false;

	private Interactable nearbyInteractable = null;


	private Sprite2D playerSprite;
	private Control pointerControl;
	private Timer dashLengthTimer;
	private Timer dashCooldownTimer;

	public AbilityNode mainAbility {get; set;}
	public AbilityNode[] abilities {get; set;} = new AbilityNode[3];

	public int selectedAbility = 0;

	public enum ABILITIES {NONE, HONK, DISTRACT, SPIN, STUN, BOX}

	[Signal]
	public delegate void HealthChangedEventHandler(int newHealth);
	[Signal]
	public delegate void DiedSignalEventHandler();
	[Signal]
	public delegate void AbilitySwappedSignalEventHandler(Player.ABILITIES ability);

    public override void _Ready()
    {
		playerSprite = GetNode<Sprite2D>("PlayerSprite");
		pointerControl = GetNode<Control>("PointerControl");

		mainAbility = GetNode<AbilityNode>("MainAbility");
        abilities[0] = GetNode<AbilityNode>("SecondaryAbility1");
		abilities[1] = GetNode<AbilityNode>("SecondaryAbility2");
		abilities[2] = GetNode<AbilityNode>("SecondaryAbility3");

		dashLengthTimer = GetNode<Timer>("DashLengthTimer");
		dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");

		EmitSignal(SignalName.AbilitySwappedSignal);
    }

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 dashDirection = Vector2.Zero;

		// if ability key is pressed, swap corresponding ability with ability pickup if one is nearby
		// if not, use ability
		// if (Input.IsActionJustPressed("ability_1")){
		// 	if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
		// 		trySwapAbility(0);
		// 	}
		// 	else if (abilities[0].canUseAbility){
		// 		abilities[0].UseAbility();
		// 	}
		// }
		
		// if (Input.IsActionJustPressed("ability_2")){
		// 	if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
		// 		trySwapAbility(1);
		// 	}
		// 	else if (abilities[1].canUseAbility){
		// 		abilities[1].UseAbility();
		// 	}
		// }

		// if (Input.IsActionJustPressed("ability_3")){
		// 	if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
		// 		trySwapAbility(2);
		// 	}
		// 	else if (abilities[2].canUseAbility){
		// 		abilities[2].UseAbility();
		// 	}
		// }

		// update ability gui indicator
		if (Input.IsActionJustPressed("cycle_selected_ability_down")){
			selectedAbility++;
			selectedAbility %= abilities.Length;
			EmitSignal(SignalName.AbilitySwappedSignal);
		}
		if (Input.IsActionJustPressed("cycle_selected_ability_up")){
			selectedAbility--;
			if (selectedAbility < 0) selectedAbility = abilities.Length-1;
			EmitSignal(SignalName.AbilitySwappedSignal);
		}

		if (Input.IsActionJustPressed("use_main_ability")){
			if (mainAbility.canUseAbility){
				mainAbility.UseAbility();
			}
		}

		if (Input.IsActionJustPressed("use_secondary_ability")){
			if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
				trySwapAbility(selectedAbility);
			}
			else if (abilities[selectedAbility].canUseAbility){
				abilities[selectedAbility].UseAbility();
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
			playerSprite.FlipH = true;
		} else {
			playerSprite.FlipH = false;
		}


		// rotate pointer GUI element to face cursor
		pointerControl.Rotation = GetAngleTo(GetGlobalMousePosition()) + 1.5708f;

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



