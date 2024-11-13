using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class RemoveInstitutionRepresentativePayload
{
    private readonly InstitutionRepresentative? _association;

    public RemoveInstitutionRepresentativePayload(
        InstitutionRepresentative institutionRepresentative
    )
    {
        _association = institutionRepresentative;
    }

    public RemoveInstitutionRepresentativePayload(
        IReadOnlyCollection<RemoveInstitutionRepresentativeError> errors
    )
    {
        Errors = errors;
    }

    public RemoveInstitutionRepresentativePayload(
        RemoveInstitutionRepresentativeError error
    )
        : this([error])
    {
    }

    public IReadOnlyCollection<RemoveInstitutionRepresentativeError>? Errors { get; }

    public async Task<Institution?> GetInstitution(
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

    public async Task<User?> GetUser(
        UserByIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        if (_association is null)
        {
            return null;
        }

        return await byId.LoadAsync(_association.UserId, cancellationToken)!;
    }
}