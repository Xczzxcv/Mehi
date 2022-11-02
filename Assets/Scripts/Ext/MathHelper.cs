namespace Ext
{
public static class MathHelper
{
    public static double InverseLerp(double start, double end, double currValue)
    {
        return (currValue - start) / (end - start);
    }
}
}