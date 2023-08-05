using Godot;
using System;

public partial class Interactable : Area2D
{
    public bool isInteractable {get;set;} = false;
    protected MarginContainer buttonPromptContainer;
    protected Label buttonPromptLabel;

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        buttonPromptContainer = GetNode<MarginContainer>("ButtonPromptContainer");
        buttonPromptLabel = GetNode<Label>("ButtonPromptContainer/Label");
	}

    public virtual void Interact(){
        QueueFree();
    }

    public void ToggleButtonPrompt(){
        buttonPromptContainer.Visible = !buttonPromptContainer.Visible;
    }

    private void _on_area_entered(Area2D area){
        if (area.IsInGroup("player")){
            isInteractable = true;
            ToggleButtonPrompt();
            // GD.Print("in");
        }
    }

    private void _on_area_exited(Area2D area){
        if (area.IsInGroup("player")){
            isInteractable = false;
            ToggleButtonPrompt();
            // GD.Print("out");
        }
    }
}