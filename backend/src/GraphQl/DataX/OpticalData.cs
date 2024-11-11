using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class OpticalData
    : Data
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

    public OpticalData(
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
    ) : base(
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
        NearnormalHemisphericalVisibleTransmittances = nearnormalHemisphericalVisibleTransmittances;
        NearnormalHemisphericalVisibleReflectances = nearnormalHemisphericalVisibleReflectances;
        NearnormalHemisphericalSolarTransmittances = nearnormalHemisphericalSolarTransmittances;
        NearnormalHemisphericalSolarReflectances = nearnormalHemisphericalSolarReflectances;
        InfraredEmittances = infraredEmittances;
        ColorRenderingIndices = colorRenderingIndices;
        CielabColors = cielabColors;
    }

    public IReadOnlyList<double> NearnormalHemisphericalVisibleTransmittances { get; }
    public IReadOnlyList<double> NearnormalHemisphericalVisibleReflectances { get; }
    public IReadOnlyList<double> NearnormalHemisphericalSolarTransmittances { get; }
    public IReadOnlyList<double> NearnormalHemisphericalSolarReflectances { get; }

    public IReadOnlyList<double> InfraredEmittances { get; }

    public IReadOnlyList<double> ColorRenderingIndices { get; }
    public IReadOnlyList<CielabColor> CielabColors { get; }
}