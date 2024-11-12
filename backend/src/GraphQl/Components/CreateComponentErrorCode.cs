using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.Components;

[SuppressMessage("Naming", "CA1707")]
public enum CreateComponentErrorCode
{
    UNKNOWN,
    UNKNOWN_MANUFACTURER,
    UNAUTHORIZED,
    AMBIGUOUS_REFERENCE_IN_DEFINITION_OF_SURFACES_AND_PRIME_DIRECTION
}