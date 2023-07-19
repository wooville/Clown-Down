using Godot;
using System;

public partial class AbilityPanel : Panel
{
	[Export]
	public Player.ABILITIES ability {get;set;}
	public Label label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.Text = ability.ToString();
	}

	public void updatePanel(Player.ABILITIES newAbility){
		GD.Print("panel");
		ability = newAbility;
		label.Text = ability.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
