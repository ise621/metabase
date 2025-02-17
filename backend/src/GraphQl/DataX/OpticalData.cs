using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalData(
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
    OpticalComponentType? type,
    OpticalComponentSubtype? subtype,
    CoatedSide? coatedSide,
    AppliedMethod appliedMethod,
    IReadOnlyList<GetHttpsResource> resources,
    GetHttpsResourceTree resourceTree,
    IReadOnlyList<DataApproval> approvals,
    // ResponseApproval approval
    IReadOnlyList<double> nearnormalHemisphericalVisibleTransmittances,
    IReadOnlyList<double> nearnormalHemisphericalVisibleReflectances,
    IReadOnlyList<double> nearnormalHemisphericalSolarTransmittances,
    IReadOnlyList<double> nearnormalHemisphericalSolarReflectances,
    IReadOnlyList<double> infraredEmittances,
    IReadOnlyList<double> colorRenderingIndices,
    IReadOnlyList<CielabColor> cielabColors
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
    appliedMethod,
    resources,
    resourceTree,
    approvals
    )
{
    public OpticalComponentType? Type { get; } = type;
    public OpticalComponentSubtype? Subtype { get; } = subtype;
    public CoatedSide? CoatedSide { get; } = coatedSide;
    public IReadOnlyList<double> NearnormalHemisphericalVisibleTransmittances { get; } = nearnormalHemisphericalVisibleTransmittances;
    public IReadOnlyList<double> NearnormalHemisphericalVisibleReflectances { get; } = nearnormalHemisphericalVisibleReflectances;
    public IReadOnlyList<double> NearnormalHemisphericalSolarTransmittances { get; } = nearnormalHemisphericalSolarTransmittances;
    public IReadOnlyList<double> NearnormalHemisphericalSolarReflectances { get; } = nearnormalHemisphericalSolarReflectances;

    public IReadOnlyList<double> InfraredEmittances { get; } = infraredEmittances;

    public IReadOnlyList<double> ColorRenderingIndices { get; } = colorRenderingIndices;
    public IReadOnlyList<CielabColor> CielabColors { get; } = cielabColors;
}