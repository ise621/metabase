using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataIgsdb(
    string id,
    Guid? uuid,
    DateTime timestamp,
    Guid componentId,
    string? name,
    string? description,
    GetHttpsResourceTreeIgsdb resourceTree,
    IReadOnlyList<double> thicknesses
    )
        : DataIgsdb(
    id,
    uuid,
    timestamp,
    componentId,
    name,
    description,
    resourceTree
    )
{
    public IReadOnlyList<double> Thicknesses { get; } = thicknesses;
}