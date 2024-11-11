using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Metabase.Authorization;
using Metabase.Configuration;
using Metabase.Data;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Metabase.GraphQl.ComponentAssemblies;

[ExtendObjectType(nameof(Mutation))]
public sealed class ComponentAssemblyMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<AddComponentAssemblyPayload> AddComponentAssemblyAsync(
        AddComponentAssemblyInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentAssemblyAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.PartComponentId,
                input.AssembledComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new AddComponentAssemblyPayload(
                new AddComponentAssemblyError(
                    AddComponentAssemblyErrorCode.UNAUTHORIZED,
                    "You are not authorized to add the component assembly.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<AddComponentAssemblyError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.AssembledComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentAssemblyError(
                    AddComponentAssemblyErrorCode.UNKNOWN_ASSEMBLED_COMPONENT,
                    "Unknown assembled component.",
                    new[] { nameof(input), nameof(input.AssembledComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.PartComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new AddComponentAssemblyError(
                    AddComponentAssemblyErrorCode.UNKNOWN_PART_COMPONENT,
                    "Unknown part component.",
                    new[] { nameof(input), nameof(input.PartComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new AddComponentAssemblyPayload(errors.AsReadOnly());

        if (await context.ComponentAssemblies.AsQueryable()
                .Where(a =>
                    a.AssembledComponentId == input.AssembledComponentId
                    && a.PartComponentId == input.PartComponentId
                )
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            return new AddComponentAssemblyPayload(
                new AddComponentAssemblyError(
                    AddComponentAssemblyErrorCode.DUPLICATE,
                    "Component assembly already exists.",
                    new[] { nameof(input) }
                )
            );

        var componentAssembly = new ComponentAssembly
        {
            AssembledComponentId = input.AssembledComponentId,
            PartComponentId = input.PartComponentId,
            Index = input.Index,
            PrimeSurface = input.PrimeSurface
        };
        context.ComponentAssemblies.Add(componentAssembly);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddComponentAssemblyPayload(componentAssembly);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateComponentAssemblyPayload> UpdateComponentAssemblyAsync(
        UpdateComponentAssemblyInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentAssemblyAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.PartComponentId,
                input.AssembledComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new UpdateComponentAssemblyPayload(
                new UpdateComponentAssemblyError(
                    UpdateComponentAssemblyErrorCode.UNAUTHORIZED,
                    "You are not authorized to update the component assembly.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<UpdateComponentAssemblyError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.AssembledComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new UpdateComponentAssemblyError(
                    UpdateComponentAssemblyErrorCode.UNKNOWN_ASSEMBLED_COMPONENT,
                    "Unknown assembled component.",
                    new[] { nameof(input), nameof(input.AssembledComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.PartComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new UpdateComponentAssemblyError(
                    UpdateComponentAssemblyErrorCode.UNKNOWN_PART_COMPONENT,
                    "Unknown part component.",
                    new[] { nameof(input), nameof(input.PartComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new UpdateComponentAssemblyPayload(errors.AsReadOnly());

        var componentAssembly =
            await context.ComponentAssemblies.AsQueryable()
                .Where(a =>
                    a.AssembledComponentId == input.AssembledComponentId
                    && a.PartComponentId == input.PartComponentId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (componentAssembly is null)
            return new UpdateComponentAssemblyPayload(
                new UpdateComponentAssemblyError(
                    UpdateComponentAssemblyErrorCode.UNKNOWN_ASSEMBLY,
                    "Unknown assembly.",
                    new[] { nameof(input) }
                )
            );

        componentAssembly.Index = input.Index;
        componentAssembly.PrimeSurface = input.PrimeSurface;
        context.ComponentAssemblies.Update(componentAssembly);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateComponentAssemblyPayload(componentAssembly);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<RemoveComponentAssemblyPayload> RemoveComponentAssemblyAsync(
        RemoveComponentAssemblyInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await ComponentAssemblyAuthorization.IsAuthorizedToManage(
                claimsPrincipal,
                input.PartComponentId,
                input.AssembledComponentId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new RemoveComponentAssemblyPayload(
                new RemoveComponentAssemblyError(
                    RemoveComponentAssemblyErrorCode.UNAUTHORIZED,
                    "You are not authorized to remove the component assembly.",
                    new[] { nameof(input) }
                )
            );

        var errors = new List<RemoveComponentAssemblyError>();
        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.AssembledComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentAssemblyError(
                    RemoveComponentAssemblyErrorCode.UNKNOWN_ASSEMBLED_COMPONENT,
                    "Unknown assembled component.",
                    new[] { nameof(input), nameof(input.AssembledComponentId).FirstCharToLower() }
                )
            );

        if (!await context.Components.AsQueryable()
                .Where(c => c.Id == input.PartComponentId)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
           )
            errors.Add(
                new RemoveComponentAssemblyError(
                    RemoveComponentAssemblyErrorCode.UNKNOWN_PART_COMPONENT,
                    "Unknown part component.",
                    new[] { nameof(input), nameof(input.PartComponentId).FirstCharToLower() }
                )
            );

        if (errors.Count is not 0) return new RemoveComponentAssemblyPayload(errors.AsReadOnly());

        var componentAssembly =
            await context.ComponentAssemblies.AsQueryable()
                .Where(a =>
                    a.AssembledComponentId == input.AssembledComponentId
                    && a.PartComponentId == input.PartComponentId
                )
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (componentAssembly is null)
            return new RemoveComponentAssemblyPayload(
                new RemoveComponentAssemblyError(
                    RemoveComponentAssemblyErrorCode.UNKNOWN_ASSEMBLY,
                    "Unknown assembly.",
                    new[] { nameof(input) }
                )
            );

        context.ComponentAssemblies.Remove(componentAssembly);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new RemoveComponentAssemblyPayload(componentAssembly);
    }
}