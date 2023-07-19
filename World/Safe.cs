using Godot;
using System;

public partial class Safe : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Open(){
		var random = new Random();
		var newAbilityPickup = new AbilityPickup();
		var abilitiesList = Enum.GetValues(typeof(Player.ABILITIES));
		newAbilityPickup.ability = (Player.ABILITIES) abilitiesList.GetValue(random.Next(abilitiesList.Length));
		AddSibling(newAbilityPickup);
	}
}
