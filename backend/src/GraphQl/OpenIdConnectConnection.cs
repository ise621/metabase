﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Metabase.Data;

namespace Metabase.GraphQl;

public abstract class OpenIdConnectConnection<TSubject, TAssociation, TAssociationsByAssociateIdDataLoader, TEdge>
where TSubject : OpenIdApplication
where TAssociationsByAssociateIdDataLoader : IDataLoader<Guid, TAssociation[]>
{
    private readonly Func<TAssociation, TEdge> _createEdge;

    protected OpenIdConnectConnection(
    TSubject subject,
    Func<TAssociation, TEdge> createEdge)
    {
        Subject = subject;
        _createEdge = createEdge;
    }

    protected TSubject Subject { get; }

    public async Task<IEnumerable<TEdge>> GetEdgesAsync(
        TAssociationsByAssociateIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return (
                await dataLoader.LoadAsync(Subject.Id, cancellationToken).ConfigureAwait(false) ?? []
            )
            .Select(_createEdge);
    }
}