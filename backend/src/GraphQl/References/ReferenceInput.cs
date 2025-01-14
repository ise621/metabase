using Metabase.GraphQl.Publications;
using Metabase.GraphQl.Standards;

namespace Metabase.GraphQl.References;

public sealed record ReferenceInput(
    StandardInput? Standard,
    PublicationInput? Publication
);