using Godot;
using System;

public partial class Key : Interactable
{
	public override void Interact(){
		GD.Print("key");
        QueueFree();
    }
}
