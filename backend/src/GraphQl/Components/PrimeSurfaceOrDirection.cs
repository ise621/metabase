using Metabase.Data;

namespace Metabase.GraphQl.Components;

public sealed record PrimeSurfaceOrDirection(
    DescriptionOrReference? Surface,
    DescriptionOrReference? Direction
);