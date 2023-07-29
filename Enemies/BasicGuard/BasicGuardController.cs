using Godot;
using System;
using System.Collections;

public partial class BasicGuardController : CharacterBody2D
{
	[Export] public float sightRange = 1.0f;
	[Export] public float attackRange = 1.0f;
	[Export] public float attackDuration = 1.0f;
	[Export] public float searchDuration = 2.0f;
	[Export] public float sleepDuration = 1.0f;
	[Export] public float maxHealth = 1.0f;
	[Export] public float moveSpeed = 70.0f;
	[Export] public float turnSpeed = 10.0f;
	[Export] public int attackPattern = 1;


	[Export] private Vector2 facing = new Vector2(0,1);
	[Export] private CharacterBody2D Target;
	[Export] private TileMap tileMap;
	[Export] private PackedScene projectile;

	private RayCast2D lineOfSight;
	private Area2D visionCone;
	private Timer attackCoolDown; 
	private bool isControllable = true;
	private State currentState;
	private bool isDead = false;
	private bool canSeePlayer = false;
	private bool canDetectPlayer = false;
	private bool withinRange = false;
	private float searchTimer = 0.0f; 
	private bool isSearching = false;
	private float sleepTimer = 0.0f;
	private bool isAsleep = false;
	private float currentHealth;
	private double myDelta = 0;
	private bool canAttack = true;

	/*********** Get and Set Functions ***********/
	public Vector2 Facing{
		get{return facing;}
		set{facing = value;}
	}
	
	public bool IsControllable{
		get{return IsControllable;}
		set{IsControllable = value;}
	}
	public bool IsDead{
		get{return isDead;}
		set{isDead = value;}
	}

	public bool CanSeePlayer{
		get{return canSeePlayer;}
		set{canSeePlayer = value;}
	}

	public bool CanDetectPlayer{
		get{return canDetectPlayer;}
		set{canDetectPlayer = value;}
	}

	public bool WithinRange{
		get{return withinRange;}
		set{withinRange = value;}
	} 

	public float SearchTimer{
		get{return searchTimer;}
		set{searchTimer = value;}
	}

	public bool IsSearching{
		get{return isSearching;}
		set{isSearching = value;}
	}

	public float SleepTimer{
		get{return sleepTimer;}
		set{sleepTimer = value;}
	}

	public bool IsAsleep{
		get{return isAsleep;}
		set{isAsleep = value;}
	}

	public double MyDelta{
		get{return myDelta;}
		set{myDelta = value;}
	}

	public bool CanAttack{
		get{return canAttack;}
		set{canAttack = value;}
	}

	private NavigationAgent2D nav;
	
	/*********** Godot Functions ************/

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChangeState(new IdleState());
		
		lineOfSight = GetNode<RayCast2D>("LineOfSight");
		visionCone = GetNode<Area2D>("Vision");
		attackCoolDown = GetNode<Timer>("AttackTimer");

		Vector2 visionScale = new Vector2(1,1);
		visionScale *= sightRange; 
		visionCone.Scale = (visionScale);



		nav = GetNode<NavigationAgent2D>("NavigationAgent2D");
		nav.SetNavigationMap(tileMap.GetNavigationMap(0));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MyDelta = delta;

