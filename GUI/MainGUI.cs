using Godot;
using System;

public partial class MainGUI : Control
{
	private AbilityPanel[] abilityPanels = new AbilityPanel[3];
	private Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// var player_max_health = $"../Characters/Player".max_health
		// bar.max_value = player_max_health
		player = (Player) GetTree().GetFirstNodeInGroup("player");
		abilityPanels[0] = GetNode<AbilityPanel>("AbilityMarginContainer/HBoxContainer/AbilityPanel1");
		abilityPanels[1] = GetNode<AbilityPanel>("AbilityMarginContainer/HBoxContainer/AbilityPanel2");
		abilityPanels[2] = GetNode<AbilityPanel>("AbilityMarginContainer/HBoxContainer/AbilityPanel3");
		updateAbilityPanels();
	}

	private void updateAbilityPanels(){
		for (int i = 0; i < player.abilities.Length; i++){
			// abilityPanels[i].ability = player.abilities[i].ability;
			abilityPanels[i].updatePanel(player.abilities[i].ability);
		}
	}

	private void _on_player_ability_swapped_signal(){
		updateAbilityPanels();
	}
}
