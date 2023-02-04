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
			
			// TODO replace with a particle effect
//			CSGSphere sphere = GetNode<CSGSphere>("CollisionShape/CSGSphere");
//			sphere.Material = (Material)sphere.Material.Duplicate();
//			((SpatialMaterial)sphere.Material).AlbedoColor = PowerNodeUtils.GetPowerNodeTypeColor(type);
		}
	}

	private int powerLevel = 0;
	public int PowerLevel {
		get {
			return powerLevel;
		}
		set {
			powerLevel = value;

			powerLabel.Text = PowerLevel.ToString();
		}
	}

	private bool isOptionsInitialised = false;
	private OptionNode[] optionNodes = new OptionNode[5];

	private Label3D powerLabel;
	private Spatial optionNodeParent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsOptionsOpen = false;
		IsOptionsTweening = false;
		
		powerLabel = GetParent().GetNode<Label3D>("PowerLabel");
		powerLabel.Text = PowerLevel.ToString();
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
			
			var gameManager = GetNode("/root/Level/GameManager");

			// Options are closed
			if (!IsOptionsOpen)
			{
				GD.Print("[PowerNode] Showing options");

				// Close all previous OptionNodes
				Node currentPowerNode = (Node)gameManager.Call("get_current_power_node");
				if (currentPowerNode != null) {
					((PowerNode)currentPowerNode).HideOptionNodes();
				}

				// Set state.
				IsOptionsOpen = true;
				IsOptionsTweening = true;

				// First time opening options on this power node
				if (!isOptionsInitialised)
				{
					//GD.Print("[PowerNode] Init options");
					PackedScene scene = (PackedScene)ResourceLoader.Load("res://Scenes/OptionNode.tscn");
					
					Vector3[] locations = {
						new Vector3(-3.0f, 1.5f, 0.0f),
						new Vector3(-2.0f, 1.5f, 0.0f),
						new Vector3( 0.0f, 3.0f, -1.0f),
						new Vector3( 1.0f, 2.5f, -1.0f),
						new Vector3( 2.0f, 1.5f, 0.0f)
					};

					for (int i = 0; i < locations.Length; i++) {
						CreateOptionNode(i, scene, locations[i]);
					}

					isOptionsInitialised = true;
				}

				// Option nodes already exist
				else
				{
					optionNodes.ToList().ForEach(i => i.ShowOptionNode());
				}

				// Set current state
				gameManager.Call("set_current_power_node", this);
			}

			// Options are open
			else if (IsOptionsOpen && !IsOptionsTweening)
			{
				HideOptionNodes();
			}
		}
	}

	private void CreateOptionNode(int i, PackedScene scene, Vector3 tweenFinalVal)
	{
		OptionNode optionNode = (OptionNode)scene.Instance();
		optionNode.IsTweening = true;
		optionNode.Name = "OptionNode" + (i + 1);
		optionNode.Type = (PowerNodeType)(i + 1);
		optionNode.TweenFinalVal = tweenFinalVal;
		optionNode.TweenDuration = 0.3f + 0.05f * (i + 1);
		optionNodeParent.AddChild(optionNode);
		optionNode.ShowOptionNode();

		// Store the OptionNode in an array
		optionNodes[i] = optionNode;
	}

	public void HideOptionNodes()
	{
		GD.Print("[PowerNode] Hiding options");
		optionNodes.ToList().ForEach(i => i.HideOptionNode());
		IsOptionsOpen = false;
		
		var gameManager = GetNode("/root/Level/GameManager");
		gameManager.Call("clear_current_power_node");
	}
}
