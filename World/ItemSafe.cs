using Godot;
using System;
using System.Collections.Generic;

public partial class ItemSafe : Interactable
{
	[Signal]
	public delegate void UpgradeChoiceEventHandler(Player.UPGRADES choice1, Player.UPGRADES choice2);
	private Player player;

	public override void _Ready(){
		base._Ready();
		player = (Player) GetTree().GetFirstNodeInGroup("player");
	}

	public override void Interact(){
		GetTree().Paused = true;
		var random = new Random();
		var allUpgradesList = Enum.GetValues(typeof(Player.UPGRADES));

		var itemsLeft = new List<Player.UPGRADES>();
		// Array.Copy(upgradesList, 1, newArray, 0, newArray.Length);
		
		// populate list of options with upgrades the player doesn't have yet
		foreach (Player.UPGRADES upgrade in allUpgradesList) {
			if (upgrade != Player.UPGRADES.NONE && !player.upgrades.Contains(upgrade)){
				itemsLeft.Add(upgrade);
			}
		}

		Player.UPGRADES choice1;
		Player.UPGRADES choice2;
		do {
			choice1 = itemsLeft[random.Next(itemsLeft.Count)];
			choice2 = itemsLeft[random.Next(itemsLeft.Count)];
		} while (choice1 == choice2);
		
		// EmitSignal(SignalName.UpgradeChoice, (int) choice1, (int) choice2);
		GetTree().CallGroup("manager", "chestFound");
		GetTree().CallGroup("choice_menu", "presentChoice", (int) choice1, (int) choice2);
		
		base.Interact();
	}
}
