using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalDataIgsdb(
    string id,
    Guid? uuid,
    DateTime timestamp,
    Guid componentId,
    string? name,
    string? description,
    GetHttpsResourceTreeIgsdb resourceTree,
    IReadOnlyList<double> nearnormalHemisphericalVisibleTransmittances,
    IReadOnlyList<double> nearnormalHemisphericalVisibleReflectances,
    IReadOnlyList<double> nearnormalHemisphericalSolarTransmittances,
    IReadOnlyList<double> nearnormalHemisphericalSolarReflectances,
    IReadOnlyList<double> infraredEmittances
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
    public IReadOnlyList<double> NearnormalHemisphericalVisibleTransmittances { get; } = nearnormalHemisphericalVisibleTransmittances;
    public IReadOnlyList<double> NearnormalHemisphericalVisibleReflectances { get; } = nearnormalHemisphericalVisibleReflectances;
    public IReadOnlyList<double> NearnormalHemisphericalSolarTransmittances { get; } = nearnormalHemisphericalSolarTransmittances;
    public IReadOnlyList<double> NearnormalHemisphericalSolarReflectances { get; } = nearnormalHemisphericalSolarReflectances;
    public IReadOnlyList<double> InfraredEmittances { get; } = infraredEmittances;
}