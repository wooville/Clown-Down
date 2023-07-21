using Godot;
using System;
using System.Collections;

public partial class BasicGuardController : CharacterBody2D
{
	[Export] public float sightRange = 3.0f;
	[Export] public float attackRange = 1.0f;
	[Export] public float attackDuration = 1.0f;
	[Export] public float searchDuration = 2.0f;
	[Export] public float sleepDuration = 6.0f;
	[Export] public float maxHealth = 1.0f;

	[Export] private Vector2 facing = new Vector2(0.0f, 1.0f);
	[Export] private CharacterBody2D Target;

	private RayCast2D lineOfSight;
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
	
	/*********** Godot Functions ************/

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChangeState(new IdleState());
		
		lineOfSight = GetNode<RayCast2D>("LineOfSight");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
				CanSeePlayer = false;
			}
		}
		else{
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
		//does nothing currently
	}

	public void BeIdle(){
		//does nothing when idle
		//GD.Print("idle");
	}

	public void BeDead(){
		//does nothing currently
	}
	public void BeAsleep(){
		//does nothing currently
	}

	public void Pursue(){
		//does nothing currently
	}

	public void Search(){
		//dose nothing currentlly
	}

	private void _on_vision_body_shape_entered(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
	{
		
		//Array<StringName> temp = body.GetGroups();
		foreach(String str in body.GetGroups()){
			if (str == "player"){
				GD.Print("Player Entered");
				CanDetectPlayer = true;
			}
		}
	
	}

	private void _on_vision_body_shape_exited(Rid body_rid, Node2D body, long body_shape_index, long local_shape_index)
	{
		foreach(String str in body.GetGroups()){
			if (str == "player"){
				GD.Print("Player Exited");
				CanDetectPlayer = false;
			}
		}
	}
	
}












