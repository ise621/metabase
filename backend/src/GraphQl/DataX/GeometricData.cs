using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricData(
    string id,
    Guid uuid,
    DateTime timestamp,
    string locale,
    Guid databaseId,
    Guid componentId,
    string? name,
    string? description,
    IReadOnlyList<string> warnings,
    Guid creatorId,
    DateTime createdAt,
    DataType? type,
    DataSubtype? subtype,
    CoatedSide? coatedSide,
    AppliedMethod appliedMethod,
    IReadOnlyList<GetHttpsResource> resources,
    GetHttpsResourceTree resourceTree,
    IReadOnlyList<DataApproval> approvals,
    // ResponseApproval approval,
    IReadOnlyList<double> thicknesses
    )
        : Data(
    id,
    uuid,
    timestamp,
    locale,
    databaseId,
    componentId,
    name,
    description,
    warnings,
    creatorId,
    createdAt,
    type,
    subtype,
    coatedSide,
    appliedMethod,
    resources,
    resourceTree,
    approvals
    // approval
    )
{
    public IReadOnlyList<double> Thicknesses { get; } = thicknesses;
}