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
	[Export] public int maxHealth = 1;
	[Export] public float moveSpeed = 10.0f;
	[Export] public float turnSpeed = 10.0f;
	[Export] public int attackPattern = 1;

	// [Export] private Vector2 facing = new Vector2(0,1);
	[Export] private CharacterBody2D Target;
	[Export] private TileMap tileMap;
	[Export] private PackedScene projectile  = (PackedScene) ResourceLoader.Load("res://Enemies/Attacks/Projectile1.tscn");

	[Export] private Vector2[] patrolPoints;

	private RayCast2D lineOfSight;
	private Area2D visionCone;
	private Area2D attackZone;
	private Timer attackCoolDown; 
	private bool isControllable = true;
	private State currentState;
	private bool isDead = false;
	private bool canSeePlayer = false;
	private bool canDetectPlayer = false;
	private bool withinRange = false;
	// private float searchTimer = 0.0f; 
	private bool isSearching = false;
	private float sleepTimer = 0.0f;
	private bool isAsleep = false;
	public int currentHealth = 1;
	private double myDelta = 0;
	private bool canAttack = true;

	private Vector2 lastSeen = new Vector2(0f,0f);
	private Vector2 startingPosition;
	private Vector2 nextPatrolTarget;
	private int currentPatrolIndex;

	/*********** Get and Set Functions ***********/
	// public Vector2 Facing{
	// 	get{return facing;}
	// 	set{facing = value;}
	// }
	
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

	// public float SearchTimer{
	// 	get{return searchTimer;}
	// 	set{searchTimer = value;}
	// }

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
	public PointLight2D light {get;set;}
	public Timer searchTimer {get;set;}
	private Node2D world;
	
	/*********** Godot Functions ************/

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChangeState(new IdleState());

		Target = (Player) GetTree().GetFirstNodeInGroup("player");
		tileMap = (TileMap) GetTree().GetFirstNodeInGroup("tilemap");
		
		lineOfSight = GetNode<RayCast2D>("LineOfSight");
		visionCone = GetNode<Area2D>("Vision");
		attackCoolDown = GetNode<Timer>("AttackTimer");

		Vector2 visionScale = new Vector2(1,1);
		visionScale *= sightRange; 
		visionCone.Scale = (visionScale);

		attackZone = GetNode<Area2D>("AttackZone");
		attackZone.Scale *= attackRange;

		nav = GetNode<NavigationAgent2D>("NavigationAgent2D");
		nav.SetNavigationMap(tileMap.GetNavigationMap(0));

		light = GetNode<PointLight2D>("PointLight2D");

		if (patrolPoints == null) patrolPoints = new Vector2[] {Vector2.Zero}; 
		startingPosition = GlobalPosition.Round();
		nextPatrolTarget = GlobalPosition.Round() + patrolPoints[0];

		searchTimer = GetNode<Timer>("SearchTimer");
		world = GetParent<Node2D>();

		randomizeFacing();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// MyDelta = delta;

		if (!isControllable)
			return;
		currentState.Execute(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		MyDelta = delta;

		// if (!isControllable)
		// 	return;
		// currentState.Execute(this);
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
				lastSeen = Target.GlobalPosition;
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
		if (searchTimer.IsStopped())
			isSearching = false;
		// else
		// 	isSearching = false; 
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
		if (patrolPoints[currentPatrolIndex] != Vector2.Zero){
			float angle = visionCone.GetAngleTo(nextPatrolTarget) - 1.5f;
			visionCone.Rotate(angle*((float)(MyDelta))*turnSpeed);

			// move to next patrol point after reaching target
			if ((GlobalPosition - nextPatrolTarget).Round().IsZeroApprox()){
				currentPatrolIndex++;
				currentPatrolIndex %= patrolPoints.Length;
				startingPosition = GlobalPosition.Round();
				nextPatrolTarget = startingPosition + patrolPoints[currentPatrolIndex];
				// GD.Print(nextPatrolTarget);
			}

			nav.TargetPosition = nextPatrolTarget;
			
			var direction = nav.GetNextPathPosition() - GlobalPosition;
			direction = direction.Normalized();
			Velocity = direction *( (float)MyDelta * moveSpeed);
			MoveAndSlide();
		}
		
	}

	public void BeDead(){
		//does nothing currently
		GetTree().CallGroup("manager", "guardGoofed");
		QueueFree();
	}
	public void BeAsleep(){
		//does nothing currently
	}

	public void Pursue(){
		//rotate vision cone
		float angle = visionCone.GetAngleTo(Target.GlobalPosition) - 1.5f;
		visionCone.Rotate(angle*((float)(MyDelta))*turnSpeed);

		
		nav.TargetPosition = Target.GlobalPosition;
		
		var direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
		Velocity = direction*( (float)MyDelta * moveSpeed);;
		
		// Velocity = ( Target.GlobalPosition - this.GlobalPosition ).Normalized() *( (float)MyDelta * moveSpeed);

		

		MoveAndSlide();
		//Facing = (this.GlobalPosition + Facing).Normalized();
	}

	public void Search(){
		float angle = visionCone.GetAngleTo(lastSeen) - 1.5f;
		visionCone.Rotate(angle*((float)(MyDelta))*turnSpeed);

		nav.TargetPosition = lastSeen;
		
		var direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
		Velocity = direction*( (float)MyDelta * moveSpeed);;

		MoveAndSlide();
	}

	public void FireProjectile(Vector2 direction, float offsetAngle){
		GD.Print("Fired");
		Projectile1 inst = (Projectile1)projectile.Instantiate();
		inst.Direction = direction.Rotated(offsetAngle);
		inst.GlobalPosition = this.GlobalPosition;

		world.AddChild(inst);
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

	private void _on_search_timer_timeout(){
		isSearching = false;
		GD.Print("done");
	}

	public void TakeDamage(int damage){
		currentHealth -= damage;

		if (currentHealth <= 0){
			isDead = true;
		}
	}

	public void randomizeFacing(){
		var random = new Random();
		

		var nearbyPoint = new Vector2(random.Next(-500, 500), random.Next(-500, 500));
		float angle = visionCone.GetAngleTo(Position + nearbyPoint);
		visionCone.Rotate(angle);
	}
}





















