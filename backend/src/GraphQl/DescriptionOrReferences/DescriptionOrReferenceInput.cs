using Metabase.GraphQl.References;

namespace Metabase.GraphQl.DescriptionOrReferences;

public sealed record DescriptionOrReferenceInput(
    ReferenceInput? Reference,
    string? Description
);