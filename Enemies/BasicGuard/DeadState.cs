using Godot;
using System;

public class DeadState : State
{
	public override void Execute(BasicGuardController character)
	{
		character.BeDead();
	}
}
