using Godot;
using System;
using System.Collections.Generic;

public partial class MainGUI : Control
{
	private AbilityPanel[] abilityPanels = new AbilityPanel[3];
	private List<UpgradePanel> upgradePanels = new List<UpgradePanel>();
	private List<TextureRect> healthIcons = new List<TextureRect>();
	private AbilityPanel mainAbilityPanel;
	private Player player;
	private TextureRect keyTexture;
	private Label keyNumberLabel;
	private Label timerLabel;
	private ProgressBar sillyProgressBar;
	private HBoxContainer healthContainer;
	private VBoxContainer upgradesContainer;

	public bool timerActive = false;
	public double timeElapsed {get;set;} = 0.0;

	private PackedScene upgradePanelScene = (PackedScene) ResourceLoader.Load("res://GUI/UpgradePanel.tscn");
	private PackedScene healthIcon = (PackedScene) ResourceLoader.Load("res://GUI/HealthIcon.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// var player_max_health = $"../Characters/Player".max_health
		// bar.max_value = player_max_health
		player = (Player) GetTree().GetFirstNodeInGroup("player");
		// mainAbilityPanel = GetNode<AbilityPanel>("AbilityMarginContainer/HBoxContainer/MainAbilityPanel");
		// abilityPanels[0] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel1");
		// abilityPanels[1] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel2");
		// abilityPanels[2] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel3");

		healthContainer = GetNode<HBoxContainer>("HealthMarginContainer/HBoxContainer");
		upgradesContainer = GetNode<VBoxContainer>("AbilityMarginContainer/VBoxContainer");

		keyTexture = GetNode<TextureRect>("KeyMarginContainer/KeyTexture");
		keyNumberLabel = GetNode<Label>("KeyMarginContainer/KeyNumberLabel");
		timerLabel = GetNode<Label>("TimerLabel");
		sillyProgressBar = GetNode<ProgressBar>("SillyMarginContainer/SillyProgressBar");

		timerActive = true;
		updateHealth();
		updateUpgradePanels();
		updateKeyNumberLabel();
		updateSillyMeter();
	}

	public override async void _Process(double delta){
		if (timerActive){
			timeElapsed += delta;
		}
		timerLabel.Text = timeElapsed.ToString("0.00");
	}

	// private void updateAbilityPanels(){
	// 	mainAbilityPanel.ability = player.mainAbility.ability;
	// 	mainAbilityPanel.updatePanel();

	// 	for (int i = 0; i < player.abilities.Length; i++){
	// 		abilityPanels[i].ability = player.abilities[i].ability;
	// 		abilityPanels[i].updatePanel();
	// 	}
	// }

	private void updateHealth(){
		while (player.hp > healthIcons.Count){
			TextureRect newHealthIcon = healthIcon.Instantiate<TextureRect>();
			healthIcons.Add(newHealthIcon);
			healthContainer.AddChild(newHealthIcon);
		}
		
		while (player.hp < healthIcons.Count){
			healthContainer.RemoveChild(healthIcons[healthIcons.Count-1]);
			healthIcons[healthIcons.Count-1].QueueFree();
			healthIcons.RemoveAt(healthIcons.Count-1);
		}
	}

	private void updateUpgradePanels(){
		for (int i = 0; i < player.upgrades.Count; i++){
			if (i < upgradePanels.Count){
				upgradePanels[i].upgrade = player.upgrades[i];
				upgradePanels[i].updatePanel();
			} else {
				UpgradePanel newUpgradePanel = upgradePanelScene.Instantiate<UpgradePanel>();
				newUpgradePanel.upgrade = player.upgrades[i];
				upgradePanels.Add(newUpgradePanel);
				upgradesContainer.AddChild(newUpgradePanel);
				newUpgradePanel.updatePanel();
			}
		}
	}

	private void updateKeyNumberLabel(){
		keyNumberLabel.Text = player.keys.ToString();
	}

	private void updateSillyMeter(){
		sillyProgressBar.Value = player.sillyProgress;
	}

	private void _on_player_update_gui(){
		updateHealth();
		updateUpgradePanels();
		updateKeyNumberLabel();
		updateSillyMeter();
	}
}
