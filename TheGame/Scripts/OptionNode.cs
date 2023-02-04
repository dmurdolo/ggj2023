using Godot;
using System;

public class OptionNode : Spatial
{
	public Vector3 TweenFinalVal { get; set; }
	public float TweenDuration{ get; set; }

	private PowerNodeType type;
	public PowerNodeType Type {
		get {
			return type;
		}
		set {
			type = value;

			_InitAudio(type);

			CSGSphere sphere = GetNode<CSGSphere>("StaticBody/CollisionShape/CSGSphere");
			sphere.Material = (Material)sphere.Material.Duplicate();
			((SpatialMaterial)sphere.Material).AlbedoColor = PowerNodeUtils.GetPowerNodeTypeColor(type);
		}
	}
	
	private Tween tween;
	public bool IsTweening { get; set; }

	private AudioStreamPlayer audioStreamPlayer;

	public override void _Ready()
	{
		IsTweening = false;

		tween = new Tween();
		AddChild(tween);
	}

	private void _InitAudio(PowerNodeType type)
	{
		string filename = "";

		switch (type)
		{
			case PowerNodeType.PowerDown:
				filename = "power_down";
				break;
			
			case PowerNodeType.PowerUp:
				filename = "power_up";
				break;

			case PowerNodeType.Growth:
				filename = "growth";
				break;

			case PowerNodeType.Defence:
				filename = "defence";
				break;

			case PowerNodeType.Decay:
				filename = "decay";
				break;
		}

		AudioStreamMP3 stream = (AudioStreamMP3)GD.Load("res://Audio/" + filename + ".mp3");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioStreamPlayer.Stream = stream;
	}

	public void ShowOptionNode()
	{
		if (IsTweening) return;

		IsTweening = true;
		Visible = true;
		tween.InterpolateProperty(this, "translation", Vector3.Zero, TweenFinalVal, TweenDuration, Tween.TransitionType.Sine, Tween.EaseType.Out);
		tween.Connect("tween_completed", this, "ShowCompleted");
		tween.Start();

		var gameManager = GetNode("/root/Level/GameManager");
		int currentPower = (int)gameManager.Call("get_current_power");
		int maxPower = (int)gameManager.Call("get_max_power");

		PowerNode powerNode = (PowerNode)GetParent().GetParent().GetNode("StaticBody");
		
		bool isPowerNodeValid = false;
		switch (type) {
			case PowerNodeType.PowerDown:
				isPowerNodeValid = powerNode.PowerLevel > 0;
				break;
			
			case PowerNodeType.PowerUp:
				isPowerNodeValid = currentPower > 0;
				break;

			case PowerNodeType.Growth:
			case PowerNodeType.Defence:
			case PowerNodeType.Decay:
				isPowerNodeValid = powerNode.PowerLevel > 0;
				break;
		}

		// Set enabled/disabled
		CSGSphere sphere = GetNode<CSGSphere>("StaticBody/CollisionShape/CSGSphere");
		if (isPowerNodeValid) {
			GetNode("StaticBody").SetBlockSignals(false);
			((SpatialMaterial)sphere.Material).AlbedoColor = PowerNodeUtils.GetPowerNodeTypeColor(type);
		} else {
			GetNode("StaticBody").SetBlockSignals(true);
			((SpatialMaterial)sphere.Material).AlbedoColor = PowerNodeUtils.GetDisabledPowerNodeColor();
		}

		// Play audio
		powerNode.GetParent().GetNode<AudioStreamPlayer>("NodeOpeningAudio").Play();
	}

	public void ShowCompleted(Godot.Object obj, NodePath key)
	{
		tween.Disconnect("tween_completed", this, "ShowCompleted");
		IsTweening = false;
	}

	public void HideOptionNode()
	{
		if (IsTweening) return;

		IsTweening = true;
		tween.InterpolateProperty(this, "translation", TweenFinalVal, Vector3.Zero, TweenDuration / 2.0f, Tween.TransitionType.Expo, Tween.EaseType.Out);
		tween.Connect("tween_completed", this, "HideCompleted");
		tween.Start();

		// Play audio
		audioStreamPlayer.Play();   // There is a BUG here, not sure why 2 sounds are playing.
	}

	public void HideCompleted(Godot.Object obj, NodePath key)
	{
		tween.Disconnect("tween_completed", this, "HideCompleted");
		Visible = false;	
		IsTweening = false;
	}
}
