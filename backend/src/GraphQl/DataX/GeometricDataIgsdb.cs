using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricDataIgsdb
    : DataIgsdb
{
    public GeometricDataIgsdb(
        string id,
        Guid? uuid,
        DateTime timestamp,
        Guid componentId,
        string? name,
        string? description,
        GetHttpsResourceTreeIgsdb resourceTree,
        IReadOnlyList<double> thicknesses
    ) : base(
        id,
        uuid,
        timestamp,
        componentId,
        name,
        description,
        resourceTree
    )
    {
        Thicknesses = thicknesses;
    }

    public IReadOnlyList<double> Thicknesses { get; }
}