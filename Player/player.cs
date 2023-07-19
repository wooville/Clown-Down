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
	private bool canInteract = false;

	private Interactable nearbyInteractable = null;


	private Sprite2D sprite;
	private Timer dashLengthTimer;
	private Timer dashCooldownTimer;

	private Control mainGUI;


	private AbilityNode[] abilities = new AbilityNode[3];

	public enum ABILITIES {NONE, DISTRACT, STUN, BOX}


    public override void _Ready()
    {
		sprite = GetNode<Sprite2D>("Sprite2D");

        abilities[0] = GetNode<AbilityNode>("Ability1");
		abilities[1] = GetNode<AbilityNode>("Ability2");
		abilities[2] = GetNode<AbilityNode>("Ability3");

		dashLengthTimer = GetNode<Timer>("DashLengthTimer");
		dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");

		mainGUI = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("MainGUI");
    }

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 dashDirection = Vector2.Zero;

		if (Input.IsActionJustPressed("ability_1") && abilities[0].canUseAbility){
			abilities[0].UseAbility();
		}
		
		if (Input.IsActionJustPressed("ability_2") && abilities[1].canUseAbility){
			abilities[1].UseAbility();
		}

		if (Input.IsActionJustPressed("ability_3") && abilities[2].canUseAbility){
			abilities[2].UseAbility();
		}

		// Get the input direction and handle the movement/deceleration
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		

		if (Input.IsActionJustPressed("dash_interact")){
			
			if (canInteract){
				if (nearbyInteractable != null) nearbyInteractable.Interact();
			}
			else if (canDash){
				velocity = direction.Normalized() * Speed * DashSpeed;
				if (dashLengthTimer.IsStopped()) dashLengthTimer.Start();
				if (dashCooldownTimer.IsStopped()) dashCooldownTimer.Start();			

				mainGUI.Visible = false;
				
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

	private void _on_dash_length_timer_timeout(){
		dashing = false;
		mainGUI.Visible = true;
	}

	private void _on_dash_cooldown_timer_timeout(){
		canDash = true;
	}

	private void _on_interact_area_entered(Area2D area){
		if (area.IsInGroup("interactable")) {
			var interactable = (Interactable) area;
			interactable.isInteractable = true;
			canInteract = true;
			nearbyInteractable = interactable;
		}
	}

	private void _on_interact_area_exited(Area2D area){
		if (area.IsInGroup("interactable")) {
			var interactable = (Interactable) area;
			interactable.isInteractable = false;
			canInteract = false;
			nearbyInteractable = null;
		}
	}
}



