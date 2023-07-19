using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public const float Speed = 100.0f;
	[Export]
	public const float DashSpeed = 3.0f;
	public bool dashing
	{get; set;} = false;

	private Sprite2D sprite;
	private Timer dashLengthTimer;

	private Control mainGUI;


	private AbilityNode[] abilities = new AbilityNode[3];

	public enum ABILITIES {NONE, DASH, DISTRACT, STUN, BOX}


    public override void _Ready()
    {
		sprite = GetNode<Sprite2D>("Sprite2D");

        abilities[0] = GetNode<AbilityNode>("Ability1");
		abilities[1] = GetNode<AbilityNode>("Ability2");
		abilities[2] = GetNode<AbilityNode>("Ability3");

		dashLengthTimer = GetNode<Timer>("DashLengthTimer");

		mainGUI = GetNode<Control>("MainGUI");
    }

	public override void _PhysicsProcess(double delta)
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

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		velocity = direction.Normalized() * Speed;

		if (dashing){
			dashDirection = velocity * DashSpeed;
			velocity = dashDirection;

			if (dashLengthTimer.IsStopped()) dashLengthTimer.Start();
			
			// velocity = GlobalPosition.Lerp(dashDirection, 0.5f);
			// dashing = false;
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
	}

	private void _on_interact_area_entered(Area2D area){
		if (area.IsInGroup("safe")){
			var safe = (Safe) area;
			safe.Open();
		}
	}
}



