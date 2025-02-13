using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.DataX;

[SuppressMessage("Naming", "CA1707")]
public enum CoatedSide
{
    FRONT,
    BACK,
    BOTH,
    NEITHER
}