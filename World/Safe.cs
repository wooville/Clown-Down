using Godot;
using System;

public partial class Safe : Interactable
{
	private PackedScene abilityPickup = (PackedScene) ResourceLoader.Load("res://World/AbilityPickup.tscn");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void Interact(){
		var random = new Random();
		var newAbilityPickup = (AbilityPickup) abilityPickup.Instantiate();
		var abilitiesList = Enum.GetValues(typeof(Player.ABILITIES));
		newAbilityPickup.ability = (Player.ABILITIES) abilitiesList.GetValue(random.Next(abilitiesList.Length));
		newAbilityPickup.Position = new Vector2(GlobalPosition.X - 24, GlobalPosition.Y);
		AddSibling(newAbilityPickup);
		GD.Print(newAbilityPickup.ability);
	}
}
