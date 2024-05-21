using System.Diagnostics.CodeAnalysis;

namespace Metabase.GraphQl.Components;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateComponentErrorCode
{
    UNKNOWN,
    UNKNOWN_MANUFACTURER,
    UNKNOWN_FURTHER_MANUFACTURERS,
    UNKNOWN_VARIANT_OF_COMPONENTS,
    UNKNOWN_GENERALIZATION_OF_COMPONENTS,
    UNKNOWN_CONCRETIZATION_OF_COMPONENTS,
    UNKNOWN_PART_OF_COMPONENTS,
    UNKNOWN_ASSEMBLED_OF_COMPONENTS,
    UNAUTHORIZED,
    UNKNOWN_COMPONENT
}