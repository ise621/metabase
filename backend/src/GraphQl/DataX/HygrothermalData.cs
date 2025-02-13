using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class HygrothermalData(
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
    IReadOnlyList<DataApproval> approvals
    // ResponseApproval approval
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
    )
{
}