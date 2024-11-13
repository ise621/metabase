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
    AppliedMethod appliedMethod,
    IReadOnlyList<GetHttpsResource> resources,
    GetHttpsResourceTree resourceTree,
    // IReadOnlyList<DataApproval> approvals
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
    resourceTree
    )
{
    internal static OpticalData From(OpticalDataIgsdb node)
    {
        return new OpticalData(
            node.Id,
            node.Uuid ?? node.ComponentId, // The IGSDB has one data set per component.
            node.Timestamp,
            IgsdbLocale,
            new Guid(IgsdbDatabaseId),
            node.ComponentId,
            node.Name,
            node.Description,
            Array.Empty<string>().AsReadOnly(),
            new Guid(IgsdbInstitutionId), // We suppose that LBNL created the data set.
            DateTime.UtcNow, // That is the best date-time information we have.
            new AppliedMethod(
                new Guid(IgsdbMethodId),
                Array.Empty<NamedMethodArgument>().AsReadOnly(),
                Array.Empty<NamedMethodSource>().AsReadOnly()
            ),
            [GetHttpsResource.From(node.ResourceTree.Root.Value)],
            GetHttpsResourceTree.From(node.ResourceTree),
            // node.Approvals
            // node.Approval
            node.NearnormalHemisphericalVisibleTransmittances,
            node.NearnormalHemisphericalVisibleReflectances,
            node.NearnormalHemisphericalSolarTransmittances,
            node.NearnormalHemisphericalSolarReflectances,
            node.InfraredEmittances,
            Array.Empty<double>().AsReadOnly(),
            Array.Empty<CielabColor>().AsReadOnly()
        );
    }

    public IReadOnlyList<double> NearnormalHemisphericalVisibleTransmittances { get; } = nearnormalHemisphericalVisibleTransmittances;
    public IReadOnlyList<double> NearnormalHemisphericalVisibleReflectances { get; } = nearnormalHemisphericalVisibleReflectances;
    public IReadOnlyList<double> NearnormalHemisphericalSolarTransmittances { get; } = nearnormalHemisphericalSolarTransmittances;
    public IReadOnlyList<double> NearnormalHemisphericalSolarReflectances { get; } = nearnormalHemisphericalSolarReflectances;

    public IReadOnlyList<double> InfraredEmittances { get; } = infraredEmittances;

    public IReadOnlyList<double> ColorRenderingIndices { get; } = colorRenderingIndices;
    public IReadOnlyList<CielabColor> CielabColors { get; } = cielabColors;
}