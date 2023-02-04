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

			CSGSphere sphere = GetNode<CSGSphere>("StaticBody/CollisionShape/CSGSphere");
			sphere.Material = (Material)sphere.Material.Duplicate();
			((SpatialMaterial)sphere.Material).AlbedoColor = PowerNodeUtils.GetPowerNodeTypeColor(type);
		}
	}
	
	private Tween tween;
	public bool IsTweening { get; set; }

	public override void _Ready()
	{
		IsTweening = false;

		tween = new Tween();
		AddChild(tween);
	}

	public void ShowOptionNode()
	{
		if (IsTweening) return;

		IsTweening = true;
		Visible = true;
		tween.InterpolateProperty(this, "translation", Vector3.Zero, TweenFinalVal, TweenDuration, Tween.TransitionType.Sine, Tween.EaseType.Out);
		tween.Connect("tween_completed", this, "ShowCompleted");
		tween.Start();
	}

	public void ShowCompleted(Godot.Object obj, NodePath key)
	{
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
		tween.Disconnect("tween_completed", this, "ShowCompleted");
		IsTweening = false;
	}

	public void HideOptionNode()
	{
		if (IsTweening) return;

		IsTweening = true;
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
		tween.InterpolateProperty(this, "translation", TweenFinalVal, Vector3.Zero, TweenDuration / 2.0f, Tween.TransitionType.Expo, Tween.EaseType.Out);
		tween.Connect("tween_completed", this, "HideCompleted");
		tween.Start();
	}

	public void HideCompleted(Godot.Object obj, NodePath key)
	{
		tween.Disconnect("tween_completed", this, "HideCompleted");
		Visible = false;	
		IsTweening = false;
	}
}
