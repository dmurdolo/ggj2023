using Godot;
using System;
using System.Linq;

public enum PowerNodeType 
{
	None,
	Power,
	Attack,
	Decay
};

public class PowerNode : Node
{
	private PowerNodeType type;
	public PowerNodeType Type {
		get {
			return type;
		}
		set {
			type = value;
			
			CSGSphere sphere = GetNode<CSGSphere>("CollisionShape/CSGSphere");
			sphere.Material = (Material)sphere.Material.Duplicate();
			((SpatialMaterial)sphere.Material).AlbedoColor = PowerNodeUtils.GetPowerNodeTypeColor(type);
		}
	}

	public bool IsOptionsOpen { get; set; }

	private bool isOptionsInitialised = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("[PowerNode] Ready");
		IsOptionsOpen = false;
	}
	
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			GD.Print("[PowerNode] Left Click");

			// Options are closed
			if (!IsOptionsOpen)
			{
				IsOptionsOpen = !IsOptionsOpen;

				// First time
				if (!isOptionsInitialised)
				{
					PackedScene scene = (PackedScene)ResourceLoader.Load("res://Scenes/OptionNode.tscn");
					
					Vector3[] locations = {
						new Vector3(-1.5f, 1.6f, 0.0f),
						new Vector3( 0.0f, 2.0f, 0.0f),
						new Vector3( 1.5f, 1.6f, 0.0f)
					};
					
					for (int i = 0; i < locations.Length; i++) {
						OptionNode optionNode = (OptionNode)scene.Instance();
						optionNode.Name = "OptionNode" + (i + 1);
						optionNode.Type = (PowerNodeType)(i + 1);
						optionNode.TweenFinalVal = locations[i];
						optionNode.TweenDuration = 0.5f + 0.05f * (i + 1);
						GetParent().AddChild(optionNode);
						optionNode.ShowOptionNode();
					}

					isOptionsInitialised = true;
				}

				// Option nodes already exist
				else
				{
					var optionNodes = GetParent().GetChildren().OfType<OptionNode>();
					optionNodes.ToList().ForEach(i => i.ShowOptionNode());
				}
			}

		}
	}
}
