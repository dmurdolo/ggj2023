using Godot;
using System;

public class PowerNode : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("[PowerNode] Ready");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	public override void _Input(InputEvent inputEvent)
	{
		/*if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch ((ButtonList) mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:
					GD.Print($"Left button was clicked at {mouseEvent.Position}");
					break;
					
				case ButtonList.WheelUp:
					GD.Print("Wheel up");
					break;
			}
		}*/
	}
	
	private void _on_StaticBody_input_event(object camera, object @event, Vector3 position, Vector3 normal, int shape_idx)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == 1)
		{
			GD.Print("[PowerNode] Left Click");
			
			//var camera = GetNode<Camera>("Camera");
			//var from = camera.ProjectRayOrigin(eventMouseButton.Position);
			//var to = from + camera.ProjectRayNormal(eventMouseButton.Position) * rayLength;
		}
	}
}
