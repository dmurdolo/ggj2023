using Godot;
using System;
using System.Linq; 	 	

public class PowerNode : Node
{
	public bool IsOptionsOpen { get; set; }
	public bool IsOptionsTweening { get; set; }

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

	private bool isOptionsInitialised = false;
	private OptionNode[] optionNodes = new OptionNode[3];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsOptionsOpen = false;
		IsOptionsTweening = false;
	}

	public override void _Process(float delta)
	{
		// Check if tweening has finished
		if (isOptionsInitialised && IsOptionsOpen && IsOptionsTweening)
		{
			//var optionNodes = GetParent().GetChildren().OfType<OptionNode>();
			bool isStillTweening = false;
			
			foreach(OptionNode n in optionNodes)
			{
				if (n.IsTweening) 
				{
					isStillTweening = true;
				}
			}

			if (!isStillTweening) {
				IsOptionsTweening = false;
				//GD.Print("TWEENDONE");
			}
		}
	}
	
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			if (IsOptionsTweening) return;

			// Options are closed
			if (!IsOptionsOpen)
			{
				//GD.Print("[PowerNode] Left Click");

				IsOptionsOpen = true;
				IsOptionsTweening = true;

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
						optionNode.IsTweening = true;
						optionNode.Name = "OptionNode" + (i + 1);
						optionNode.Type = (PowerNodeType)(i + 1);
						optionNode.TweenFinalVal = locations[i];
						optionNode.TweenDuration = 0.5f + 0.05f * (i + 1);
						GetParent().AddChild(optionNode);
						optionNode.ShowOptionNode();

						// Store the OptionNode in an array
						optionNodes[i] = optionNode;
					}

					isOptionsInitialised = true;
				}

				// Option nodes already exist
				else
				{
					//var optionNodes = GetParent().GetChildren().OfType<OptionNode>();
					optionNodes.ToList().ForEach(i => i.ShowOptionNode());
				}
			}

		}
	}
}
