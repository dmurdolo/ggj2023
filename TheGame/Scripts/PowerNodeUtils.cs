using Godot;

public enum PowerNodeType 
{
	None,
	PowerDown,
	PowerUp,
	Growth,
	Defence,
	Decay
};

public class PowerNodeUtils
{
	public static Color GetPowerNodeTypeColor(PowerNodeType type)
	{
		Color color = new Color(0,0,0,1);

		switch (type)
		{
			case PowerNodeType.PowerDown:
				color = new Color(0.5f,0,0,1);
				break;
			
			case PowerNodeType.PowerUp:
				color = new Color(0,0.5f,0,1);
				break;

			case PowerNodeType.Growth:
				color = new Color(0,1,0,1);
				break;
			
			case PowerNodeType.Defence:
				color = new Color(1,0,0,1);
				break;

			case PowerNodeType.Decay:
				color = new Color(0.5f,0.15f,0.5f,1);
				break;
		}
		
		return color;
	}

	public static Color GetDisabledPowerNodeColor()
	{
		return new Color(0.5f,0.5f,0.5f,1);
	}
}
