using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;

namespace Metabase.GraphQl.Entities;

public abstract class AssociationsByAssociateIdDataLoader<TAssociation>
    : GroupedDataLoader<Guid, TAssociation>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    private readonly Func<TAssociation, Guid> _getAssociateId;

    private readonly Func<ApplicationDbContext, IReadOnlyList<Guid>, IQueryable<TAssociation>>
        _getAssociations;

    protected AssociationsByAssociateIdDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        Func<ApplicationDbContext, IReadOnlyList<Guid>, IQueryable<TAssociation>> getAssociations,
        Func<TAssociation, Guid> getAssociateId
    )
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
        _getAssociations = getAssociations;
        _getAssociateId = getAssociateId;
    }

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