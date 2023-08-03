using Godot;
using System;
using System.Collections.Generic;

public partial class UpgradeIcon : TextureRect
{
	[Export]
	public Player.UPGRADES upgrade {get;set;}
	private Dictionary<Player.UPGRADES, Rect2> upgradeAtlasRegions = new Dictionary<Player.UPGRADES, Rect2>();
	private AtlasTexture atlas;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		atlas = (AtlasTexture) Texture;
		// label = GetNode<Label>("Label");
		// atlas.Region = new Rect2();
		upgradeAtlasRegions.Add(Player.UPGRADES.WHOOPIE_CUSHION, new Rect2(0, 0, 16, 16));
		upgradeAtlasRegions.Add(Player.UPGRADES.PIE, new Rect2(16, 0, 16, 16));
		upgradeAtlasRegions.Add(Player.UPGRADES.GUN, new Rect2(48, 0, 16, 16));
		upgradeAtlasRegions.Add(Player.UPGRADES.HORN, new Rect2(0, 16, 16, 16));
		upgradeAtlasRegions.Add(Player.UPGRADES.LOLLIPOP, new Rect2(16, 16, 16, 16));
		upgradeAtlasRegions.Add(Player.UPGRADES.FLOWER, new Rect2(32, 16, 16, 16));
		upgradeAtlasRegions.Add(Player.UPGRADES.NONE, new Rect2(48, 16, 16, 16));

		updateIcon();
	}

	public void updateIcon(){
		atlas.Region = upgradeAtlasRegions[upgrade];
	}
}