		if (!isControllable)
			return;
		currentState.Execute(this);
	}

	/*********** Update Functions ***********/

	//check if player is within sight cone
	public void CheckCanDetectPlayer(){

	}

	//check for line of sight with player
	public void CheckCanSeePlayer(){
		
		if (CanDetectPlayer == true){

			lineOfSight.TargetPosition = Target.GlobalPosition - this.GlobalPosition;
			lineOfSight.ForceRaycastUpdate();
			Object temp = lineOfSight.GetCollider();
			//GD.Print(temp);

			if (temp == Target){
				CanSeePlayer = true;
				//GD.Print("Can See Player");
			}
			else{
				//GD.Print("cant see player");
				CanSeePlayer = false;
			}
		}
		else{
			//GD.Print("cant see player");
			CanSeePlayer = false;
		}
	}
	//check if player is within attack range
	public void CheckWithinRange(){

	}

	public void CheckSearching(){
		if (searchTimer > 0.0f)
			isSearching = true;
		else
			isSearching = false; 
	}

	//changes current state
	public void ChangeState(State newState){
		currentState = newState;
	}
	
	/*********** Behaviours ***********/
	public void Attack(){
		//rotate vision cone
		float angle = visionCone.GetAngleTo(Target.GlobalPosition) - 1.5f;
		visionCone.Rotate(angle*((float)(MyDelta))*turnSpeed);

		if(canAttack == true){
			CanAttack = false;
			attackCoolDown.Start();
			Vector2 direction = (Target.GlobalPosition - this.GlobalPosition );
			switch(attackPattern){
				case 1:
					FireProjectile(direction, 0);
					break;
				case 2:
					FireProjectile(direction, 0);
					FireProjectile(direction, 0.125f);
					FireProjectile(direction, -0.125f);
					break;
				case 3:
					FireProjectile(direction, 0);
					FireProjectile(direction, 0.25f);
					FireProjectile(direction, -0.25f);
					FireProjectile(direction, 0.5f);
					FireProjectile(direction, -0.5f);
					FireProjectile(direction, 0.75f);
					FireProjectile(direction, -0.75f);
					FireProjectile(direction, 1.0f);
					break;

				case 4:
					FireProjectile(direction, 0);
					FireProjectile(direction, 0.75f);
					FireProjectile(direction, -0.75f);
					FireProjectile(direction, 1.5f);
					FireProjectile(direction, -1.5f);
					FireProjectile(direction, 2.25f);
					FireProjectile(direction, -2.25f);
					FireProjectile(direction, 3.0f);
					break;
				default:
					FireProjectile(direction, 0);
					break;
			}
		}
	}

	public void BeIdle(){
		//does nothing when idle
		// GD.Print("idle");
	}

	public void BeDead(){
		//does nothing currently
		QueueFree();
	}
	public void BeAsleep(){
		//does nothing currently
	}

	public void Pursue(){
		//rotate vision cone
		float angle = visionCone.GetAngleTo(Target.GlobalPosition) - 1.5f;
		visionCone.Rotate(angle*((float)(MyDelta))*turnSpeed);

		/*
		nav.TargetPosition = Target.GlobalPosition;
		
		var direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
		Velocity = direction*moveSpeed;
		*/
		Velocity = ( Target.GlobalPosition - this.GlobalPosition ) *( (float)MyDelta * moveSpeed);
		

		MoveAndSlide();
		//Facing = (this.GlobalPosition + Facing).Normalized();
	}

	public void Search(){
		//dose nothing currentlly
	}

	public void FireProjectile(Vector2 direction, float offsetAngle){
		GD.Print("Fired");
		Projectile1 inst = (Projectile1)projectile.Instantiate();
		inst.Direction = direction.Rotated(offsetAngle);
		inst.GlobalPosition = this.GlobalPosition;
		Owner.AddChild(inst);
	}

	private void _on_vision_body_shape_entered(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
	{
		
		//Array<StringName> temp = body.GetGroups();
		foreach(String str in body.GetGroups()){
			if (str == "player"){
				//GD.Print("Player Entered");
				CanDetectPlayer = true;
			}
		}
	
	}

	private void _on_vision_body_shape_exited(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
	{
		foreach(String str in body.GetGroups()){
			if (str == "player"){
				//GD.Print("Player Exited");
				CanDetectPlayer = false;
			}
		}
	}
	private void _on_attack_zone_body_entered(Node2D body)
	{
		// Replace with function body.
		foreach(String str in body.GetGroups()){
			if (str == "player"){
				//GD.Print("Player within attack range");
				WithinRange = true;
			}
		}
	}

	private void _on_attack_zone_body_exited(Node2D body)
	{
		// Replace with function body.
		foreach(String str in body.GetGroups()){
			if (str == "player"){
				//GD.Print("Player outside attack range");
				WithinRange = false;
			}
		}
	}

	private void _on_attack_timer_timeout()
	{
		CanAttack = true;
	}
	
}





















