namespace WebApi;

public static class Utils
{
    private const float Minimum = 0.0f;
    private const float Maximum = 1.0f;

    public static float Truncate(float value) => value switch
    {
        < Minimum => Minimum,
        > Maximum => Maximum,
        _ => value
    };
}