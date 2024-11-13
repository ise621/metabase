using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Components;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class RemoveComponentManufacturerPayload
{
    private readonly ComponentManufacturer? _association;

    public RemoveComponentManufacturerPayload(
        ComponentManufacturer componentManufacturer
    )
    {
        _association = componentManufacturer;
    }

    public RemoveComponentManufacturerPayload(
        IReadOnlyCollection<RemoveComponentManufacturerError> errors
    )
    {
        Errors = errors;
    }

    public RemoveComponentManufacturerPayload(
        RemoveComponentManufacturerError error
    )
        : this([error])
    {
    }

    public IReadOnlyCollection<RemoveComponentManufacturerError>? Errors { get; }

    public async Task<Component?> GetComponentAsync(
        ComponentByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.ComponentId, cancellationToken)!;
    }

    public async Task<Institution?> GetInstitutionAsync(
        InstitutionByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.InstitutionId, cancellationToken)!;
    }
}