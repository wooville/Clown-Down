using Godot;
using System;
using System.Collections.Generic;

public partial class MainGUI : Control
{
	private AbilityPanel[] abilityPanels = new AbilityPanel[3];
	private List<UpgradeIcon> upgradeIcons = new List<UpgradeIcon>();
	private List<TextureRect> healthIcons = new List<TextureRect>();
	private AbilityPanel mainAbilityPanel;
	private Player player;
	// private TextureRect keyTexture;
	private LevelManager levelManager;
	private Label keyNumberLabel;
	private Label clownNumberLabel;
	private Label guardNumberLabel;
	
	private Label timerLabel;
	private Label sillyLabel;
	private TextureProgressBar sillyProgressBar;
	private HBoxContainer healthContainer;
	private VBoxContainer upgradesContainer;

	public bool timerActive = false;
	public double timeElapsed {get;set;} = 0.0;

	private PackedScene upgradeIconScene = (PackedScene) ResourceLoader.Load("res://GUI/UpgradeIcon.tscn");
	private PackedScene healthIcon = (PackedScene) ResourceLoader.Load("res://GUI/HealthIcon.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// var player_max_health = $"../Characters/Player".max_health
		// bar.max_value = player_max_health
		player = (Player) GetTree().GetFirstNodeInGroup("player");
		levelManager = (LevelManager) GetTree().GetFirstNodeInGroup("manager");
		// mainAbilityPanel = GetNode<AbilityPanel>("AbilityMarginContainer/HBoxContainer/MainAbilityPanel");
		// abilityPanels[0] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel1");
		// abilityPanels[1] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel2");
		// abilityPanels[2] = GetNode<AbilityPanel>("AbilityMarginContainer/VBoxContainer/AbilityPanel3");

		healthContainer = GetNode<HBoxContainer>("HealthMarginContainer/HBoxContainer");
		upgradesContainer = GetNode<VBoxContainer>("AbilityMarginContainer/VBoxContainer");

		// keyTexture = GetNode<TextureRect>("StatsMarginContainer/VBoxContainer/KeyControl/KeyTexture");
		keyNumberLabel = GetNode<Label>("StatsMarginContainer/VBoxContainer/KeyControl/KeyNumberLabel");
		clownNumberLabel = GetNode<Label>("StatsMarginContainer/VBoxContainer/ClownsControl/ClownNumberLabel");
		guardNumberLabel = GetNode<Label>("StatsMarginContainer/VBoxContainer/GuardsControl/GuardNumberLabel");
		
		timerLabel = GetNode<Label>("TimerLabel");

		sillyLabel = GetNode<Label>("SillyMarginContainer/SillyLabel");
		sillyProgressBar = GetNode<TextureProgressBar>("SillyMarginContainer/SillyProgressBar");

		timerActive = true;
		updateHealth();
		updateUpgradeIcons();
		updateStatsLabels();
		updateSillyMeter();
	}

	public override async void _Process(double delta){
		if (timerActive){
			timeElapsed += delta;
		}
		var minutes = timeElapsed / 60;
		var seconds = timeElapsed % 60;
		var milliseconds = (timeElapsed % 1) *100;
		timerLabel.Text = ((int)minutes).ToString("00") + ":" + ((int)seconds).ToString("00") + ":" + ((int)milliseconds).ToString("00");
	}

	// private void updateAbilityPanels(){
	// 	mainAbilityPanel.ability = player.mainAbility.ability;
	// 	mainAbilityPanel.updateIcon();

	// 	for (int i = 0; i < player.abilities.Length; i++){
	// 		abilityPanels[i].ability = player.abilities[i].ability;
	// 		abilityPanels[i].updateIcon();
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

	private void updateUpgradeIcons(){
		for (int i = 0; i < player.upgrades.Count; i++){
			if (i < upgradeIcons.Count){
				upgradeIcons[i].upgrade = player.upgrades[i];
				upgradeIcons[i].updateIcon();
			} else {
				UpgradeIcon newUpgradeIcon = upgradeIconScene.Instantiate<UpgradeIcon>();
				newUpgradeIcon.upgrade = player.upgrades[i];
				upgradeIcons.Add(newUpgradeIcon);
				upgradesContainer.AddChild(newUpgradeIcon);
				newUpgradeIcon.updateIcon();
			}
		}
	}

	private void updateStatsLabels(){
		keyNumberLabel.Text = player.keys.ToString();
		clownNumberLabel.Text = levelManager.currentClownsFreed.ToString();
		guardNumberLabel.Text = levelManager.currentGuardsGoofed.ToString();
	}

	private void updateSillyMeter(){
		if (sillyProgressBar.Value >= 100) {
			sillyLabel.Visible = true;
		} else if (sillyProgressBar.Value <= 0){
			sillyLabel.Visible = false;
		}
		sillyProgressBar.Value = player.sillyProgress;
	}

	private void _on_player_update_gui(){
		updateHealth();
		updateUpgradeIcons();
		updateStatsLabels();
		updateSillyMeter();
	}
}
