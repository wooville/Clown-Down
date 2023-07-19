using Godot;
using System;
using System.Collections;

public partial class BasicGuardController : CharacterBody2D
{
	[Export] public float sightRange = 3.0f;
	public float attackRange = 1.0f;
	public float attackDuration = 1.0f;
	public float searchDuration = 2.0f;
	public float sleepDuration = 6.0f;
	public float maxHealth = 1.0f;

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

}
