using Godot;
using System;

public class GrowthNode : Node
{
	private float growthRate = 10.0f;
	private SceneTreeTimer timer;
	private PowerNode powerNode;

	public override void _Ready()
	{
		powerNode = GetParent<PowerNode>();
		GrowPower();
	}

	public override void _Process(float delta)
	{
		if (timer != null)
		{
			var progressBar2D = GetNode("ProgressBar3D/Viewport/ProgressBar2D");
			progressBar2D.Set("max_value", GetPoweredGrowthRate());
			progressBar2D.Set("value", GetPoweredGrowthRate() - timer.TimeLeft);
		}
	}

	private async void GrowPower()
	{
		timer = GetTree().CreateTimer(GetPoweredGrowthRate());
		await ToSignal(timer, "timeout");

		var gameManager = GetNode("/root/Level/GameManager");
		int maxEnergyCap = (int)gameManager.Call("get_max_energy_capy");
		int maxEnergy = (int)gameManager.Call("get_max_energy");

		if (maxEnergy < maxEnergyCap)
		{
			gameManager.Call("increase_max_energy");
			gameManager.Call("increase_current_energy");

			var hud = GetNode("/root/Level/HUD");
			hud.Call("update_energy");

			GrowPower();
		}
	}

	private float GetPoweredGrowthRate()
	{
		return growthRate / powerNode.PowerLevel;
	}
}
