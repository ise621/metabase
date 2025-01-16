using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Metabase.Data;
using OpenIddict.Core;

namespace Metabase.GraphQl.OpenIdConnect.Application
{
    public sealed class ApplicationByIdDataLoader
    : BatchDataLoader<Guid, OpenIdApplication?>
    {
        private OpenIddictApplicationManager<OpenIdApplication> _applicationManager;

        public ApplicationByIdDataLoader(
            IBatchScheduler batchScheduler,
            DataLoaderOptions options,
            OpenIddictApplicationManager<OpenIdApplication> applicationManager) : base(batchScheduler, options)
        {
            _applicationManager = applicationManager;
        }

        protected override async Task<IReadOnlyDictionary<Guid, OpenIdApplication?>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            var ret = new Dictionary<Guid, OpenIdApplication?>();
            foreach (var key in keys)
            {
                ret.Add(key, await _applicationManager.FindByIdAsync(key.ToString(), cancellationToken: cancellationToken).ConfigureAwait(false));
            }
            return ret;
        }
    }
}