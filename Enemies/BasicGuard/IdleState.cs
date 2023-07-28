using Godot;
using System;

public class IdleState : State
{
	public override void Execute(BasicGuardController character)
	{
		//run checks
		character.CheckCanDetectPlayer();
		
		character.CheckCanSeePlayer();
		
		if (character.CanSeePlayer)
			character.CheckWithinRange();
		character.CheckSearching();

		//change state
		if (character.IsDead){
			character.ChangeState(new DeadState());
			GD.Print("Entered Dead State");
		}
		else if (character.IsAsleep){
			character.ChangeState(new SleepState());
			GD.Print("Entered Sleep State");
		}
		else if (character.CanSeePlayer && character.WithinRange){
			character.ChangeState(new AttackState());
			GD.Print("Entered Attack State");
		}
		else if (character.CanDetectPlayer && character.CanSeePlayer){
			character.ChangeState(new PursueState());
			GD.Print("Entered Pursue State");
		}
		else{
			character.BeIdle();
		}
	}
}
