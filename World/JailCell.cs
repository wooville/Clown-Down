using Godot;
using System;

public partial class JailCell : Interactable
{
	[Signal]
	public delegate void UpgradeChoiceEventHandler(Player.UPGRADES choice1, Player.UPGRADES choice2);

	public override void Interact(){
		GetTree().Paused = true;
		var random = new Random();
		var upgradesList = Enum.GetValues(typeof(Player.UPGRADES));
		Player.UPGRADES choice1 = (Player.UPGRADES) upgradesList.GetValue(random.Next(upgradesList.Length));
		Player.UPGRADES choice2 = (Player.UPGRADES) upgradesList.GetValue(random.Next(upgradesList.Length));
		EmitSignal(SignalName.UpgradeChoice, (int) choice1, (int) choice2);
		base.Interact();
	}
}
