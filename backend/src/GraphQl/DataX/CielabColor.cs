namespace Metabase.GraphQl.DataX;

public sealed class CielabColor(
    double lStar,
    double aStar,
    double bStar
    )
{
    public double LStar { get; } = lStar;
    public double AStar { get; } = aStar;
    public double BStar { get; } = bStar;
}