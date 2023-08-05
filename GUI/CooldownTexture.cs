using Godot;
using System;

public partial class CooldownTexture : TextureRect
{
	// public Timer timer;
	private Tween tween;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// timer = GetNode<Timer>("CooldownTimer");
	}

	public void animateCooldown(){
		tween = GetTree().CreateTween();
		var full_mod = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		var no_mod = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		Modulate = no_mod;
		tween.TweenProperty(this, "modulate", full_mod, 1.0);
	}

	public void instantCooldown(){
		var full_mod = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		tween.Stop();
		Modulate = full_mod;
	}
}
