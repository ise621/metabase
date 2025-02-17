using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed record CoatedSidePropositionInput(
  CoatedSide? EqualTo,
  CoatedSide? NotEqualTo,
  IReadOnlyList<CoatedSide>? In,
  IReadOnlyList<CoatedSide>? NotIn
);