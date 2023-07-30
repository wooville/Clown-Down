using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	[Export]
	public float speed = 60.0f;
	[Export]
	public float dashSpeed = 3.0f;
	[Export]
	public float sillySpeed = 100.0f;
	private int hp = 3;
	private bool dashing = false;
	private bool canDash = true;
	private bool canPerformAction = true;
	public int sillyProgress {get;set;} = 0;
	private double sillyDiminish = 0.0;
	public bool silly = false;

	public bool hasKey {get;set;} = false;

	private int selectedInteractable = 0;
	private List<Interactable> nearbyInteractables = new List<Interactable>();
	private Sprite2D playerSprite;
	// private Control pointerControl;
	private Timer dashLengthTimer;
	private Timer dashCooldownTimer;
	private Timer parryTimer;
	private Timer actionCooldownTimer;
	private Timer parryPauseTimer;
	private Area2D parryArea;
	private CollisionShape2D collisionShape;

	public AbilityNode mainAbility {get; set;}
	public AbilityNode[] abilities {get; set;} = new AbilityNode[3];
	// public UPGRADES[] upgrades {get; set;} = new UPGRADES[3];
	public List<UPGRADES> upgrades {get; set;} = new List<UPGRADES>();

	public enum ABILITIES {NONE, GAG, HONK, DISTRACT, SPIN, STUN, BOX}
	public enum UPGRADES {NONE, WHOOPIE_CUSHION, PIE, GUN, HORN, LOLLIPOP, FLOWER}
	private enum SIDES {NONE, LEFT, UP, RIGHT, DOWN};
	private SIDES parryingOnSide = SIDES.NONE;

	private PackedScene sound = (PackedScene) ResourceLoader.Load("res://World/Sound.tscn");
	private PackedScene chicken = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Chicken.tscn");
	private Node2D world;

	// private PackedScene parry = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Parry.tscn");


	[Signal]
	public delegate void HealthChangedEventHandler(int newHealth);
	[Signal]
	public delegate void DiedSignalEventHandler();
	[Signal]
	public delegate void UpdateGUIEventHandler();

	public override void _Ready()
	{
		playerSprite = GetNode<Sprite2D>("PlayerSprite");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		// pointerControl = GetNode<Control>("PointerControl");

		mainAbility = GetNode<AbilityNode>("MainAbility");
		abilities[0] = GetNode<AbilityNode>("SecondaryAbility1");
		abilities[1] = GetNode<AbilityNode>("SecondaryAbility2");
		abilities[2] = GetNode<AbilityNode>("SecondaryAbility3");

		dashLengthTimer = GetNode<Timer>("DashLengthTimer");
		dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");

		parryTimer = GetNode<Timer>("ParryTimer");
		actionCooldownTimer = GetNode<Timer>("ActionCooldownTimer");
		parryPauseTimer = GetNode<Timer>("ParryPauseTimer");
		parryPauseTimer.ProcessMode = Node.ProcessModeEnum.Always;

		parryArea = GetNode<Area2D>("ParryArea");

		world = GetParent<Node2D>();

		// EmitSignal(SignalName.UpdateGUI);
	}

	public override async void _Process(double delta){
		if (silly){
			sillyDiminish += (5 * delta);
			if (sillyDiminish > 5.0){
				sillyProgress -= (int)sillyDiminish;
				sillyDiminish = 0;
				if(sillyProgress < 0){
					silly = false;
					sillyProgress = 0;
				}
				// GD.Print(sillyProgress);
				EmitSignal(SignalName.UpdateGUI);
			}
		}
		
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
				velocity = (silly) ? direction.Normalized() * sillySpeed * dashSpeed : direction.Normalized() * speed * dashSpeed;
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
			velocity = (silly) ? direction.Normalized() * sillySpeed : direction.Normalized() * speed;
		}

		if (Input.IsActionJustPressed("action_left")){
			if (canPerformAction){
				if (silly){
					chickenSlap(SIDES.LEFT);
				} else {
					tryParryOnSide(SIDES.LEFT);
				}
			}
		}

		if (Input.IsActionJustPressed("action_up")){
			if (canPerformAction){
				if (silly){
					chickenSlap(SIDES.UP);
				} else {
					tryParryOnSide(SIDES.UP);
				}
			}
		}

		if (Input.IsActionJustPressed("action_right")){
			if (canPerformAction){
				if (silly){
					chickenSlap(SIDES.RIGHT);
				} else {
					tryParryOnSide(SIDES.RIGHT);
				}
			}
		}

		if (Input.IsActionJustPressed("action_down")){
			if (canPerformAction){
				if (silly){
					chickenSlap(SIDES.DOWN);
				} else {
					tryParryOnSide(SIDES.DOWN);
				}
			}
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
		// pointerControl.Rotation = GetAngleTo(GetGlobalMousePosition()) + 1.5708f;

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

	private void tryParryOnSide(SIDES side){
		GD.Print("PARRY " + side);
		if (parryTimer.IsStopped()) parryTimer.Start();
		if (actionCooldownTimer.IsStopped()) actionCooldownTimer.Start();

		canPerformAction = false;
		parryingOnSide = side;
		parryArea.SetDeferred(Area2D.PropertyName.Monitoring, true);
	}

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
		}
	}

	private void _on_parry_timer_timeout(){
		parryArea.SetDeferred(Area2D.PropertyName.Monitoring, false);
		parryingOnSide = SIDES.NONE;
	}

	private void _on_action_cooldown_timer_timeout(){
		canPerformAction = true;
	}

	private void _on_parry_pause_timer_timeout(){
		GetTree().Paused = false;
		// GD.Print("test");
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

	public void TakeDamage(int damage){
		hp -= damage;
		GD.Print(hp);
	}

	// private void _on_pointer_area_entered(Area2D area){
	// 	if (area.IsInGroup("enemy")) {
	// 		GD.Print("yeah");
	// 	}
		// if (area.IsInGroup("parryable")){
		// 	if (parrying) {
		// 		GD.Print("parried");
		// 		canPerformAction = true;
		// 		sillyProgress += 10;
		// 		actionCooldownTimer.Stop();
		// 		parryPauseTimer.Start();
		// 		EmitSignal(SignalName.UpdateGUI);
		// 		GetTree().Paused = true;
		// 	}
		// }
	// }

	private void parrySuccess(){
		// reset for next parry
		canPerformAction = true;
		parryArea.SetDeferred(Area2D.PropertyName.Monitoring, false);
		parryingOnSide = SIDES.NONE;
		parryTimer.Stop();

		// reset cooldown and gain meter as a reward
		actionCooldownTimer.Stop();
		sillyProgress += 10;

		if (sillyProgress >= 100){
			sillyProgress = 100;
			silly = true;
		}

		EmitSignal(SignalName.UpdateGUI);
		
		// start brief pause for juice
		parryPauseTimer.Start();
		GetTree().Paused = true;
	}

	private void _on_parry_area_entered(Area2D area){
		if (area.IsInGroup("parryable")){
			// check if the side user is trying to parry on matches with side that the parryable is on
			// overlap on corners
			switch (parryingOnSide){
				case SIDES.LEFT:
					GD.Print(area.GlobalPosition, " < ", collisionShape.GlobalPosition - collisionShape.GetTransform().X / 2);
					// past the left face of the collision shape defining the player's body
					// ie in the parry area but hasn't hit the player
					// assumes player collision shape and parry area are centred at same point
					if (area.GlobalPosition.X < (collisionShape.GlobalPosition - collisionShape.GetTransform().X / 2).X){
						parrySuccess();
					}
					break;
				case SIDES.UP:
					GD.Print(area.GlobalPosition, " < ", collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2);
					if (area.GlobalPosition.Y < (collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2).Y){
						parrySuccess();
					}
					break;
				case SIDES.RIGHT:
					GD.Print(area.GlobalPosition, " > ", collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2);
					if (area.GlobalPosition.X > (collisionShape.GlobalPosition + collisionShape.GetTransform().X / 2).X){
						parrySuccess();
					}
					break;
				case SIDES.DOWN:
					GD.Print(area.GlobalPosition, " > ", collisionShape.GlobalPosition + collisionShape.GetTransform().Y / 2);
					if (area.GlobalPosition.Y > (collisionShape.GlobalPosition + collisionShape.GetTransform().Y / 2).Y){
						parrySuccess();
					}
					break;
				default:
					GD.Print("ouch");
					break;
			}
		}
	}

	private void chickenSlap(SIDES side){
		Node2D newChicken = chicken.Instantiate<Node2D>();
		// newHonk.Position = GlobalPosition;
		switch (side){
			case SIDES.LEFT:
				newChicken.RotationDegrees = -90;
				newChicken.Position -= collisionShape.GetTransform().X / 2;
				break;
			case SIDES.UP:
				newChicken.RotationDegrees = 0;
				newChicken.Position -= collisionShape.GetTransform().Y / 2;
				break;
			case SIDES.RIGHT:
				newChicken.RotationDegrees = 90;
				newChicken.Position += collisionShape.GetTransform().X / 2;
				break;
			case SIDES.DOWN:
				newChicken.RotationDegrees = 180;
				newChicken.Position += collisionShape.GetTransform().Y / 2;
				break;
		}
		
		AddChild(newChicken);
		canPerformAction = false;
		if (actionCooldownTimer.IsStopped()) actionCooldownTimer.Start();
	}

	private void _on_jail_cell_upgrade_choice(UPGRADES choice1, UPGRADES choice2){
		if (!parryPauseTimer.IsStopped()) parryPauseTimer.Stop();
	}

	private void _on_choice_menu_upgrade_acquired(Player.UPGRADES upgrade){
		upgrades.Add(upgrade);
		EmitSignal(SignalName.UpdateGUI);
	}
}



