using Godot;
using System;

public class SearchState : State
{
	public override void Execute(BasicGuardController character)
	{
		//run checks
		character.CheckCanDetectPlayer();
		
		character.CheckCanSeePlayer();
		if (character.CanSeePlayer)
			character.CheckWithinRange();
		// character.CheckSearching();
		
		//change state
		if (character.IsDead){
			character.ChangeState(new DeadState());
			character.EndSearch();
			GD.Print("Entered Dead State");
		}
		else if (character.IsAsleep){
			character.ChangeState(new SleepState());
			character.EndSearch();
			GD.Print("Entered Sleep State");
		}
		else if (character.CanSeePlayer && character.WithinRange){
			character.EndSearch();
			character.ChangeState(new AttackState());
			GD.Print("Entered Attack State");
		}
		else if (character.CanDetectPlayer && character.CanSeePlayer){
			character.EndSearch();
			character.ChangeState(new PursueState());
			GD.Print("Entered Pursue State");
		}
		else if (character.IsSearching){
			character.Search();
		}
		else{
			character.StartAlert();
			character.ChangeState(new AlertState());
			GD.Print("Entered Alert State");
		}
	}
}
