using Godot;
using System;

public partial class GameOverChoiceControl : Control
{
	public bool selected {get;set;} = false;
	private Label Label;
	private TextureRect selectedIndicator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Label = GetNode<Label>("Panel/Label");
		selectedIndicator = GetNode<TextureRect>("SelectedIndicator");
	}

	public void updateControl(){
		if (selected){
			selectedIndicator.Visible = true;
		} else {
			selectedIndicator.Visible = false;
		}
	}
}
