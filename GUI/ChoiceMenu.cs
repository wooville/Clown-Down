using Godot;
using System;

public partial class ChoiceMenu : Control
{
	
	private ChoiceControl choiceControl1;
	private ChoiceControl choiceControl2;
	[Signal]
	public delegate void UpgradeAcquiredEventHandler(Player.UPGRADES upgrade);

	public override void _Ready()
	{
		choiceControl1 = GetNode<ChoiceControl>("ChoiceControl1");
		choiceControl2 = GetNode<ChoiceControl>("ChoiceControl2");
	}

	public override async void _Process(double delta){
		if (Input.IsActionJustPressed("left") || Input.IsActionJustPressed("action_left")){
			choiceControl1.selected = true;
			choiceControl2.selected = false;
			choiceControl1.updateControl();
			choiceControl2.updateControl();
		}

		if (Input.IsActionJustPressed("right") || Input.IsActionJustPressed("action_right")){
			choiceControl1.selected = false;
			choiceControl2.selected = true;
			choiceControl1.updateControl();
			choiceControl2.updateControl();
		}

		if (Input.IsActionJustPressed("dash_interact")){
			if (choiceControl1.selected){
				GetTree().Paused = false;
				EmitSignal(SignalName.UpgradeAcquired, (int) choiceControl1.upgrade);
				CallDeferred(MethodName.SetVisible, false);
				CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.Disabled);
			} else if (choiceControl2.selected){
				GetTree().Paused = false;
				EmitSignal(SignalName.UpgradeAcquired, (int) choiceControl2.upgrade);
				CallDeferred(MethodName.SetVisible, false);
				CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.Disabled);
			}
		}
	}

	private void presentChoice(Player.UPGRADES choice1, Player.UPGRADES choice2){
		CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
		choiceControl1.upgrade = choice1;
		choiceControl2.upgrade = choice2;
		choiceControl1.updateControl();
		choiceControl2.updateControl();
		CallDeferred(MethodName.SetVisible, true);
	}

	// private void _on_item_safe_upgrade_choice(Player.UPGRADES choice1, Player.UPGRADES choice2){
	// 	CallDeferred(MethodName.SetProcessMode, (int) ProcessModeEnum.WhenPaused);
	// 	choiceControl1.upgrade = choice1;
	// 	choiceControl2.upgrade = choice2;
	// 	choiceControl1.updateControl();
	// 	choiceControl2.updateControl();
	// 	CallDeferred(MethodName.SetVisible, true);
	// }
}
