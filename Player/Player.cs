using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	[Export]
	public float speed = 60.0f;
	[Export]
	public float sillySpeed = 100.0f;
	[Export]
	public float dashSpeed = 3.0f;
	
	public int hp = 3;
	private bool dashing = false;
	private bool canDash = true;
	private bool canPerformAction = true;
	public int sillyProgress {get;set;} = 0;
	private double sillyDiminish = 0.0;	// accumulates with delta before being used to diminish silly meter during silly time
	private double sillyMeterLostPerSecond = 10;
	public bool silly = false;	// "silly mode" - allows user to attack guards when sillyProgress reaches 100 from parrying

	// public bool hasKey {get;set;} = false;
	public int keys {get;set;} = 0;

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

	// public AbilityNode mainAbility {get; set;}
	// public AbilityNode[] abilities {get; set;} = new AbilityNode[3];
	// public UPGRADES[] upgrades {get; set;} = new UPGRADES[3];
	public List<UPGRADES> upgrades {get; set;} = new List<UPGRADES>();

	public enum ABILITIES {NONE, GAG, HONK, DISTRACT, SPIN, STUN, BOX}
	public enum UPGRADES {NONE, WHOOPIE_CUSHION, PIE, GUN, HORN, LOLLIPOP, FLOWER}
	public enum SIDES {LEFT, UP, RIGHT, DOWN, NONE};
	private SIDES parryingOnSide = SIDES.NONE;
	private TextureRect[] parryTextures = new TextureRect[4];

	private PackedScene sound = (PackedScene) ResourceLoader.Load("res://World/Sound.tscn");
	private PackedScene chicken = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Chicken.tscn");
	private PackedScene pie = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Pie.tscn");

	private Node2D world;

	// private PackedScene parry = (PackedScene) ResourceLoader.Load("res://Player/Abilities/Parry.tscn");

	[Signal]
	public delegate void DiedEventHandler();
	[Signal]
	public delegate void UpdateGUIEventHandler();

	public override void _Ready()
	{
		playerSprite = GetNode<Sprite2D>("PlayerSprite");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		dashLengthTimer = GetNode<Timer>("DashLengthTimer");
		dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");

		parryTimer = GetNode<Timer>("ParryTimer");
		actionCooldownTimer = GetNode<Timer>("ActionCooldownTimer");
		parryPauseTimer = GetNode<Timer>("ParryPauseTimer");
		parryPauseTimer.ProcessMode = Node.ProcessModeEnum.Always;

		parryArea = GetNode<Area2D>("ParryArea");
		parryTextures[(int) SIDES.LEFT] = GetNode<TextureRect>("ParryTextureLeft");
		parryTextures[(int) SIDES.UP] = GetNode<TextureRect>("ParryTextureUp");
		parryTextures[(int) SIDES.RIGHT] = GetNode<TextureRect>("ParryTextureRight");
		parryTextures[(int) SIDES.DOWN] = GetNode<TextureRect>("ParryTextureDown");

		world = (Node2D) GetTree().GetFirstNodeInGroup("level");
		// upgrades.Add(UPGRADES.WHOOPIE_CUSHION);
		// sillyProgress = 90;
		// EmitSignal(SignalName.UpdateGUI);
	}

	public override async void _Process(double delta){
		if (silly){
			sillyDiminish += (sillyMeterLostPerSecond * delta);
			if (sillyDiminish > sillyMeterLostPerSecond){
				sillyProgress -= (int) sillyDiminish;
				sillyDiminish = 0;
				if(sillyProgress < 0){
					GetTree().CallGroup("main_gui", "sillyTimeOver");
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

		// Get the input direction and handle the movement/deceleration
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		

		// interact if possible, else dash
		if (Input.IsActionJustPressed("dash_interact")){
			// interact with object if possible
			if (nearbyInteractables.Count > 0){
				// if (nearbyInteractables[selectedInteractable].IsInGroup("ability_pickup")){
				// 	if (abilities[0].ability == Player.ABILITIES.NONE) tryQueueAbility();
				// 	else GD.Print("abilities full");
				// } else {
				nearbyInteractables[selectedInteractable].Interact();
				// }
			}
			// dash will speed up movement for very short duration
			else if (canDash){
				velocity = (silly) ? direction.Normalized() * sillySpeed * dashSpeed : direction.Normalized() * speed * dashSpeed;
				if (dashLengthTimer.IsStopped()) dashLengthTimer.Start();
				if (dashCooldownTimer.IsStopped()) dashCooldownTimer.Start();			

				// velocity = GlobalPosition.Lerp(dashDirection, 0.5f);
				canDash = false;
				dashing = true;
				GetTree().CallGroup("main_gui", "usedCooldown", "dash");
			}
			EmitSignal(SignalName.UpdateGUI);
		} else if (!dashing) {
			// normal movement
			velocity = (silly) ? direction.Normalized() * sillySpeed : direction.Normalized() * speed;
		}

		if (Input.IsActionJustPressed("action_left")){
			if (canPerformAction){
				if (silly){
					sillyAttack(SIDES.LEFT);
				} else {
					tryParryOnSide(SIDES.LEFT);
				}
				GetTree().CallGroup("main_gui", "usedCooldown", "action");
			}
		}

		if (Input.IsActionJustPressed("action_up")){
			if (canPerformAction){
				if (silly){
					sillyAttack(SIDES.UP);
				} else {
					tryParryOnSide(SIDES.UP);
				}
				GetTree().CallGroup("main_gui", "usedCooldown", "action");
			}
		}

		if (Input.IsActionJustPressed("action_right")){
			if (canPerformAction){
				if (silly){
					sillyAttack(SIDES.RIGHT);
				} else {
					tryParryOnSide(SIDES.RIGHT);
				}
				GetTree().CallGroup("main_gui", "usedCooldown", "action");
			}
		}

		if (Input.IsActionJustPressed("action_down")){
			if (canPerformAction){
				if (silly){
					sillyAttack(SIDES.DOWN);
				} else {
					tryParryOnSide(SIDES.DOWN);
				}
				GetTree().CallGroup("main_gui", "usedCooldown", "action");
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
		// GD.Print("PARRY " + side);
		if (parryTimer.IsStopped()) parryTimer.Start();
		if (actionCooldownTimer.IsStopped()) actionCooldownTimer.Start();

		parryTextures[(int) side].CallDeferred(MethodName.SetVisible, true);

		canPerformAction = false;
		parryingOnSide = side;
		parryArea.SetDeferred(Area2D.PropertyName.Monitoring, true);
	}

	// private void tryQueueAbility(){
	// 	if (nearbyInteractables[selectedInteractable] != null && nearbyInteractables[selectedInteractable].IsInGroup("ability_pickup") && abilities[0].ability == Player.ABILITIES.NONE){
	// 		// var nearbyPickup = (AbilityPickup) nearbyInteractable;
	// 		var nearbyPickup = (AbilityPickup) nearbyInteractables[selectedInteractable];
			
	// 		// put ability in first available slot from the bottom (end)
	// 		for (int i = abilities.Length-1; i >= 0 ; i--){
	// 			if (abilities[i].ability == ABILITIES.NONE){
	// 				abilities[i].ability = nearbyPickup.ability;
	// 				nearbyPickup.Interact();
	// 				break;
	// 			}
	// 		}
	// 	}
	// }

	private void _on_parry_timer_timeout(){
		parryArea.SetDeferred(Area2D.PropertyName.Monitoring, false);
		parryTextures[(int) parryingOnSide].CallDeferred(MethodName.SetVisible, false);
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
		// GD.Print(hp);
		EmitSignal(SignalName.UpdateGUI);

		if (hp <= 0){
			if (!parryPauseTimer.IsStopped()) parryPauseTimer.Stop();
			GetTree().Paused = true;
			GetTree().CallGroup("main_gui", "stopTimerAndGetTotalTime");
			GetTree().CallGroup("game_over_gui", "gameOver");
		}
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
		if (upgrades.Contains(UPGRADES.HORN)){
			sillyProgress += 20;
		} else {
			sillyProgress += 10;
		}

		if (sillyProgress >= 100){
			GetTree().CallGroup("main_gui", "sillyTime", upgrades.Contains(UPGRADES.PIE));
			sillyProgress = 100;
			silly = true;
		}

		GetTree().CallGroup("main_gui", "finishCooldown", "action");
		EmitSignal(SignalName.UpdateGUI);
		
		// start brief pause for juice
		parryPauseTimer.Start();
		GetTree().Paused = true;
	}

	private void _on_parry_area_entered(Area2D area){
		if (area.IsInGroup("parryable")){
			
			// switch (parryingOnSide){
			// 	case SIDES.LEFT:
			// 		// GD.Print(area.GlobalPosition, " < ", collisionShape.GlobalPosition - collisionShape.GetTransform().X / 2);
					
			// 		if (area.GlobalPosition.X < (collisionShape.GlobalPosition - collisionShape.GetTransform().X / 2).X){
			// 			parrySuccess();
			// 			if (upgrades.Contains(UPGRADES.FLOWER)) ((Projectile1) area).redirectProjectile();
			// 		}
			// 		break;
			// 	case SIDES.UP:
			// 		// GD.Print(area.GlobalPosition, " < ", collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2);
			// 		if (area.GlobalPosition.Y < (collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2).Y){
			// 			parrySuccess();
			// 			if (upgrades.Contains(UPGRADES.FLOWER)) ((Projectile1) area).redirectProjectile();
			// 		}
			// 		break;
			// 	case SIDES.RIGHT:
			// 		// GD.Print(area.GlobalPosition, " > ", collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2);
			// 		if (area.GlobalPosition.X > (collisionShape.GlobalPosition + collisionShape.GetTransform().X / 2).X){
			// 			parrySuccess();
			// 			if (upgrades.Contains(UPGRADES.FLOWER)) ((Projectile1) area).redirectProjectile();
			// 		}
			// 		break;
			// 	case SIDES.DOWN:
			// 		// GD.Print(area.GlobalPosition, " > ", collisionShape.GlobalPosition + collisionShape.GetTransform().Y / 2);
			// 		if (area.GlobalPosition.Y > (collisionShape.GlobalPosition + collisionShape.GetTransform().Y / 2).Y){
			// 			parrySuccess();
			// 			if (upgrades.Contains(UPGRADES.FLOWER)) ((Projectile1) area).redirectProjectile();
			// 		}
			// 		break;
			// 	default:
			// 		GD.Print("ouch");
			// 		break;
			// }

			// check if the side user is trying to parry on matches with side that the parryable is on
			// overlap on corners
			// past the [side] face of the collision shape defining the player's body
			// ie in the parry area but hasn't hit the player
			// assumes player collision shape and parry area are centred at same point
			if ((parryingOnSide == SIDES.LEFT && area.GlobalPosition.X < (collisionShape.GlobalPosition - collisionShape.GetTransform().X / 2).X)	|| 
				(parryingOnSide == SIDES.UP && area.GlobalPosition.Y < (collisionShape.GlobalPosition - collisionShape.GetTransform().Y / 2).Y)		||
				(parryingOnSide == SIDES.RIGHT && area.GlobalPosition.X > (collisionShape.GlobalPosition + collisionShape.GetTransform().X / 2).X)	||
				(parryingOnSide == SIDES.DOWN && area.GlobalPosition.Y > (collisionShape.GlobalPosition + collisionShape.GetTransform().Y / 2).Y)	){
					var projectile = ((Projectile1) area);
					if (upgrades.Contains(UPGRADES.FLOWER)){
						projectile.redirectProjectile(parryingOnSide);
					} else {
						projectile.QueueFree();
					}
					parryTextures[(int) parryingOnSide].CallDeferred(MethodName.SetVisible, false);
					parrySuccess();
			}
		}
	}

	private void sillyAttack(SIDES side){
		if (upgrades.Contains(UPGRADES.PIE)){
			throwPie(side);
		} else {
			chickenSlap(side);
		}
		canPerformAction = false;
		if (actionCooldownTimer.IsStopped()) actionCooldownTimer.Start();
	}

	private void throwPie(SIDES side){
		Pie newPie = pie.Instantiate<Pie>();
		newPie.Position = GlobalPosition;
		switch (side){
			case SIDES.LEFT:
				newPie.RotationDegrees = -90;
				newPie.direction = Vector2.Left;
				// newChicken.Position -= collisionShape.GetTransform().X / 2;
				break;
			case SIDES.UP:
				newPie.RotationDegrees = 0;
				newPie.direction = Vector2.Up;
				// newPie.Position -= collisionShape.GetTransform().Y / 2;
				break;
			case SIDES.RIGHT:
				newPie.RotationDegrees = 90;
				newPie.direction = Vector2.Right;
				// newChicken.Position += collisionShape.GetTransform().X / 2;
				break;
			case SIDES.DOWN:
				newPie.RotationDegrees = 180;
				newPie.direction = Vector2.Down;
				// newChicken.Position += collisionShape.GetTransform().Y / 2;
				break;
		}

		// if (upgrades.Contains(UPGRADES.HORN)){
		// 	newPie.Scale *= 2f;
		// }
		
		world.AddChild(newPie);
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

		// if (upgrades.Contains(UPGRADES.HORN)){
		// 	newChicken.Scale *= 2f;
		// }
		
		AddChild(newChicken);
	}

	private void levelSpawned(){
		world = (Node2D) GetTree().GetFirstNodeInGroup("level");
	}

	private void _on_jail_cell_upgrade_choice(UPGRADES choice1, UPGRADES choice2){
		if (!parryPauseTimer.IsStopped()) parryPauseTimer.Stop();
	}

	private void _on_choice_menu_upgrade_acquired(Player.UPGRADES upgrade){
		upgrades.Add(upgrade);

		if (upgrade == UPGRADES.GUN){
			parryArea.CallDeferred(MethodName.SetScale, new Vector2(1.25f,1.25f));

			parryTextures[(int) SIDES.LEFT].CallDeferred(MethodName.SetScale, new Vector2(1.5f,1.25f));
			parryTextures[(int) SIDES.LEFT].CallDeferred(MethodName.SetPosition, new Vector2(-6,-10));

			parryTextures[(int) SIDES.UP].CallDeferred(MethodName.SetScale, new Vector2(1.2f,2.0f));
			parryTextures[(int) SIDES.UP].CallDeferred(MethodName.SetPosition, new Vector2(-6,-10));

			parryTextures[(int) SIDES.RIGHT].CallDeferred(MethodName.SetScale, new Vector2(1.5f,1.25f));
			parryTextures[(int) SIDES.RIGHT].CallDeferred(MethodName.SetPosition, new Vector2(3,-10));

			parryTextures[(int) SIDES.DOWN].CallDeferred(MethodName.SetScale, new Vector2(1.2f,2.0f));
			parryTextures[(int) SIDES.DOWN].CallDeferred(MethodName.SetPosition, new Vector2(-6,6));
		} else if (upgrade == UPGRADES.LOLLIPOP){
			sillyMeterLostPerSecond = 5;
		} else if (upgrade == UPGRADES.WHOOPIE_CUSHION){
			dashSpeed *= 2;
		}

		EmitSignal(SignalName.UpdateGUI);
	}
}



