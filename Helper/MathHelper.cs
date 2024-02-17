public static class MathHelper
{
    public static float Percentage(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    public static double Percentage(double value, double min, double max)
    {
        return (value - min) / (max - min);
    }

    public static float Percentage(int value, int min, int max)
    {
        return Percentage((float)value, min, max);
    }
}
