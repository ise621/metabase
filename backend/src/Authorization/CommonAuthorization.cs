using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Metabase.Data;
using Metabase.Enumerations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRole = Metabase.Enumerations.UserRole;

namespace Metabase.Authorization;

public static class CommonAuthorization
{
    public static bool IsSame(
        User user,
        Guid userId
    )
    {
        return user.Id == userId;
    }

    public static Task<bool> IsAdministrator(
        User user,
        UserManager<User> userManager
    )
    {
        return IsInRole(
            user,
            UserRole.ADMINISTRATOR,
            userManager
        );
    }

    public static async Task<bool> IsAdministrator(
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        return user is not null
               && await CommonAuthorization.IsAdministrator(user, userManager);
    }

    public static Task<bool> IsVerifier(
        User user,
        UserManager<User> userManager
    )
    {
        return IsInRole(
            user,
            UserRole.VERIFIER,
            userManager
        );
    }

    public static async Task<bool> IsOwner(
        User user,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var roles = await FetchRoles(
                   user,
                   context,
                   cancellationToken
               ).ConfigureAwait(false);

        if (roles == null) return false;
        return roles.Contains(InstitutionRepresentativeRole.OWNER);
    }

    public static async Task<bool> IsAtLeastAssistant(
        User user,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var roles = await FetchRoles(
                   user,
                   context,
                   cancellationToken
               ).ConfigureAwait(false);

        if (roles == null) return false;
        return roles.Contains(InstitutionRepresentativeRole.OWNER) || roles.Contains(InstitutionRepresentativeRole.ASSISTANT);
    }

    private static async Task<bool> IsInRole(
        User user,
        UserRole role,
        UserManager<User> userManager
    )
    {
        return await userManager.IsInRoleAsync(
            user,
            Role.EnumToName(role)
        ).ConfigureAwait(false);
    }

    public static async Task<bool> IsVerified(
        Guid institutionId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await context.Institutions.AsNoTracking()
            .AnyAsync(x =>
                    x.Id == institutionId &&
                    x.State == InstitutionState.VERIFIED,
                cancellationToken
            ).ConfigureAwait(false);
    }

    public static async Task<bool> IsOwnerOfInstitution(
        User user,
        Guid institutionId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await FetchRole(
                   user,
                   institutionId,
                   context,
                   cancellationToken
               ).ConfigureAwait(false)
               == InstitutionRepresentativeRole.OWNER;
    }

    public static async Task<bool> IsOwnerOfVerifiedInstitution(
        User user,
        Guid institutionId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await IsVerified(
                institutionId,
                context,
                cancellationToken
            ).ConfigureAwait(false) &&
            await IsOwnerOfInstitution(
                user,
                institutionId,
                context,
                cancellationToken
            ).ConfigureAwait(false);
    }

    public static async Task<bool> IsAtLeastAssistant(
        User user,
        Guid institutionId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var role = await FetchRole(
                user,
                institutionId,
                context,
                cancellationToken
            ).ConfigureAwait(false);
        return
            role is InstitutionRepresentativeRole.OWNER
            or InstitutionRepresentativeRole.ASSISTANT;
    }

    public static async Task<bool> IsAtLeastAssistantOfVerifiedInstitution(
        User user,
        Guid institutionId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await IsVerified(
                institutionId,
                context,
                cancellationToken
            ).ConfigureAwait(false) &&
            await IsAtLeastAssistant(
                user,
                institutionId,
                context,
                cancellationToken
            ).ConfigureAwait(false);
    }

    private static async Task<InstitutionRepresentativeRole?> FetchRole(
        User user,
        Guid institutionId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var wrappedRole = await context.InstitutionRepresentatives.AsNoTracking()
                .Where(x =>
                    x.InstitutionId == institutionId &&
                    x.UserId == user.Id &&
                    !x.Pending
                )
                .Select(x => new
                {
                    x.Role
                }) // We wrap the role in an object whose default value is `null`. Note that enumerations have the first value as default value.
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (wrappedRole is not null)
        {
            return wrappedRole.Role;
        }

        var wrappedManagerRole =
            await context.InstitutionRepresentatives.AsNoTracking()
                .Where(x => !x.Pending)
                .Join(
                    context.Institutions,
                    representative => representative.InstitutionId,
                    institution => institution.ManagerId,
                    (representative, institution) => new
                    { Representative = representative, Institution = institution }
                )
                .Where(x =>
                    x.Institution.Id == institutionId &&
                    x.Representative.UserId == user.Id
                )
                .Select(x => new
                {
                    x.Representative.Role
                }) // We wrap the role in an object whose default value is `null`. Note that enumerations have the first value as default value.
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        return wrappedManagerRole?.Role;
    }

    private static async Task<List<InstitutionRepresentativeRole>?> FetchRoles(
        User user,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await context.InstitutionRepresentatives.AsNoTracking()
                .Where(x => x.UserId == user.Id && !x.Pending)
                .Select(x => x.Role)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
    }

    public static async Task<bool> IsVerifiedManufacturerOfComponents(
        Guid institutionId,
        Guid[] componentIds,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (componentIds.Length == 0)
        {
            return true;
        }

        return await context.ComponentManufacturers.AsNoTracking()
            .AnyAsync(x =>
                    x.InstitutionId == institutionId &&
                    componentIds.Contains(x.ComponentId) &&
                    !x.Pending,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public static async Task<bool> IsVerifiedManufacturerOfComponent(
        Guid institutionId,
        Guid componentId,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return await context.ComponentManufacturers.AsNoTracking()
            .AnyAsync(x =>
                    x.InstitutionId == institutionId &&
                    x.ComponentId == componentId &&
                    !x.Pending,
                cancellationToken
            )
            .ConfigureAwait(false);
    }
}