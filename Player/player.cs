using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	[Export]
	public const float Speed = 90.0f;
	[Export]
	public const float DashSpeed = 3.0f;
	private bool dashing = false;
	private bool canDash = true;

	public bool hasKey {get;set;} = false;
	// private bool canInteract = false;

	private int selectedInteractable = 0;
	private List<Interactable> nearbyInteractables = new List<Interactable>();


	private Sprite2D playerSprite;
	private Control pointerControl;
	private Timer dashLengthTimer;
	private Timer dashCooldownTimer;

	public AbilityNode mainAbility {get; set;}
	public AbilityNode[] abilities {get; set;} = new AbilityNode[3];

	public enum ABILITIES {NONE, GAG, HONK, DISTRACT, SPIN, STUN, BOX}

	private PackedScene sound = (PackedScene) ResourceLoader.Load("res://World/Sound.tscn");
	private Node2D world;

	[Signal]
	public delegate void HealthChangedEventHandler(int newHealth);
	[Signal]
	public delegate void DiedSignalEventHandler();
	[Signal]
	public delegate void UpdateGUIEventHandler();

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

		world = GetParent<Node2D>();

		// EmitSignal(SignalName.AbilitySwappedSignal);
		EmitSignal(SignalName.UpdateGUI);
    }

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 dashDirection = Vector2.Zero;

		if (Input.IsActionJustPressed("use_main_ability")){
			if (mainAbility.canUseAbility){
				mainAbility.UseAbility();
			}
		}

		if (Input.IsActionJustPressed("use_secondary_ability")){
			if (abilities[abilities.Length-1].canUseAbility){
				abilities[abilities.Length-1].UseAbility();
			}
			EmitSignal(SignalName.UpdateGUI);
		}

		// Get the input direction and handle the movement/deceleration
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		

		// interact if possible, else dash
		if (Input.IsActionJustPressed("dash_interact")){
			// interact with object if possible
			if (nearbyInteractables.Count > 0){
				if (nearbyInteractables[selectedInteractable].IsInGroup("ability_pickup")){
					if (abilities[0].ability == Player.ABILITIES.NONE) tryQueueAbility();
					else GD.Print("abilities full");
				} else {
					nearbyInteractables[selectedInteractable].Interact();
				}
			}
			// dash will speed up movement for very short duration
			else if (canDash){
				velocity = direction.Normalized() * Speed * DashSpeed;
				if (dashLengthTimer.IsStopped()) dashLengthTimer.Start();
				if (dashCooldownTimer.IsStopped()) dashCooldownTimer.Start();			

				Sound newSound = (Sound) sound.Instantiate();
				// assign sound here
				newSound.GlobalPosition = GlobalPosition;
				world.AddChild(newSound);

				// velocity = GlobalPosition.Lerp(dashDirection, 0.5f);
				canDash = false;
				dashing = true;
			}
			EmitSignal(SignalName.UpdateGUI);
		} else if (!dashing) {
			// normal movement
			velocity = direction.Normalized() * Speed;
		}

		if (Input.IsActionJustPressed("cycle_interactable")){
			selectedInteractable++;
			selectedInteractable = (nearbyInteractables.Count > 0) ? selectedInteractable%nearbyInteractables.Count : 0;
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
	// private void trySwapAbility(int index){
	// 	if (nearbyInteractable != null && nearbyInteractable.IsInGroup("ability_pickup")){
	// 		var nearbyPickup = (AbilityPickup) nearbyInteractable;
	// 		var temp = abilities[index].ability;
	// 		abilities[index].ability = nearbyPickup.ability;
	// 		nearbyPickup.Interact(temp);
	// 		EmitSignal(SignalName.AbilitySwappedSignal);
	// 	}
	// }

	private void tryQueueAbility(){
		if (nearbyInteractables[selectedInteractable] != null && nearbyInteractables[selectedInteractable].IsInGroup("ability_pickup") && abilities[0].ability == Player.ABILITIES.NONE){
			// var nearbyPickup = (AbilityPickup) nearbyInteractable;
			var nearbyPickup = (AbilityPickup) nearbyInteractables[selectedInteractable];
			
			// put ability in first available slot from the bottom (end)
			for (int i = abilities.Length-1; i >= 0 ; i--){
				if (abilities[i].ability == ABILITIES.NONE){
					abilities[i].ability = nearbyPickup.ability;
					nearbyPickup.Interact();
					break;
				}
			}

			// for (int i = 0; i < abilities.Length; i++){
			// 	GD.Print(abilities[i].ability);
			// }
			
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
			nearbyInteractables.Add(interactable);
			selectedInteractable = nearbyInteractables.Count-1;	// default to last item in list to interact with
		}
	}

	private void _on_interact_area_exited(Area2D area){
		if (area.IsInGroup("interactable")) {
			var interactable = (Interactable) area;
			nearbyInteractables.Remove(interactable);
			selectedInteractable = 0;
		}
	}

	private void _on_pointer_area_entered(Area2D area){
		if (area.IsInGroup("enemy")) {
			GD.Print("yeah");
		}
	}
}



