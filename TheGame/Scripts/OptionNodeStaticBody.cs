using Godot;
using System;
using System.Linq;

public class OptionNodeStaticBody : Spatial
{
	private void StartEmitter(Particles particle) {
		if (!particle.Emitting) {
			particle.Emitting=true;
		} else {
			return;
		}
	}	
	
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			// Set PowerNode type
			PowerNode powerNode = GetParent().GetParent().GetParent().GetNode<PowerNode>("StaticBody");
			Particles particle;
			
			if (!powerNode.IsOptionsTweening)
			{
				powerNode.IsOptionsOpen = false;
				powerNode.Type = ((OptionNode)GetParent()).Type;
				powerNode.GetParent().GetNode<Label3D>("PowerLabel").Visible = true;

				var gameManager = GetNode("/root/Level/GameManager");
				var hud = GetNode("/root/Level/HUD");

				switch (powerNode.Type)
				{
					case PowerNodeType.PowerDown:
						powerNode.PowerLevel--;
						gameManager.Call("increase_current_energy");
						hud.Call("update_energy");
						if (powerNode.PowerLevel == 0) {
							powerNode.RemoveSpecialisedNodes();
						}
						break;

					case PowerNodeType.PowerUp:
						if (powerNode.PowerLevel < PowerNodeUtils.NODE_MAX_POWER_LEVEL) {
							powerNode.PowerLevel++;
							gameManager.Call("decrease_current_energy");
							hud.Call("update_energy");
						}
						break;

					case PowerNodeType.Growth:
						powerNode.HideOptionNodes();
						powerNode.RemoveSpecialisedNodes();

						// Add a GrowthNode
						PackedScene scene = (PackedScene)ResourceLoader.Load("res://Scenes/GrowthNode.tscn");
						GrowthNode growthNode = (GrowthNode)scene.Instance();
						powerNode.AddChild(growthNode);

						// Start particles
						particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");	
						particle.ProcessMaterial.Set("color", new Color(0,1,0,1));
						StartEmitter(particle);

						break;

					case PowerNodeType.Defence:
						powerNode.HideOptionNodes();
						powerNode.RemoveSpecialisedNodes();

						// Start particles
						particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");	
						particle.ProcessMaterial.Set("color", new Color(1,0,0,1));
						StartEmitter(particle);
						break;
					
					case PowerNodeType.Decay:
						powerNode.HideOptionNodes();
						powerNode.RemoveSpecialisedNodes();

						// Start particles
						particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");	
						particle.ProcessMaterial.Set("color", new Color(0.5f,0.15f,0.5f,1));
						StartEmitter(particle);
						break;
				}

				// Play audio
				AudioStreamPlayer audioStreamPlayer = GetParent().GetNode<AudioStreamPlayer>("AudioStreamPlayer");
				audioStreamPlayer.Play();
			}
		}
	}
}
