using Metabase.Data;

namespace Metabase.GraphQl.Components;

public sealed record PrimeSurfaceOrDirection(
    DescriptionOrReference? PrimeSurface,
    DescriptionOrReference? PrimeDirection
);