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
        Color color = new Color("000000");;

        switch (type)
        {
            case PowerNodeType.PowerDown:
                color = new Color("#00ff00");
                break;
            
            case PowerNodeType.PowerUp:
                color = new Color("#00ff00");
                break;

            case PowerNodeType.Growth:
                color = new Color("#00ff00");
                break;
            
            case PowerNodeType.Defence:
                color = new Color("ff0000");
                break;

            case PowerNodeType.Decay:
                color = new Color("000000");
                break;
        }
        
        return color;
    }

    public static Color GetDisabledPowerNodeColor()
    {
        return new Color("dddddd");
    }
}