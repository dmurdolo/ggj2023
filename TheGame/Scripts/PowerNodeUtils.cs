using Godot;

public class PowerNodeUtils
{
    public static Color GetPowerNodeTypeColor(PowerNodeType type)
    {
        Color color = new Color("000000");;

        switch (type)
        {
            case PowerNodeType.Attack:
                color = new Color("ff0000");
                break;

            case PowerNodeType.Power:
                color = new Color("#00ff00");
                break;

            case PowerNodeType.Decay:
                color = new Color("000000");
                break;
        }
        
        return color;
    }
}