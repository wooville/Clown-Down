using Godot;
using System;

public partial class AbilityPickup : Interactable
{
	[Export]
	public Player.ABILITIES ability;
	private Sprite2D sprite;
	// private Texture2D distraction = (Texture2D) ResourceLoader.Load("res://Player/Abilities/Distraction.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// sprite = GetNode<Sprite2D>("Sprite2D");

		// switch (ability){
		// 	case Player.ABILITIES.DISTRACT:
		// 	sprite.Texture = 
		// 	break;
		// }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void Interact(){
		GD.Print("yay " + ability);
	}
}
