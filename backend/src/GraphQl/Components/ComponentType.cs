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

namespace Metabase.GraphQl.Components;

public sealed class ComponentType
    : EntityType<Component, ComponentByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<Component> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(t => t.Manufacturers)
            .Argument(nameof(ComponentManufacturer.Pending).FirstCharToLower(),
                _ => _.Type<NonNullType<BooleanType>>().DefaultValue(false))
            .Type<NonNullType<ObjectType<ComponentManufacturerConnection>>>()
            .Resolve(context =>
                new ComponentManufacturerConnection(
                    context.Parent<Component>(),
                    context.ArgumentValue<bool>(nameof(ComponentManufacturer.Pending).FirstCharToLower())
                )
            );
        descriptor
            .Field(t => t.ManufacturerEdges).Ignore();
        descriptor
            .Field(t => t.Parts)
            .Name("assembledOf")
            .Type<NonNullType<ObjectType<ComponentAssembledOfConnection>>>()
            .Resolve(context =>
                new ComponentAssembledOfConnection(
                    context.Parent<Component>()
                )
            );
        descriptor
            .Field(t => t.PartEdges).Ignore();
        descriptor
            .Field(t => t.PartOf)
            .Type<NonNullType<ObjectType<ComponentPartOfConnection>>>()
            .Resolve(context =>
                new ComponentPartOfConnection(
                    context.Parent<Component>()
                )
            );
        descriptor
            .Field(t => t.PartOfEdges).Ignore();
        descriptor
            .Field(t => t.Generalizations)
            .Name("concretizationOf")
            .Type<NonNullType<ObjectType<ComponentConcretizationOfConnection>>>()
            .Resolve(context =>
                new ComponentConcretizationOfConnection(
                    context.Parent<Component>()
                )
            );
        descriptor
            .Field(t => t.GeneralizationEdges)
            .Ignore();
        descriptor
            .Field(t => t.Concretizations)
            .Name("generalizationOf")
            .Type<NonNullType<ObjectType<ComponentGeneralizationOfConnection>>>()
            .Resolve(context =>
                new ComponentGeneralizationOfConnection(
                    context.Parent<Component>()
                )
            );
        descriptor
            .Field(t => t.ConcretizationEdges)
            .Ignore();
        descriptor
            .Field(t => t.Variants)
            .Ignore();
        descriptor
            .Field(t => t.VariantEdges).Ignore();
        descriptor
            .Field(t => t.VariantOf)
            .Type<NonNullType<ObjectType<ComponentVariantOfConnection>>>()
            .Resolve(context =>
                new ComponentVariantOfConnection(
                    context.Parent<Component>()
                )
            );
        descriptor
            .Field(t => t.VariantOfEdges).Ignore();
        descriptor
            .Field("canCurrentUserUpdateNode")
            .ResolveWith<ComponentResolvers>(x =>
                ComponentResolvers.GetCanCurrentUserUpdateNodeAsync(default!, default!, default!, default!,
                    default!))
            .UseUserManager();
    }

    private sealed class ComponentResolvers
    {
        public static Task<bool> GetCanCurrentUserUpdateNodeAsync(
            [Parent] Component component,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager,
            ApplicationDbContext context,
            CancellationToken cancellationToken
        )
        {
            return ComponentAuthorization.IsAuthorizedToUpdate(claimsPrincipal, component.Id, userManager, context,
                cancellationToken);
        }
    }
}