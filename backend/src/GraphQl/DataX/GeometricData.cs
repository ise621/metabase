using System;
using System.Collections.Generic;

namespace Metabase.GraphQl.DataX;

public sealed class GeometricData
    : Data
{
    internal static GeometricData From(GeometricDataIgsdb node)
    {
        return new GeometricData(
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
            node.Thicknesses
        );
    }

    public GeometricData(
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
        // IReadOnlyList<DataApproval> approvals,
        // ResponseApproval approval,
        IReadOnlyList<double> thicknesses
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
        // approvals
        // approval
    )
    {
        Thicknesses = thicknesses;

    }
    public IReadOnlyList<double> Thicknesses { get; }

}