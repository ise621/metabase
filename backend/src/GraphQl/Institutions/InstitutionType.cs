using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionType
    : EntityType<Institution, InstitutionByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<Institution> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(t => t.DevelopedMethods)
            .Argument(nameof(InstitutionMethodDeveloper.Pending).FirstCharToLower(),
                _ => _.Type<NonNullType<BooleanType>>().DefaultValue(false))
            .Type<NonNullType<ObjectType<InstitutionDevelopedMethodConnection>>>()
            .Resolve(context =>
                new InstitutionDevelopedMethodConnection(
                    context.Parent<Institution>(),
                    context.ArgumentValue<bool>(nameof(InstitutionMethodDeveloper.Pending).FirstCharToLower())
                )
            );
        descriptor
            .Field(t => t.DevelopedMethodEdges)
            .Ignore();
        descriptor
            .Field(t => t.ManufacturedComponents)
            .Argument(nameof(ComponentManufacturer.Pending).FirstCharToLower(),
                _ => _.Type<NonNullType<BooleanType>>().DefaultValue(false))
            .Type<NonNullType<ObjectType<InstitutionManufacturedComponentConnection>>>()
            .Resolve(context =>
                new InstitutionManufacturedComponentConnection(
                    context.Parent<Institution>(),
                    context.ArgumentValue<bool>(nameof(ComponentManufacturer.Pending).FirstCharToLower())
                )
            );
        descriptor
            .Field(t => t.ManufacturedComponentEdges)
            .Ignore();
        descriptor
            .Field(t => t.ManagedDataFormats)
            .Type<NonNullType<ObjectType<InstitutionManagedDataFormatConnection>>>()
            .Resolve(context =>
                new InstitutionManagedDataFormatConnection(
                    context.Parent<Institution>()
                )
            );
        descriptor
            .Field(t => t.ManagedInstitutions)
            .Type<NonNullType<ObjectType<InstitutionManagedInstitutionConnection>>>()
            .Resolve(context =>
                new InstitutionManagedInstitutionConnection(
                    context.Parent<Institution>()
                )
            );
        descriptor
            .Field(t => t.ManagedMethods)
            .Type<NonNullType<ObjectType<InstitutionManagedMethodConnection>>>()
            .Resolve(context =>
                new InstitutionManagedMethodConnection(
                    context.Parent<Institution>()
                )
            );
        descriptor
            .Field(t => t.Manager)
            .Type<ObjectType<InstitutionManagerEdge>>()
            .Resolve(context =>
                {
                    var institution = context.Parent<Institution>();
                    return institution.ManagerId is null
                        ? null
                        : new InstitutionManagerEdge(institution);
                }
            );
        descriptor
            .Field(t => t.ManagerId)
            .Ignore();
        descriptor
            .Field(t => t.OperatedDatabases)
            .Type<NonNullType<ObjectType<InstitutionOperatedDatabaseConnection>>>()
            .Resolve(context =>
                new InstitutionOperatedDatabaseConnection(
                    context.Parent<Institution>()
                )
            );
        descriptor
            .Field(t => t.Representatives)
            .Argument(nameof(InstitutionRepresentative.Pending).FirstCharToLower(),
                _ => _.Type<NonNullType<BooleanType>>().DefaultValue(false))
            .Type<NonNullType<ObjectType<InstitutionRepresentativeConnection>>>()
            .Resolve(context =>
                new InstitutionRepresentativeConnection(
                    context.Parent<Institution>(),
                    context.ArgumentValue<bool>(nameof(InstitutionRepresentative.Pending).FirstCharToLower())
                )
            );
        descriptor
            .Field(t => t.RepresentativeEdges)
            .Ignore();
        descriptor
            .Field("canCurrentUserUpdateNode")
            .ResolveWith<InstitutionResolvers>(x =>
                InstitutionResolvers.GetCanCurrentUserUpdateNodeAsync(default!, default!, default!, default!,
                    default!))
            .UseUserManager();
        descriptor
            .Field("canCurrentUserDeleteNode")
            .ResolveWith<InstitutionResolvers>(x =>
                InstitutionResolvers.GetCanCurrentUserDeleteNodeAsync(default!, default!, default!, default!,
                    default!))
            .UseUserManager();
        descriptor
            .Field("canCurrentUserSwitchOperatingStateOfNode")
            .ResolveWith<InstitutionResolvers>(x =>
                InstitutionResolvers.GetCanCurrentUserSwitchOperatingStateOfNodeAsync(default!, default!, default!, default!,
                    default!))
            .UseUserManager();
    }

    private sealed class InstitutionResolvers
    {
        public static Task<bool> GetCanCurrentUserUpdateNodeAsync(
            [Parent] Institution institution,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return InstitutionAuthorization.IsAuthorizedToUpdateInstitution(claimsPrincipal, institution.Id,
                userManager, context, cancellationToken);
        }

        public static Task<bool> GetCanCurrentUserDeleteNodeAsync(
            [Parent] Institution institution,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return InstitutionAuthorization.IsAuthorizedToDeleteInstitution(claimsPrincipal, institution.Id,
                userManager, context, cancellationToken);
        }

        public static Task<bool> GetCanCurrentUserSwitchOperatingStateOfNodeAsync(
            [Parent] Institution institution,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return InstitutionAuthorization.IsAuthorizedToSwitchInstitutionOperatingState(claimsPrincipal, institution.Id,
                userManager, context, cancellationToken);
        }
    }
}