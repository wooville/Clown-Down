using Godot;
using System;

public partial class Safe : Interactable
{
	private PackedScene abilityPickup = (PackedScene) ResourceLoader.Load("res://World/AbilityPickup.tscn");

	public override void Interact(){
		var random = new Random();
		var newAbilityPickup = (AbilityPickup) abilityPickup.Instantiate();
		var abilitiesList = Enum.GetValues(typeof(Player.ABILITIES));
		// newAbilityPickup.ability = (Player.ABILITIES) abilitiesList.GetValue(random.Next(abilitiesList.Length));
		newAbilityPickup.ability = Player.ABILITIES.DISTRACT;
		newAbilityPickup.Position = new Vector2(GlobalPosition.X - 24, GlobalPosition.Y);
		AddSibling(newAbilityPickup);
		GD.Print(newAbilityPickup.ability);
		base.Interact();
	}
}
