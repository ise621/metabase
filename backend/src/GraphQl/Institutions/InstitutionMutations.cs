using System;
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
using Metabase.Enumerations;
using Metabase.Extensions;
using Metabase.GraphQl.Users;
using Metabase.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRole = Metabase.Enumerations.UserRole;

namespace Metabase.GraphQl.Institutions;

[ExtendObjectType(nameof(Mutation))]
public sealed class InstitutionMutations
{
    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<CreateInstitutionPayload> CreateInstitutionAsync(
        CreateInstitutionInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        IEmailSender emailSender,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        if (input.ManagerId is not null && !await InstitutionAuthorization
                .IsAuthorizedToCreateInstitutionManagedByInstitution(
                    claimsPrincipal,
                    input.ManagerId ?? Guid.Empty,
                    userManager,
                    context,
                    cancellationToken
                ).ConfigureAwait(false)
           )
            return new CreateInstitutionPayload(
                new CreateInstitutionError(
                    CreateInstitutionErrorCode.UNAUTHORIZED,
                    "You are not authorized to create institutions.",
                    new[] { nameof(input), nameof(input.ManagerId).FirstCharToLower() }
                )
            );

        var user = await userManager.GetUserAsync(claimsPrincipal).ConfigureAwait(false);
        if (user is null)
            return new CreateInstitutionPayload(
                new CreateInstitutionError(
                    CreateInstitutionErrorCode.UNKNOWN,
                    "Unknown user.",
                    Array.Empty<string>()
                )
            );

        if (input.OwnerIds.Count is 0 && input.ManagerId is null)
            return new CreateInstitutionPayload(
                new CreateInstitutionError(
                    CreateInstitutionErrorCode.NEITHER_OWNER_NOR_MANAGER,
                    "Neither owner nor manager given.",
                    new[] { nameof(input) }
                )
            );

        var unknownOwnerIds =
            input.OwnerIds.Except(
                await context.Users.AsQueryable()
                    .Where(u => input.OwnerIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false)
            );
        if (unknownOwnerIds.Any())
            return new CreateInstitutionPayload(
                new CreateInstitutionError(
                    CreateInstitutionErrorCode.UNKNOWN_OWNERS,
                    $"There are no users with identifier(s) {string.Join(", ", unknownOwnerIds)}.",
                    new[] { nameof(input), nameof(input.OwnerIds).FirstCharToLower() }
                )
            );

        if (input.ManagerId is not null &&
            !await context.Institutions.AsQueryable()
                .AnyAsync(
                    x => x.Id == input.ManagerId,
                    cancellationToken
                )
                .ConfigureAwait(false)
           )
            return new CreateInstitutionPayload(
                new CreateInstitutionError(
                    CreateInstitutionErrorCode.UNKNOWN_MANAGER,
                    "Unknown manager.",
                    new[] { nameof(input), nameof(input.ManagerId).FirstCharToLower() }
                )
            );

        var institution = new Institution(
            input.Name,
            input.Abbreviation,
            input.Description,
            input.WebsiteLocator,
            input.PublicKey,
            await GetInitialInstitutionState(input, user, userManager).ConfigureAwait(false),
            InstitutionOperatingState.OPERATING
        )
        {
            ManagerId = input.ManagerId
        };
        foreach (var ownerId in input.OwnerIds)
            institution.RepresentativeEdges.Add(
                new InstitutionRepresentative
                {
                    UserId = ownerId,
                    Role = InstitutionRepresentativeRole.OWNER,
                    Pending = !await InstitutionRepresentativeAuthorization
                        .IsAuthorizedToConfirm(claimsPrincipal, ownerId, userManager).ConfigureAwait(false)
                }
            );

        context.Institutions.Add(institution);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (institution.State == InstitutionState.PENDING)
        {
            var verifiers =
                await userManager.GetUsersInRoleAsync(
                    Role.EnumToName(UserRole.VERIFIER)
                ).ConfigureAwait(false);
            await Task.WhenAll(
                verifiers.Select(verifier =>
                    verifier.Email is null
                        ? Task.CompletedTask
                        : emailSender.SendAsync(
                            (verifier.Name, verifier.Email),
                            $"New institution `{institution.Name}` in metabase awaits verification",
                            $"Dear {verifier.Name}, please verify institution '{institution.Name}' with UUID {institution.Id:D} on {appSettings.Host}/institutions Have a nice day! :-)"
                        )
                )
            ).ConfigureAwait(false);
        }

        return new CreateInstitutionPayload(institution);
    }

