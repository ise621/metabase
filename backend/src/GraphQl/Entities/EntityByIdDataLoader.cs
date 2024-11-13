using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Metabase.Data;

namespace Metabase.GraphQl.Entities;

public abstract class EntityByIdDataLoader<TEntity>(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    Func<ApplicationDbContext, DbSet<TEntity>> getQueryable
    )
    : BatchDataLoader<Guid, TEntity?>(batchScheduler, options)
    where TEntity : class, IEntity
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;
    private readonly Func<ApplicationDbContext, DbSet<TEntity>> _getQueryable = getQueryable;

    protected override async Task<IReadOnlyDictionary<Guid, TEntity?>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext =
            _dbContextFactory.CreateDbContext();
        return await _getQueryable(dbContext).AsNoTrackingWithIdentityResolution()
            .Where(entity => keys.Contains(entity.Id))
            .ToDictionaryAsync(
                entity => entity.Id,
                entity => (TEntity?)entity,
                cancellationToken
            )
            .ConfigureAwait(false);
    }
}