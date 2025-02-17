using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed record OpticalComponentTypePropositionInput(
  OpticalComponentType? EqualTo,
  OpticalComponentType? NotEqualTo,
  IReadOnlyList<OpticalComponentType>? In,
  IReadOnlyList<OpticalComponentType>? NotIn
);