using Godot;
using System;

public class SleepState : State
{
	public override void Execute(BasicGuardController character)
	{
		//run checks
		character.CheckCanDetectPlayer();
		if (character.CanDetectPlayer)
			character.CheckCanSeePlayer();
		if (character.CanSeePlayer)
			character.CheckWithinRange();
		character.CheckSearching();
		
		//change state
		if (character.IsDead){
			character.ChangeState(new DeadState());
		}
		else if (character.IsAsleep){
			character.BeAsleep();
		}
		else{
			character.ChangeState(new IdleState());
		}
	}
}
