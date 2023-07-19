using Godot;
using System;

public partial class Interactable : Area2D
{
    public bool isInteractable {get;set;} = false;
    protected MarginContainer buttonPromptContainer;

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        buttonPromptContainer = GetNode<MarginContainer>("ButtonPromptContainer");
	}

    public virtual void Interact(){
        GD.Print("Interacted with " + Name);
    }

    public void ToggleButtonPrompt(){
        buttonPromptContainer.Visible = !buttonPromptContainer.Visible;
    }
}