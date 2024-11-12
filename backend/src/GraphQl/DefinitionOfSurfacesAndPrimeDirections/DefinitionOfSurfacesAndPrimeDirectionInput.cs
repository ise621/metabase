using Metabase.GraphQl.References;

namespace Metabase.GraphQl.DefinitionOfSurfacesAndPrimeDirections;

public sealed record DefinitionOfSurfacesAndPrimeDirectionInput(
    ReferenceInput? Reference,
    string? Description
);