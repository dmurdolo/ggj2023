using Godot;
using System;
using System.Linq;

public class OptionNodeStaticBody : Spatial
{
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			// Set PowerNode type
			PowerNode powerNode = GetParent().GetParent().GetParent().GetNode<PowerNode>("StaticBody");
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
						gameManager.Call("increase_current_power");
						hud.Call("update_power");
						break;

					case PowerNodeType.PowerUp:
						powerNode.PowerLevel++;
						gameManager.Call("decrease_current_power");
						hud.Call("update_power");
						break;

					case PowerNodeType.Growth:
						break;

					case PowerNodeType.Defence:
						break;
					
					case PowerNodeType.Decay:
						break;
				}

				// Close all Option Nodes
				powerNode.HideOptionNodes();
			}
		}
	}
}
