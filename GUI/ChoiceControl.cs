using Godot;
using System;
using System.Collections.Generic;

public partial class ChoiceControl : Control
{
	[Export]
	public Player.UPGRADES upgrade {get;set;}
	public bool selected {get;set;} = false;
	private Label titleLabel;
	private Label contentLabel;
	private UpgradeIcon icon;
	private ColorRect selectedIndicator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleLabel = GetNode<Label>("Panel/TitleLabel");
		contentLabel = GetNode<Label>("Panel/ContentLabel");
		icon = GetNode<UpgradeIcon>("UpgradeIcon");
		selectedIndicator = GetNode<ColorRect>("ColorRect");
		
	}

	public void updateControl(){
		icon.upgrade = upgrade;
		icon.updateIcon();

		switch (upgrade){
			case Player.UPGRADES.WHOOPIE_CUSHION:
				titleLabel.Text = "Whoopie Cushion";
				contentLabel.Text = "Dash farther and faster";
				break;
			case Player.UPGRADES.PIE:
				titleLabel.Text = "Throwing Pie";
				contentLabel.Text = "Throw pies during Silly Time";
				break;
			case Player.UPGRADES.GUN:
				titleLabel.Text = "Bang Gun";
				contentLabel.Text = "Increase parrying range";
				break;
			case Player.UPGRADES.HORN:
				titleLabel.Text = "Clown Horn";
				contentLabel.Text = "Parries give more meter";
				break;
			case Player.UPGRADES.LOLLIPOP:
				titleLabel.Text = "Jumbo Lollipop";
				contentLabel.Text = "Silly Time lasts longer";
				break;
			case Player.UPGRADES.FLOWER:
				titleLabel.Text = "Water Flower";
				contentLabel.Text = "Parry redirects projectiles";
				break;
			default:
				titleLabel.Text = upgrade.ToString();
				break;
		}
		if (selected){
			selectedIndicator.Visible = true;
		} else {
			selectedIndicator.Visible = false;
		}
	}
}
