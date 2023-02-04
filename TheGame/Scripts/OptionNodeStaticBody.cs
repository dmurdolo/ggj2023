using Godot;
using System;
using System.Linq;

public class OptionNodeStaticBody : Spatial
{
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			//GD.Print($"[OptionNodeStaticBody] ({GetParent().Name}) Left Click");
			
			// Set PowerNode type
			PowerNode powerNode = GetParent().GetParent().GetParent().GetNode<PowerNode>("StaticBody");
			if (!powerNode.IsOptionsTweening)
			{
				powerNode.IsOptionsOpen = false;
				powerNode.Type = ((OptionNode)GetParent()).Type;

				switch (powerNode.Type)
				{
					case PowerNodeType.Power:
						powerNode.GetParent().GetNode<Label3D>("PowerLabel").Visible = true;
						break;

					case PowerNodeType.Attack:
						powerNode.GetParent().GetNode<Label3D>("PowerLabel").Visible = false;
						break;

					case PowerNodeType.Decay:
						powerNode.GetParent().GetNode<Label3D>("PowerLabel").Visible = false;
						break;
				}

				// Close all Option Nodes
				var optionNodes = GetParent().GetParent().GetChildren().OfType<OptionNode>();
				optionNodes.ToList().ForEach(i => i.HideOptionNode());
			}
		}
	}
}
