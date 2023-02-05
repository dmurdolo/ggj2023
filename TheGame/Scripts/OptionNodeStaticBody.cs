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
			Node towerNode = GetParent().GetParent().GetParent().GetParent();
			PowerNode powerNode = GetParent().GetParent().GetParent().GetNode<PowerNode>("StaticBody");
			Particles particle = null;
			
			if (!powerNode.IsOptionsTweening)
			{
				powerNode.IsOptionsOpen = false;
				powerNode.Type = ((OptionNode)GetParent()).Type;
				powerNode.GetParent().GetNode<Label3D>("PowerLabel").Visible = true;

				var gameManager = GetNode("/root/Level/GameManager");
				var centralRootManager = GetNode("/root/Level/CentralRoot");
				var hud = GetNode("/root/Level/HUD");

				switch (powerNode.Type)
				{
					case PowerNodeType.PowerDown:
						powerNode.PowerLevel--;
						gameManager.Call("increase_current_energy");
						hud.Call("update_energy");

						if (powerNode.PowerLevel == 0 && particle != null) {
							particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");						
							particle.Emitting=false;
						}
						break;

					case PowerNodeType.PowerUp:
						powerNode.PowerLevel++;
						gameManager.Call("decrease_current_energy");
						hud.Call("update_energy");
						
						bool IsAdjacentToCore = false;
						if (!powerNode.IsCentralNode) {
							IsAdjacentToCore = (bool)towerNode.Get("adjacent_to_core");

							if (powerNode.PowerLevel >= 1 && IsAdjacentToCore) {
								centralRootManager.Call("update_surrounding_node_has_power", true);
							} else {
								centralRootManager.Call("update_surrounding_node_has_power", false);
							}
						}
						break;

					case PowerNodeType.Growth:
						particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");	
						particle.ProcessMaterial.Set("color", new Color(0,1,0,1));
						StartEmitter(particle);
						break;

					case PowerNodeType.Defence:						
						particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");	
						particle.ProcessMaterial.Set("color", new Color(1,0,0,1));
						StartEmitter(particle);
						break;
					
					case PowerNodeType.Decay:				
						particle = GetParent().GetParent().GetParent().GetNode<Particles>("Particle");	
						particle.ProcessMaterial.Set("color", new Color(0.5f,0.15f,0.5f,1));
						StartEmitter(particle);
						break;
				}

				// Play audio
				AudioStreamPlayer audioStreamPlayer = GetParent().GetNode<AudioStreamPlayer>("AudioStreamPlayer");
				audioStreamPlayer.Play();

				// Close all Option Nodes
				powerNode.HideOptionNodes();
			}
		}
	}
}
