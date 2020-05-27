using Models = Icon.Models;
using System.Linq;
using GreenDonut;
using DateTime = System.DateTime;
using CancellationToken = System.Threading.CancellationToken;
using HotChocolate;
using IQueryBus = Icon.Infrastructure.Query.IQueryBus;
using IResolverContext = HotChocolate.Resolvers.IResolverContext;
using System.Collections.Generic;
using System.Threading.Tasks;
using IPageInfo = HotChocolate.Types.Relay.IPageInfo;

namespace Icon.GraphQl
{
    public sealed class ManufacturedComponentConnection
      : Connection
    {
        public ManufacturedComponentConnection(
            Institution institution
            )
          : base(
              fromId: institution.Id,
              pageInfo: null!,
              requestTimestamp: institution.RequestTimestamp
              )
        {
        }

        public async Task<IReadOnlyList<ManufacturedComponentEdge>> GetEdges(
            [DataLoader] ComponentsManufacturedByInstitutionAssociationDataLoader manufacturedComponentsLoader
            )
        {
            return (await manufacturedComponentsLoader.LoadAsync(
                  TimestampHelpers.TimestampId(FromId, RequestTimestamp)
                  )
                .ConfigureAwait(false)
                )
              .Select(a => new ManufacturedComponentEdge(a))
              .ToList().AsReadOnly();
        }

        public sealed class ComponentsManufacturedByInstitutionAssociationDataLoader
            : BackwardManyToManyAssociationsOfModelDataLoader<ComponentManufacturer, Models.Institution, Models.ComponentManufacturer>
        {
            public ComponentsManufacturedByInstitutionAssociationDataLoader(IQueryBus queryBus)
              : base(ComponentManufacturer.FromModel, queryBus)
            {
            }
        }

        public Task<IReadOnlyList<Component>> GetNodes(
            [DataLoader] ComponentsManufacturedByInstitutionDataLoader manufacturedComponentsLoader
            )
        {
            return manufacturedComponentsLoader.LoadAsync(
                TimestampHelpers.TimestampId(FromId, RequestTimestamp)
                );
        }

        public sealed class ComponentsManufacturedByInstitutionDataLoader
            : BackwardManyToManyAssociatesOfModelDataLoader<Component, Models.Institution, Models.ComponentManufacturer, Models.Component>
        {
            public ComponentsManufacturedByInstitutionDataLoader(IQueryBus queryBus)
              : base(Component.FromModel, queryBus)
            {
            }
        }
    }
}