namespace MasTUICommon;

public class Color
{
    public int BackgroundColor
    {
        get;
        init
        {
            if (value > 16) value = 16;
            if (value < 0) value = 0;
            field = value;
        }
    }

    public int ForegroundColor
    {
        get; 
        init
        {
            if (value > 16) value = 16;
            if (value < 0) value = 0;
            field = value;
        }
    }

    public override string ToString()
    {
        char[] hexCodes = ['0', '1', '2', '3', '4', '5','6', '7', '8', '9', 'A', 'B', 'C', 'D','E', 'F', '-'];
        return $"{hexCodes[BackgroundColor]}{hexCodes[ForegroundColor]}";
    }
}