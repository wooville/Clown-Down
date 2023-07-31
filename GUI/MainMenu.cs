using Godot;
using System;


public partial class MainMenu : Control
{
	private void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://World/MockGeneratedRoom.tscn");
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


