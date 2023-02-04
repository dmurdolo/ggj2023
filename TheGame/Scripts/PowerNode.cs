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

	public int PowerLevel = 0;

	private bool isOptionsInitialised = false;
	private OptionNode[] optionNodes = new OptionNode[3];

	private Label3D powerLabel;
	private Spatial optionNodeParent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsOptionsOpen = false;
		IsOptionsTweening = false;

		powerLabel = GetParent().GetNode<Label3D>("PowerLabel");
		powerLabel.Visible = false;

		optionNodeParent = GetParent().GetNode<Spatial>("OptionNodeParent");
	}

	public override void _Process(float delta)
	{
		// Check if tweening open has finished
		if (isOptionsInitialised && IsOptionsOpen && IsOptionsTweening)
		{
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

		// Rotate PowerLabel
		powerLabel.LookAt(GetViewport().GetCamera().GlobalTransform.Orthonormalized().origin, Vector3.Up);
		powerLabel.RotateObjectLocal(Vector3.Up, Mathf.Deg2Rad(180));

		// Rotate OptionNodeParent
		optionNodeParent.LookAt(GetViewport().GetCamera().GlobalTransform.Orthonormalized().origin, Vector3.Up);
		optionNodeParent.RotateObjectLocal(Vector3.Up, Mathf.Deg2Rad(180));
	}
	
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			if (IsOptionsTweening) return;

			// Options are closed
			if (!IsOptionsOpen)
			{
				GD.Print("[PowerNode] Showing options");

				IsOptionsOpen = true;
				IsOptionsTweening = true;

				// First time
				if (!isOptionsInitialised)
				{
					GD.Print("[PowerNode] Init options");
					PackedScene scene = (PackedScene)ResourceLoader.Load("res://Scenes/OptionNode.tscn");
					
					Vector3[] locations = {
						new Vector3(-2.0f, 1.5f, 0.0f),
						new Vector3( 0.0f, 3.0f, -1.0f),
						new Vector3( 2.0f, 1.5f, 0.0f)
					};
					
					for (int i = 0; i < locations.Length; i++) {
						OptionNode optionNode = (OptionNode)scene.Instance();
						optionNode.IsTweening = true;
						optionNode.Name = "OptionNode" + (i + 1);
						optionNode.Type = (PowerNodeType)(i + 1);
						optionNode.TweenFinalVal = locations[i];
						optionNode.TweenDuration = 0.3f + 0.05f * (i + 1);
						optionNodeParent.AddChild(optionNode);
						optionNode.ShowOptionNode();

						// Store the OptionNode in an array
						optionNodes[i] = optionNode;
					}

					isOptionsInitialised = true;
				}

				// Option nodes already exist
				else
				{
					optionNodes.ToList().ForEach(i => i.ShowOptionNode());
				}
			}

			// Options are open
			else if (IsOptionsOpen && !IsOptionsTweening)
			{
				GD.Print("[PowerNode] Hiding options");
				optionNodes.ToList().ForEach(i => i.HideOptionNode());
				IsOptionsOpen = false;
			}

		}
	}
}
