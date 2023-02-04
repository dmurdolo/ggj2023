using Godot;
using System;
using System.Linq;

public class OptionNodeStaticBody : Spatial
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			//GD.Print($"[OptionNodeStaticBody] ({GetParent().Name}) Left Click");
			
			// Set PowerNode to closed
			var powerNodes = GetParent().GetParent().GetChildren().OfType<PowerNode>();
			powerNodes.ToList().ForEach(i => i.IsOptionsOpen = false);

			// Close all Option Nodes
			var optionNodes = GetParent().GetParent().GetChildren().OfType<OptionNode>();
			optionNodes.ToList().ForEach(i => i.HideOptionNode());
		}
	}
}
