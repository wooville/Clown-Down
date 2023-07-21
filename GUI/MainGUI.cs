using Godot;
using System;

public partial class MainGUI : Control
{
	private AbilityPanel[] abilityPanels = new AbilityPanel[3];
	private AbilityPanel mainAbilityPanel;
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// var player_max_health = $"../Characters/Player".max_health
		// bar.max_value = player_max_health
		player = (Player) GetTree().GetFirstNodeInGroup("player");
		mainAbilityPanel = GetNode<AbilityPanel>("AbilityMarginContainer/HBoxContainer/MainAbilityPanel");
		abilityPanels[0] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel1");
		abilityPanels[1] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel2");
		abilityPanels[2] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel3");
		updateAbilityPanels();
	}

	private void updateAbilityPanels(){
		mainAbilityPanel.ability = player.mainAbility.ability;
		mainAbilityPanel.updatePanel();

		for (int i = 0; i < player.abilities.Length; i++){
			abilityPanels[i].ability = player.abilities[i].ability;
			// GD.Print(player.abilities[i].ability);
			// if (i == player.selectedAbility){
			// 	abilityPanels[i].isSelected = true;
			// } else {
			// 	abilityPanels[i].isSelected = false;
			// }
			abilityPanels[i].updatePanel();
		}
	}

	private void _on_player_ability_swapped_signal(){
		updateAbilityPanels();
	}
}
