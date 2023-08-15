using Godot;
using System;

public partial class ShaderController : Camera2D
{
	private ColorRect screenRect;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenRect = GetNode<ColorRect>("CanvasLayer/ScreenRect");
	}

	private void updateOffset(float offset){
		(screenRect.Material as ShaderMaterial).SetShaderParameter("offset", offset);
	}
}
