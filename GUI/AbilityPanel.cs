using Godot;
using System;

public partial class AbilityPanel : Panel
{
	[Export]
	public Player.ABILITIES ability {get;set;}
	// crashed the game for some reason
	// {
	// 	get
	// 	{
	// 		return ability;
	// 	}
	// 	set
	// 	{
	// 		ability = value;
	// 		updatePanel();
	// 	}
	// }
	public bool isSelected {get;set;}
	// {
	// 	get
	// 	{
	// 		return isSelected;
	// 	}
	// 	set
	// 	{
	// 		isSelected = value;
	// 		updatePanel();
	// 	}
	// }
	private Label label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
	}

	public void updatePanel(){
		label.Text = ability.ToString();
	}
}
