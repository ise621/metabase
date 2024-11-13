using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Components;

namespace Metabase.GraphQl.ComponentGeneralizations;

public sealed class RemoveComponentGeneralizationPayload
{
    private readonly ComponentConcretizationAndGeneralization? _association;

    public RemoveComponentGeneralizationPayload(
        ComponentConcretizationAndGeneralization association
    )
    {
        _association = association;
    }

    public RemoveComponentGeneralizationPayload(
        IReadOnlyCollection<RemoveComponentGeneralizationError> errors
    )
    {
        Errors = errors;
    }

    public RemoveComponentGeneralizationPayload(
        RemoveComponentGeneralizationError error
    )
        : this([error])
    {
    }

    public IReadOnlyCollection<RemoveComponentGeneralizationError>? Errors { get; }

    public async Task<Component?> GetGeneralComponentAsync(
        ComponentByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.GeneralComponentId, cancellationToken)!;
    }

    public async Task<Component?> GetConcreteComponentAsync(
        ComponentByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.ConcreteComponentId, cancellationToken)!;
    }
}