    private static async Task<InstitutionState> GetInitialInstitutionState(
        CreateInstitutionInput input,
        User user,
        UserManager<User> userManager
    )
    {
        if (input.ManagerId is not null
            || await userManager.IsInRoleAsync(user, Role.EnumToName(UserRole.ADMINISTRATOR))
                .ConfigureAwait(false)
            || await userManager.IsInRoleAsync(user, Role.EnumToName(UserRole.VERIFIER))
                .ConfigureAwait(false)
           )
            return InstitutionState.VERIFIED;

        return InstitutionState.PENDING;
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<VerifyInstitutionPayload> VerifyInstitutionAsync(
        VerifyInstitutionInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionAuthorization.IsAuthorizedToVerifyInstitution(
                claimsPrincipal,
                userManager
            ).ConfigureAwait(false)
           )
            return new VerifyInstitutionPayload(
                new VerifyInstitutionError(
                    VerifyInstitutionErrorCode.UNAUTHORIZED,
                    "You are not authorized to verify institutions.",
                    Array.Empty<string>()
                )
            );

        var institution =
            await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institution is null)
            return new VerifyInstitutionPayload(
                new VerifyInstitutionError(
                    VerifyInstitutionErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        institution.Verify();
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new VerifyInstitutionPayload(institution);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<UpdateInstitutionPayload> UpdateInstitutionAsync(
        UpdateInstitutionInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionAuthorization.IsAuthorizedToUpdateInstitution(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new UpdateInstitutionPayload(
                new UpdateInstitutionError(
                    UpdateInstitutionErrorCode.UNAUTHORIZED,
                    "You are not authorized to update the institution.",
                    Array.Empty<string>()
                )
            );

        var institution =
            await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institution is null)
            return new UpdateInstitutionPayload(
                new UpdateInstitutionError(
                    UpdateInstitutionErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        institution.Update(
            input.Name,
            input.Abbreviation,
            input.Description,
            input.WebsiteLocator,
            input.PublicKey
        );
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateInstitutionPayload(institution);
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<DeleteInstitutionPayload> DeleteInstitutionAsync(
        DeleteInstitutionInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionAuthorization.IsAuthorizedToDeleteInstitution(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new DeleteInstitutionPayload(
                new DeleteInstitutionError(
                    DeleteInstitutionErrorCode.UNAUTHORIZED,
                    "You are not authorized to delete the institution.",
                    Array.Empty<string>()
                )
            );

        var institution =
            await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institution is null)
            return new DeleteInstitutionPayload(
                new DeleteInstitutionError(
                    DeleteInstitutionErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        if (
            await context
                .Entry(institution)
                .Collection(i => i.ManagedDataFormats)
                .Query()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
            ||
            await context
                .Entry(institution)
                .Collection(i => i.ManagedInstitutions)
                .Query()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
            ||
            await context
                .Entry(institution)
                .Collection(i => i.ManagedMethods)
                .Query()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
            ||
            await context
                .Entry(institution)
                .Collection(i => i.OperatedDatabases)
                .Query()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
            ||
            await context
                .Entry(institution)
                .Collection(i => i.ManufacturedComponents)
                .Query()
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false)
        )
            return new DeleteInstitutionPayload(
                new DeleteInstitutionError(
                    DeleteInstitutionErrorCode.MANAGING,
                    "The institution manages or is associated to other entities.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );

        context.Institutions.Remove(institution);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new DeleteInstitutionPayload();
    }

    [UseUserManager]
    [Authorize(Policy = AuthConfiguration.WritePolicy)]
    public async Task<SwitchInstitutionOperatingStatePayload> SwitchInstitutionOperatingStateAsync(
        SwitchInstitutionOperatingStateInput input,
        ClaimsPrincipal claimsPrincipal,
        UserManager<User> userManager,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!await InstitutionAuthorization.IsAuthorizedToSwitchInstitutionOperatingState(
                claimsPrincipal,
                input.InstitutionId,
                userManager,
                context,
                cancellationToken
            ).ConfigureAwait(false)
           )
            return new SwitchInstitutionOperatingStatePayload(
                new SwitchInstitutionOperatingStateError(
                    SwitchInstitutionOperatingStateErrorCode.UNAUTHORIZED,
                    "You are not authorized to switch institution operating state.",
                    Array.Empty<string>()
                )
            );

        var institution =
            await context.Institutions.AsQueryable()
                .Where(i => i.Id == input.InstitutionId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        if (institution is null)
            return new SwitchInstitutionOperatingStatePayload(
                new SwitchInstitutionOperatingStateError(
                    SwitchInstitutionOperatingStateErrorCode.UNKNOWN_INSTITUTION,
                    "Unknown institution.",
                    new[] { nameof(input), nameof(input.InstitutionId).FirstCharToLower() }
                )
            );
        switch (institution.OperatingState)
        {
            case InstitutionOperatingState.NOT_OPERATING:
                institution.SwitchOperatingState(InstitutionOperatingState.OPERATING);
                break;
            case InstitutionOperatingState.OPERATING:
                institution.SwitchOperatingState(InstitutionOperatingState.NOT_OPERATING);
                break;
            default:
                throw new ArgumentException($"The operating state {institution.OperatingState} is not supported.");
        }
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new SwitchInstitutionOperatingStatePayload(institution);
    }
}