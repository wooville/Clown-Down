using Godot;
using System;
using System.Collections;

public abstract class State
{
	public abstract void Execute(BasicGuardController character);
}
