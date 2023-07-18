using Godot;
using System;


public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://World/TestLevel.tscn");
	}

	private void _on_options_button_pressed()
	{
		// Replace with function body.
	}

	private void _on_quit_button_pressed()
	{
		GetTree().Quit();
	}
}
