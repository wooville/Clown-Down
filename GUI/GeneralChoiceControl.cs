using Godot;
using System;

public partial class GeneralChoiceControl : Control
{
	public bool selected {get;set;} = false;
	private Control selectedIndicator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectedIndicator = GetNode<Control>("SelectedIndicator");
	}

	public void updateControl(){
		if (selected){
			selectedIndicator.Visible = true;
		} else {
			selectedIndicator.Visible = false;
		}
	}
}
