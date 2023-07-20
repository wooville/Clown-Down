using Godot;
using System;

public class AttackState : State
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
			character.ChangeState(new SleepState());
		}
		else if (character.CanSeePlayer && character.WithinRange){
			character.Attack();
		}
		else if (character.CanDetectPlayer && character.CanSeePlayer){
			character.ChangeState(new PursueState());
		}
		else{
			character.ChangeState(new SearchState());
		}
	}
}
