using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Components;

namespace Metabase.GraphQl.ComponentAssemblies;

public sealed class RemoveComponentAssemblyPayload
{
    private readonly ComponentAssembly? _association;

    public RemoveComponentAssemblyPayload(
        ComponentAssembly componentAssembly
    )
    {
        _association = componentAssembly;
    }

    public RemoveComponentAssemblyPayload(
        IReadOnlyCollection<RemoveComponentAssemblyError> errors
    )
    {
        Errors = errors;
    }

    public RemoveComponentAssemblyPayload(
        RemoveComponentAssemblyError error
    )
        : this([error])
    {
    }

    public IReadOnlyCollection<RemoveComponentAssemblyError>? Errors { get; }

    public async Task<Component?> GetAssembledComponentAsync(
        ComponentByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.AssembledComponentId, cancellationToken)!;
    }

    public async Task<Component?> GetPartComponentAsync(
        ComponentByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.PartComponentId, cancellationToken)!;
    }
}