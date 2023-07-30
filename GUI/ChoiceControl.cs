using Godot;
using System;

public partial class ChoiceControl : Control
{
	[Export]
	public Player.UPGRADES upgrade {get;set;}
	public bool selected {get;set;} = false;
	private Label titleLabel;
	private Label contentLabel;
	private TextureRect texture;
	private ColorRect selectedIndicator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleLabel = GetNode<Label>("Panel/TitleLabel");
		contentLabel = GetNode<Label>("Panel/ContentLabel");
		texture = GetNode<TextureRect>("TextureRect");
		selectedIndicator = GetNode<ColorRect>("ColorRect");
	}

	public void updateControl(){
		switch (upgrade){
			case Player.UPGRADES.FLOWER:
				titleLabel.Text = "Water Flower";
				contentLabel.Text = "";
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
