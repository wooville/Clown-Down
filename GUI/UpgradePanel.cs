using Godot;
using System;

public partial class UpgradePanel : Panel
{
	[Export]
	public Player.UPGRADES upgrade {get;set;}
	private Label label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
	}

	public void updatePanel(){
		label.Text = upgrade.ToString();
	}
}
