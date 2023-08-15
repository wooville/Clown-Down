using Godot;
using System;

public partial class Credits : Control
{
	[Export]
	public Panel[] panelList;
	private int currentSelect = 0;
	private TextureRect leftArrow;
	private TextureRect rightArrow;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		leftArrow = GetNode<TextureRect>("ArrowLeft");
		rightArrow = GetNode<TextureRect>("ArrowRight");
		checkTextures();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left") || Input.IsActionJustPressed("action_left")){
			panelList[currentSelect].Visible = false;
			currentSelect--;
			if (currentSelect < 0) currentSelect = 0;
			panelList[currentSelect].Visible = true;
			checkTextures();
		}

		if (Input.IsActionJustPressed("right") || Input.IsActionJustPressed("action_right")){
			panelList[currentSelect].Visible = false;
			currentSelect++;
			if (currentSelect >= panelList.Length) currentSelect = panelList.Length-1;
			panelList[currentSelect].Visible = true;
			checkTextures();
		}

		if (Input.IsActionJustPressed("dash_interact")){
			GetTree().ChangeSceneToFile("res://GUI/MainMenu.tscn");
		}
	}

	private void checkTextures(){
		if (currentSelect == 0){
			leftArrow.Visible = false;
		} else {
			leftArrow.Visible = true;
		}
		if (currentSelect == panelList.Length-1){
			rightArrow.Visible = false;
		} else {
			rightArrow.Visible = true;
		}
	}
}
