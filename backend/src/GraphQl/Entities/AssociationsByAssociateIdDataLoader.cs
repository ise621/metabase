using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;

namespace Metabase.GraphQl.Entities;

public abstract class AssociationsByAssociateIdDataLoader<TAssociation>(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    Func<ApplicationDbContext, IReadOnlyList<Guid>, IQueryable<TAssociation>> getAssociations,
    Func<TAssociation, Guid> getAssociateId
    )
    : GroupedDataLoader<Guid, TAssociation>(batchScheduler, options)
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;

    private readonly Func<TAssociation, Guid> _getAssociateId = getAssociateId;

    private readonly Func<ApplicationDbContext, IReadOnlyList<Guid>, IQueryable<TAssociation>>
        _getAssociations = getAssociations;

    protected override async Task<ILookup<Guid, TAssociation>> LoadGroupedBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext =
            _dbContextFactory.CreateDbContext();
        return (
                await _getAssociations(dbContext, keys)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false)
            )
            .ToLookup(_getAssociateId);
    }
}