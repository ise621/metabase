using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed record GeometricDataPropositionInput(
    UuidPropositionInput? ComponentId,
    IReadOnlyList<GeometricDataPropositionInput>? And,
    GeometricDataPropositionInput? Not,
    IReadOnlyList<GeometricDataPropositionInput>? Or,
    GetHttpsResourcesPropositionInput? Resources,
    FloatsPropositionInput? Thicknesses
);