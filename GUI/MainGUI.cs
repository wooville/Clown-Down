using Godot;
using System;

public partial class MainGUI : Control
{
	private AbilityPanel[] abilityPanels = new AbilityPanel[3];
	private AbilityPanel mainAbilityPanel;
	private Player player;
	private TextureRect keyTexture;
	private Label timerLabel;
	private ProgressBar sillyProgressBar;

	public bool timerActive = false;
	public double timeElapsed {get;set;} = 0.0;

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

		keyTexture = GetNode<TextureRect>("KeyMarginContainer/KeyTexture");
		timerLabel = GetNode<Label>("TimerLabel");
		sillyProgressBar = GetNode<ProgressBar>("SillyMarginContainer/SillyProgressBar");

		timerActive = true;
	}

	public override async void _Process(double delta){
		if (timerActive){
			timeElapsed += delta;
		}
		timerLabel.Text = timeElapsed.ToString("0.00");
	}

	private void updateAbilityPanels(){
		mainAbilityPanel.ability = player.mainAbility.ability;
		mainAbilityPanel.updatePanel();

		for (int i = 0; i < player.abilities.Length; i++){
			abilityPanels[i].ability = player.abilities[i].ability;
			abilityPanels[i].updatePanel();
		}
	}

	private void updateKeyTexture(){
		if (player.hasKey){
			keyTexture.Modulate = new Godot.Color(1, 1, 1, 255);
		} else {
			keyTexture.Modulate = new Godot.Color(0.5f, 0.5f, 0.5f, 80);
		}
	}

	private void updateSillyMeter(){
		sillyProgressBar.Value = player.sillyProgress;
	}

	private void _on_player_update_gui(){
		updateAbilityPanels();
		updateKeyTexture();
		updateSillyMeter();
	}
}
