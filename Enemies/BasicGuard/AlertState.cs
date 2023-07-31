using Godot;
using System;

public class AlertState : State
{
	public override void Execute(BasicGuardController character)
	{
		//run checks
		character.CheckCanDetectPlayer();
		
		character.CheckCanSeePlayer();
	
		character.CheckSearching();
		
		//change state
		if (character.IsDead){
            character.EndAlert();
			character.ChangeState(new DeadState());
			//GD.Print("Entered Dead State");
		}
		else if (character.IsAsleep){
            character.EndAlert();
			character.ChangeState(new SleepState());
			//GD.Print("Entered Sleep State");
		}
		else if (character.CanSeePlayer && character.WithinRange){
            character.EndAlert();
			character.ChangeState(new AttackState());
			
		}
		else if (character.CanDetectPlayer && character.CanSeePlayer){
            character.EndAlert();
			character.ChangeState(new PursueState());
			//GD.Print("Entered Pursue State");
		}
        else if (character.IsAlert){
            character.BeAlert();
        }
		else{
			character.ChangeState(new IdleState());
			//GD.Print("Entered Idle State");
		}
	}
}
