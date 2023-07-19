using Godot;
using System;

public partial class Interactable : Area2D
{
    public bool isInteractable {get;set;} = false;
    public virtual void Interact(){
        GD.Print("Interacted with " + Name);
    }
}